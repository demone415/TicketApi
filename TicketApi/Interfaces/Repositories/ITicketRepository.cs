using TicketApi.Entities;
using TicketApi.Models;

namespace TicketApi.Interfaces.Repositories;

public interface ITicketRepository
{
    public IAsyncEnumerable<TicketHeader> GetTicketsAsync(int pageNum);

    public Task<bool> SaveTicketAsync(TicketHeader ticket, CancellationToken ct);

    public Task<TopCategories> GetTopCategoriesAsync(CancellationToken ct);
}