using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace TicketApi.Repositories.Base;

public abstract class ContextBase<T> : DbContext where T : DbContext
{
    private readonly DbContextOptions<T> _options;

    private string _connectionString;

    protected ContextBase(DbContextOptions<T> options, IConfiguration configuration) : base(options)
    {
        _options = options;
        var connectionString = configuration
            .GetConnectionString(nameof(T));
        _connectionString = connectionString;
    }

    public T GetNewInstance(bool readOnly = false)
    {
        var resultDb = (T)Activator.CreateInstance(typeof(T), _options);

        if (readOnly)
        {
            resultDb.Database.GetDbConnection().ConnectionString = _connectionString;
            resultDb.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            resultDb.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        return resultDb;
    }

    public void RegistrationContext(IServiceCollection collection, IConfiguration configuration)
    {
        var connectionString = configuration
            .GetConnectionString(typeof(T).Name);
        _connectionString = connectionString;
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.UseJsonNet();
        var dataSource = dataSourceBuilder.Build();
        collection.AddDbContext<T>(_ =>
            {
                _.UseNpgsql(dataSource, _ =>
                {
                    _.MigrationsAssembly("TicketApi");
                    _.EnableRetryOnFailure(3);
                    _.CommandTimeout(60);
                    _.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
                _.EnableDetailedErrors();
            }
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityColumns();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(T).Assembly);
    }
}