using TicketApi.Models;

namespace TicketApi.Interfaces.Services;

public interface IProverkaCheckaService
{
    public Task<CheckResult> GetTicketDataFromQrString(string qrString, CancellationToken ct);

    public Task<CheckResult> GetTicketDataFromQrData(QrData data, CancellationToken ct);

    public Task<CheckResult> GetTicketDataFromQrUrl(string qrUrl, CancellationToken ct);
}