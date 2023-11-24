using TicketApi.Entities;
using TicketApi.Models;

namespace TicketApi.Interfaces.Services;

public interface ITicketService
{
    public Task<TicketDataResult> GetTicketDataAsync(string qrdata, CancellationToken ct);

    public Task<TicketDataResult> GetTicketDataAsync(TicketHeader header, CancellationToken ct);

    public Task<AutoTicketResult> ProcessQrAutoAsync(string qrdata, CancellationToken ct);

    public Task<TopCategories> GetTopCategoriesAsync(CancellationToken ct);
}