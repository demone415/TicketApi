using Microsoft.EntityFrameworkCore;
using TicketApi.Models.ConnectionStrings;

namespace TicketApi.Entities;

// https://metanit.com/sharp/efcore/
public class PostgresContext : DbContext
{
    public DbSet<TicketLine> Lines => Set<TicketLine>();
    public DbSet<TicketHeader> Headers => Set<TicketHeader>();
    
    public PostgresContext()
    {
        //Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConnectionStrings.Postgres.Value!);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketHeader>().HasKey(e => e.Id);
        modelBuilder.Entity<TicketHeader>().HasMany(e => e.Lines);
        
        modelBuilder.Entity<TicketLine>().HasKey(e => e.Id);
    }
}