using Serilog;
using TicketApi.Service;

try
{
    CreateHostBuilder(args).Build().Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

return;

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(ConfigConfiguration)
        .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
        .UseSerilog();
}

static void ConfigConfiguration(
    HostBuilderContext builderContext,
    IConfigurationBuilder configurationBuilder)
{
    configurationBuilder.AddJsonFile("appsettings.json", false, true);

    // Считываем конфиги для пространства
    var aspEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    if (aspEnv != null)
    {
        var environment =
            $"appsettings.{aspEnv}.json";
        Console.WriteLine($"Current ENVIRONMENT: {environment}");
        configurationBuilder.AddJsonFile(environment, true, true);
    }

    configurationBuilder.AddEnvironmentVariables();
}