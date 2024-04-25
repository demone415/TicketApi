using Serilog;
using TicketApi.Interfaces.Services;
using TicketApi.Services;
using TicketApi.Shared.Configuration;
using TicketApi.Shared.Extensions;

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

services.AddResponseCompression();

services.AddScoped<ICategorizationService, CategorizationService>();

services.SetupNewtonsoft();

services.AddControllers();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseResponseCaching();
app.MapControllers();

app.Run();






























