namespace TicketApi.Models;

public record struct QrData
{
    public string FiscalNumber { get; init; }
    public string FiscalDocument { get; init;}
    public string FiscalSign { get; init;}
    public DateTime Date { get; init;}
    public decimal Sum { get; init;}
    public OperationType OperationType { get; init; }
}