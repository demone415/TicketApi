using TicketApi.Entities;

namespace TicketApi.Interfaces.Services;

public interface IClassifierService
{
    public Task<TicketHeader> ClassifyTicketAsync(TicketHeader header, CancellationToken ct);
}