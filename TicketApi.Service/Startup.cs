using System.Text;
using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Service.Workers;
using TicketApi.Shared.Configuration;
using TicketApi.Shared.Extensions;
using TicketApi.Shared.SimplifySetting;
using TicketApi.Shared.Swagger;
using StartupBase = TicketApi.Shared.Configuration.StartupBase;

namespace TicketApi.Service;

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

        services.RegisterContext(typeof(IContextRegistration), Configuration);

        services.AddMemoryCache();
        services.AddResponseCompression(options => { options.EnableForHttps = true; });

        services.AddLogging(lb => lb.ClearProviders().AddSerilog());

        var redisConf = new RedisConfiguration
        {
            ConnectionString = Configuration.GetConnectionString("Redis")
        };

        services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConf);

        services.AddOptions();
        services.AddControllers();

        #region refit

        services.AddProverkaCheckaServiceClient(new Uri(Configuration.GetSection("ProverkaChecka")["Host"]!));

        #endregion

        services.AddDbContext<MainContext>();

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
            conf.UseMicrosoftDependencyInjectionJobFactory();
        });

        services.RegisterByInterface(typeof(IScopeRegistration));

        services.RegisterAllRepository(Configuration);

        services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<SwaggerIgnoreFilter>();
            options.OperationFilter<IgnorePropertyFilter>();
        });

        services.SetupNewtonSoft();

#pragma warning disable ASP0000
        services.AddSingleton<IServiceProvider>(services.BuildServiceProvider());
#pragma warning restore ASP0000
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding.GetEncoding("windows-1252");
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment environment,
        IApiVersionDescriptionProvider provider)
    {
        base.Configure(app, environment, provider);
        app.UseSerilogRequestLogging();
        app.UseResponseCompression();
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var main = serviceScope.ServiceProvider.GetService<MainContext>();
        main?.Database.Migrate();
    }
}