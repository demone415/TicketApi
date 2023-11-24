/*using Serilog;
using TicketApi.Interfaces.Services;
using TicketApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging(lb => lb.ClearProviders().AddSerilog());
builder.Services.AddScoped<ICategorizationService, CategorizationService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();*/

using Serilog;
using TicketApi.Categorizer;

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