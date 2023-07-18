using Newtonsoft.Json;
using Quartz;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Models.ConnectionStrings;
using TicketApi.Repositories;
using TicketApi.Service.Services;
using TicketApi.Service.Workers;
using TicketApi.Services;

var builder = WebApplication.CreateBuilder(args);

FillConnectionStrings();

var redisConfigurations = new[]
{
    new RedisConfiguration
    {
        AbortOnConnectFail = true,
        Hosts = new RedisHost[] { ConnectionStrings.Redis },
        AllowAdmin = true,
        ConnectTimeout = 5000,
        Database = 0,
        PoolSize = 5,
        IsDefault = true
    }
};

builder.Services.AddControllers();
builder.Services.AddLogging();
builder.Services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(redisConfigurations);
builder.Services.AddDbContext<PostgresContext>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IClassifierService, ClassifierService>();
builder.Services.AddScoped<IProverkaCheckaService, ProverkaCheckaService>();
builder.Services.AddScoped<IRedisService, RedisService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddQuartzHostedService(opts =>
{
    opts.WaitForJobsToComplete = true;
    opts.AwaitApplicationStarted = true;
});
builder.Services.AddQuartz(conf =>
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

app.Run();

void FillConnectionStrings()
{
    ConnectionStrings.Redis = JsonConvert.DeserializeObject<RedisConnectionModel>(Environment.GetEnvironmentVariable("CRED_REDIS")!);
    ConnectionStrings.Postgres = new PostgresConnectionString(Environment.GetEnvironmentVariable("POSTGRES_HOMEAPI"));

}