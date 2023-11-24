using Newtonsoft.Json;

namespace TicketApi.Models.Requests;

public sealed record QrStringCheckRequest : CheckRequestBase
{
    /// <summary>
    /// Строка, считанная из QR-кода чека
    /// </summary>
    [JsonProperty("qrraw")]
    public string QrString { get; set; }

    public QrStringCheckRequest(string qrString, string token)
    {
        QrString = qrString;
        Token = token;
    }
}