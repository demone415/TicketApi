using Microsoft.EntityFrameworkCore;
using TicketApi.Entities;
using TicketApi.Extensions;
using TicketApi.Models;

namespace TicketApi.Repositories;

public class TicketRepository : ITicketRepository
{
    private const int PageSize = 1000;
    private readonly PostgresContext _postgresContext;

    public TicketRepository(PostgresContext postgresContext)
    {
        _postgresContext = postgresContext;
    }

    public async Task<List<TicketHeader>> GetTicketsAsync(int pageNum, CancellationToken ct)
    {
        return await CompiledQueries.GetTicketsAsync(_postgresContext, pageNum, PageSize).ToListAsync(ct);
    }

    public async Task<bool> SaveTicketAsync(TicketHeader header, CancellationToken ct)
    {
        await _postgresContext.Headers.AddRangeAsync(header);
        await _postgresContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<TopCategories> GetTopCategoriesAsync(CancellationToken ct)
    {
        var c1 = await CompiledQueries.GetTopCategoriesAsync(_postgresContext, l => l.Category1).ToListAsync(ct);
        var c2 = await CompiledQueries.GetTopCategoriesAsync(_postgresContext, l => l.Category2).ToListAsync(ct);
        var c3 = await CompiledQueries.GetTopCategoriesAsync(_postgresContext, l => l.Category3).ToListAsync(ct);

        var ret = new TopCategories
        {
            Categories1 = c1,
            Categories2 = c2,
            Categories3 = c3,
        };
        
        return ret;
    }
}