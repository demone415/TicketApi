using TicketApi.Entities;

namespace TicketApi.Repositories;

public interface ITicketRepository
{
    public Task<List<TicketHeader>> GetTickets(int pageNum);
    
    public Task<bool> SaveTicket(TicketHeader ticket);
}