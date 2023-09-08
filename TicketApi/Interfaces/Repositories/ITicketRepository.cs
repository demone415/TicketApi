using TicketApi.Entities;
using TicketApi.Models;

namespace TicketApi.Repositories;

public interface ITicketRepository
{
    public Task<List<TicketHeader>> GetTicketsAsync(int pageNum, CancellationToken ct);

    public Task<bool> SaveTicketAsync(TicketHeader ticket, CancellationToken ct);

    public Task<TopCategories> GetTopCategoriesAsync(CancellationToken ct);
}