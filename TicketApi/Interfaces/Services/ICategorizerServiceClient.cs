using TicketApi.Entities;

namespace TicketApi.Interfaces.Services;

public interface ICategorizerServiceClient
{
    public Task<TicketHeader> CategorizeTicketAsync(TicketHeader header, CancellationToken ct);
}