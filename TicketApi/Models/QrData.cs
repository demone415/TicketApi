using Refit;
using TicketApi.Entities;

namespace TicketApi.Models;

public record struct QrData
{
    public string FiscalNumber { get; init; }

    public string FiscalDocument { get; init; }

    public string FiscalSign { get; init; }

    public DateTime Date { get; init; }

    public decimal Sum { get; init; }

    public OperationType OperationType { get; init; }

    
    public QrData(TicketHeader header)
    {
        FiscalNumber = header.FsId;
        FiscalSign = header.FicsalSign;
        FiscalDocument = header.FicsalDoc;
        Date = header.Date.Date;
        Sum = header.TicketSum;
        OperationType = header.OperationType;
    }

    public override string ToString()
    {
        return $"{FiscalNumber}{FiscalSign}{FiscalDocument}{Date:s}{Sum}{OperationType}";
    }
}