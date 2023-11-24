using Microsoft.EntityFrameworkCore;
using TicketApi.Entities;
using TicketApi.Extensions;
using TicketApi.Interfaces.Repositories;
using TicketApi.Models;
using TicketApi.Repositories.Base;
using TicketApi.Shared.SimplifySetting;

namespace TicketApi.Repositories;

public class TicketRepository : RepositoryBase<TicketHeader>, ITicketRepository, IRegisterRepository
{
    private const int PageSize = 1000;
    private readonly MainContext _mainContext;

    public TicketRepository(MainContext mainContext) : base(mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task<List<TicketHeader>> GetTicketsAsync(int pageNum, CancellationToken ct)
    {
        return await CompiledQueries.GetTicketsAsync(_mainContext, pageNum, PageSize).ToListAsync(ct);
    }

    public async Task<bool> SaveTicketAsync(TicketHeader header, CancellationToken ct)
    {
        await AddAsync(header, ct);
        await SaveAsync(ct);
        //await _mainContext.Headers.AddRangeAsync(header);
        //await _mainContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<TopCategories> GetTopCategoriesAsync(CancellationToken ct)
    {
        var c1 = await CompiledQueries.GetTopCategoriesAsync(_mainContext, l => l.Category1).ToListAsync(ct);
        var c2 = await CompiledQueries.GetTopCategoriesAsync(_mainContext, l => l.Category2).ToListAsync(ct);
        var c3 = await CompiledQueries.GetTopCategoriesAsync(_mainContext, l => l.Category3).ToListAsync(ct);

        var ret = new TopCategories
        {
            Categories1 = c1,
            Categories2 = c2,
            Categories3 = c3
        };

        return ret;
    }
}