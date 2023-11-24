using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TicketApi.Entities;
using TicketApi.Repositories.Base;
using TicketApi.Shared.SimplifySetting;

namespace TicketApi;

// https://metanit.com/sharp/efcore/
public class MainContext : ContextBase<MainContext>, IContextRegistration
{
    public DbSet<TicketLine> Lines => Set<TicketLine>();
    public DbSet<TicketHeader> Headers => Set<TicketHeader>();

    public MainContext(DbContextOptions<MainContext> options, IConfiguration configuration) : base(options,
        configuration)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketHeader>().HasKey(e => e.Id);
        modelBuilder.Entity<TicketHeader>().HasMany(e => e.Lines);

        modelBuilder.Entity<TicketLine>().HasKey(e => e.Id);
    }
}