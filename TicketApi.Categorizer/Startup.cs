using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Serilog;
using TicketApi.Interfaces.Services;
using TicketApi.Services;
using TicketApi.Shared.Extensions;
using TicketApi.Shared.Swagger;
using StartupBase = TicketApi.Shared.Configuration.StartupBase;

namespace TicketApi.Categorizer;

/// <inheritdoc />
[UsedImplicitly]
public sealed class Startup : StartupBase
{
    /// <inheritdoc />
    public Startup(IWebHostEnvironment env, IConfiguration configuration) : base(env, configuration)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.Configure((Action<JsonOptions>)(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        }));

        services.AddMemoryCache();
        services.AddResponseCompression(options => { options.EnableForHttps = true; });

        services.AddLogging(lb => lb.ClearProviders().AddSerilog());

        services.AddOptions();
        services.AddControllers();

        services.AddScoped<ICategorizationService, CategorizationService>();

        services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<SwaggerIgnoreFilter>();
            options.OperationFilter<IgnorePropertyFilter>();
        });

        services.SetupNewtonSoft();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment environment,
        IApiVersionDescriptionProvider provider)
    {
        base.Configure(app, environment, provider);
        app.UseSerilogRequestLogging();
        app.UseResponseCompression();
    }
}