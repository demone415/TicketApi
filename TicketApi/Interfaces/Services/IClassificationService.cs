using TicketApi.Entities;

namespace TicketApi.Interfaces.Services;

public interface IClassificationService
{
    public Task<TicketHeader> Classify(TicketHeader header);
}