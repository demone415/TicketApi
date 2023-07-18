using TicketApi.Entities;
using TicketApi.Models;

namespace TicketApi.Models;

public class TicketDataResult
{
    public ResultCodes ResultCode { get; set; }
    public TicketHeader Header { get; set; }
}