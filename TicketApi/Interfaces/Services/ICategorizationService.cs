using TicketApi.Entities;

namespace TicketApi.Interfaces.Services;

public interface ICategorizationService
{
    public Task<TicketHeader> CategorizeTicketAsync(TicketHeader header, CancellationToken ct);
}