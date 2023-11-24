using Newtonsoft.Json;

namespace TicketApi.Models.Requests;

public sealed record QrDataCheckRequest : CheckRequestBase
{
    /// <summary>
    /// Номер фискального накопителя
    /// </summary>
    [JsonProperty("fn")]
    public string FiscalNumber { get; init; }

    /// <summary>
    /// Номер фискального документа
    /// </summary>
    [JsonProperty("fd")]
    public string FiscalDocument { get; init; }

    /// <summary>
    /// Фискальный признак документа
    /// </summary>
    [JsonProperty("fp")]
    public string FiscalSign { get; init; }

    /// <summary>
    /// Дата и время
    /// </summary>
    /// <remarks>Формат 'yyyyMMddTHHmm'</remarks>
    [JsonProperty("t")]
    public string Date { get; init; }

    /// <summary>
    /// Сумма чека
    /// </summary>
    [JsonProperty("s")]
    public decimal Sum { get; init; }

    /// <summary>
    /// Тип операции
    /// </summary>
    [JsonProperty("n")]
    public OperationType OperationType { get; init; }

    /// <summary>
    /// Признак сканирования qr кода (0/1)
    /// </summary>
    [JsonProperty("qr")]
    public int QrFlag { get; init; } = 1;

    public QrDataCheckRequest(QrData qrData, string token)
    {
        Token = token;
        FiscalNumber = qrData.FiscalNumber;
        FiscalDocument = qrData.FiscalDocument;
        FiscalSign = qrData.FiscalSign;
        Date = qrData.Date.ToString("yyyyMMddTHHmm");
        Sum = qrData.Sum;
        OperationType = qrData.OperationType;
        QrFlag = 1;
    }
}