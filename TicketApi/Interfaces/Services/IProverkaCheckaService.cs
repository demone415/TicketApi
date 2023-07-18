using TicketApi.Models;

namespace TicketApi.Interfaces.Services;

public interface IProverkaCheckaService
{
    public Task<CheckResult> GetTicketDataFromQrString(string qrString);

    public Task<CheckResult> GetTicketDataFromQrData(QrData data);

    public Task<CheckResult> GetTicketDataFromQrUrl(string qrUrl);
}