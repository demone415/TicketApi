namespace TicketApi.Interfaces.Services;

public interface IRedisService
{
    public Task<int> IncreaseRequestCountAsync(DateTime key);

    public Task<bool> CanMakeRequestAsync(DateTime key);
}