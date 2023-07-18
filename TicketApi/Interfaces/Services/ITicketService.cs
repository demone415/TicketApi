using TicketApi.Entities;
using TicketApi.Models;

namespace TicketApi.Interfaces.Services;

public interface ITicketService
{
    public Task<TicketDataResult> GetTicketData(string qrdata);
    public Task<TicketDataResult> GetTicketData(TicketHeader header);

    public Task<TicketHeader> ClassifyTicket(TicketHeader lines);

    public Task<AutoTicketResult> ProcessQrAuto(string qrdata);
}