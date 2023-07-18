using Microsoft.EntityFrameworkCore;
using TicketApi.Entities;

namespace TicketApi.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly PostgresContext _postgresContext;

    private const int PageSize = 1000;
    
    public TicketRepository(PostgresContext postgresContext)
    {
        _postgresContext = postgresContext;
    }
    
    public async Task<List<TicketHeader>> GetTickets(int pageNum)
    {
        return await _postgresContext.Headers.AsNoTracking()
            .Include(header => header.Lines)
            .OrderByDescending(x => x.Id)
            .Skip(pageNum * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }
    
    public async Task<bool> SaveTicket(TicketHeader header)
    {
        await _postgresContext.Headers.AddRangeAsync(header);
        await _postgresContext.SaveChangesAsync();
        return true;
    }
}