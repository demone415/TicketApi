using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using TicketApi.Shared.Errors;
using TicketApi.Shared.Swagger;
using DateTimeOffsetConverter = TicketApi.Shared.Converters.DateTimeOffsetConverter;

namespace TicketApi.Shared.Configuration;

public abstract class StartupBase
{
    /// <summary>
    /// Текущее окружение
    /// </summary>
    protected IWebHostEnvironment Environment { get; }

    /// <summary>
    /// Конфигурация сервиса
    /// </summary>
    protected IConfiguration Configuration { get; }

    /// <summary>
    /// Конфигурация сервиса
    /// </summary>
    protected ServiceOptions ServiceOptions { get; }

    /// <summary>
    /// .ctor
    /// </summary>
    protected StartupBase(IWebHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
        var instance = new ServiceOptions();
        Configuration.GetSection("Service").Bind(instance);
        ServiceOptions = instance;
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
    }

    /// <summary>Регистрирует сервисы в IoC-контейнере.</summary>
    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(lb => lb.ClearProviders().AddSerilog());
        services.AddOptions();
        services.Configure<ServiceOptions>(Configuration.GetSection("Service"));
        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = ServiceOptions.GetDefaultApiVersion();
        });
        services.AddVersionedApiExplorer(options =>
        {
            options.DefaultApiVersion = ServiceOptions.GetDefaultApiVersion();
            options.ApiVersionParameterSource = new UrlSegmentApiVersionReader();
            options.SubstituteApiVersionInUrl = true;
        });
        AddSwagger(services);
        services.Configure<MvcOptions>(options =>
        {
            options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
        });
        services.AddControllers();
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetConverter());
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });
        ConfigureHealthChecks(services.AddHealthChecks());

        services.AddSingleton(_ =>
            new ActivitySource("TicketApi", ServiceOptions.DefaultVersion));
    }

    /// <summary>Конфигурирование проверок состояния сервиса.</summary>
    protected virtual void ConfigureHealthChecks(IHealthChecksBuilder healthChecksBuilder)
    {
    }

    /// <summary>
    /// Конфигурирует <see cref="T:OpenTelemetry.Trace.TracerProviderBuilder" /> для настройки распределённого трейсинга.
    /// </summary>
    protected virtual void ConfigureTraceProviderBuilder(TracerProviderBuilder builder)
    {
        builder.SetResourceBuilder(ResourceBuilder.CreateEmpty().AddService(ServiceOptions.Name))
            .AddSource(ServiceOptions.Name).AddAspNetCoreInstrumentation(o =>
            {
                o.RecordException = true;
                o.Filter = TracingFilter;
            }).AddHttpClientInstrumentation(o => o.RecordException = true)
            .AddSqlClientInstrumentation(o => o.RecordException = true);
        if (Environment.IsDevelopment())
            builder.AddConsoleExporter();
        else
            builder.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(ServiceOptions.TracingSampleRate)));

        static bool TracingFilter(HttpContext context)
        {
            return !context.Request.Path.StartsWithSegments((PathString)"/internal") &&
                   !context.Request.Path.StartsWithSegments((PathString)"/swagger");
        }
    }

    /// <summary>
    /// Конфигурирование пайплайна запроса.
    /// </summary>
    public virtual void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IApiVersionDescriptionProvider provider)
    {
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ErrorWrapperMiddleware>();
        app.UseRouting();
        app.UseResponseCaching();
        if (ServiceOptions.UseCors)
            app.UseCors();
        if (ServiceOptions.UseAuthorization)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseAuthentication();
            app.UseAuthorization();
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/internal/health");
            EndpointRouteBuilderExtensions.MapGet(endpoints, "/internal/ping", (RequestDelegate)(async context =>
            {
                var headers = context.Response.Headers;
                headers.ContentType = (StringValues)"text/plain";
                headers.CacheControl = (StringValues)"no-cache, no-store, must-revalidate";
                headers.Pragma = (StringValues)"no-cache";
                headers.Expires = (StringValues)"0";
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync("pong");
            })).WithDisplayName("Ping");
        });
        app.UseSwagger(c =>
        {
            #if !DEBUG
            const string BasePath = "/ticket-api";
            c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{BasePath}" } };
            });
            #endif
        });
        app.UseSwaggerUI(c =>
        {
            foreach (var versionDescription in provider.ApiVersionDescriptions)
                c.SwaggerEndpoint(versionDescription.GroupName + "/swagger.yaml",
                    versionDescription.GroupName.ToUpperInvariant());
            c.DisplayRequestDuration();
        });
    }

    /// <summary>
    /// Регистрирует в <paramref name="services" /> и конфигурирует всё, что нужно для работы swagger'а.
    /// </summary>
    protected virtual void AddSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(x => x.FullName);
            c.EnableAnnotations();
            foreach (var filePath in GetXmlCommentsPath())
                c.IncludeXmlComments(filePath);
            c.AddSecurityDefinition("openIdConnect", new OpenApiSecurityScheme()
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
        });
        services.ConfigureOptions<ConfigureSwaggerOptions>();
    }

    /// <summary>
    /// Возвращает список файлов с документацией для Swagger'а
    /// </summary>
    protected virtual IEnumerable<string> GetXmlCommentsPath()
    {
        return new DirectoryInfo(AppContext.BaseDirectory)
            .EnumerateFiles("*.xml", SearchOption.TopDirectoryOnly)
            .Select((Func<FileInfo, string>)(_ => _.FullName));
    }
}