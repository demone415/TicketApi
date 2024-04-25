using JetBrains.Annotations;
using TicketApi.Entities;
using TicketApi.Models;

namespace TicketApi.Interfaces.Services;

public interface IRedisService
{
    public Task<int> IncreaseRequestCountAsync(DateTime dt);

    public Task<bool> CanMakeRequestAsync(DateTime dt);

    public Task<string> GetCurrentRequestCountAsync(DateTime dt);

    public Task SaveTicketAsync([NotNull] TicketHeader header);

    public Task<TicketHeader> GetTicketAsync(QrData data);
}