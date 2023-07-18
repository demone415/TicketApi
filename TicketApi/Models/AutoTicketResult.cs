using TicketApi.Models;

namespace TicketApi.Models;

public class AutoTicketResult
{
    public ResultCodes ResultCode { get; set; }
    public string Comment { get; set; }
    public bool SaveSuccessful { get; set; }
}