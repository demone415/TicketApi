using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using TicketApi;
using TicketApi.Interfaces.Services;
using TicketApi.Service.Workers;
using TicketApi.Shared.Configuration;
using TicketApi.Shared.Extensions;
using TicketApi.Shared.SimplifySetting;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var configuration = builder.Configuration;
var services = builder.Services;
var environment = builder.Environment;

configuration.AddJsonFile("appsettings.json", false, true);

// Считываем конфиги для пространства
var aspEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (aspEnv != null)
{
    var environmentFile =
        $"appsettings.{aspEnv}.json";
    Console.WriteLine($"Current ENVIRONMENT: {aspEnv}");
    configuration.AddJsonFile(environmentFile, true, true);
}

configuration.AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

services.AddLogging(lb => lb.ClearProviders().AddSerilog());
services.AddOptions();
var serviceOptions = new ServiceOptions();
configuration.GetSection("Service").Bind(serviceOptions);
services.Configure<ServiceOptions>(configuration.GetSection("Service"));
services.AddHttpContextAccessor();
services.AddMemoryCache();

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme()
    {
        Description =
            "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

services.Configure((Action<JsonOptions>)(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}));

services.RegisterContext(typeof(IContextRegistration), configuration);
services.AddResponseCompression();
var redisConf = new RedisConfiguration
{
    ConnectionString = configuration.GetConnectionString("Redis")
};

services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConf);

services.AddRefitServices(configuration);

services.AddIdentity(configuration, environment);

services.AddDbContext<MainContext>();

//services.AddJobs(configuration);

services.RegisterByInterface(typeof(IScopeRegistration));

services.AddRepositories(configuration);

services.SetupNewtonsoft();

services.AddControllers();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseResponseCaching();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.UseSwagger(options =>
{
#if !DEBUG
    const string BasePath = "/ticket-api";
    options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
    {
        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"https://{httpReq.Host.Value}{BasePath}" } };
    });
#endif
});
app.MapSwagger();
app.UseSwaggerUI(options =>
{
    options.DisplayRequestDuration();
});

app.Run();

internal static class ServiceCollectionExtensions
{
    internal static void AddRefitServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProverkaCheckaServiceClient(new Uri(configuration.GetSection("ProverkaChecka")["Host"]!));
    }

    internal static void AddIdentity(
        this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var section = configuration.GetSection("Jwt");
                options.Authority = section["Authority"]!;
                options.Audience = section["Audience"]!;
                options.TokenValidationParameters.ValidIssuer = section["Authority"]!;
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.IssuerSigningKey = GetSigningKey(section);
                
                //options.TokenValidationParameters.RoleClaimType = "role";
                //options.TokenValidationParameters.NameClaimType = "name";
                options.RequireHttpsMetadata = environment.IsProduction();
    
                if (!environment.IsProduction())
                {
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (message, certificate, chain, sslPolicyErrors) => true
                    };
                }
            });
        services
            .AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build());
    }

    private static RsaSecurityKey GetSigningKey(IConfiguration section)
    {
        var rsa = RSA.Create();
        rsa.KeySize = 4096;
        rsa.ImportFromPem(section["Rsa"]!);
        var param = rsa.ExportParameters(false);
        return new RsaSecurityKey(param);
    }

    internal static void AddJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartzHostedService(opts =>
        {
            opts.WaitForJobsToComplete = true;
            opts.AwaitApplicationStarted = true;
        });
        
        services.AddQuartz(conf =>
        {
            conf.AddJob<TicketReFetcher>(jobConf =>
            {
                jobConf.WithIdentity(nameof(TicketReFetcher));
                jobConf.DisallowConcurrentExecution();
            });
            conf.AddTrigger(triggerConf =>
            {
                triggerConf.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0, 0));
                triggerConf.ForJob(nameof(TicketReFetcher));
                triggerConf.StartNow();
            });
            //conf.UseMicrosoftDependencyInjectionJobFactory();
        });
    }
}