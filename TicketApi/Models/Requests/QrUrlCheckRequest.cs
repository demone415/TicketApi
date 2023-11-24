using Newtonsoft.Json;

namespace TicketApi.Models.Requests;

public sealed record QrUrlCheckRequest : CheckRequestBase
{
    /// <summary>
    /// Ссылка на QR-код чека
    /// </summary>
    [JsonProperty("qrurl")]
    public string QrUrl { get; set; }

    public QrUrlCheckRequest(string qrUrl, string token)
    {
        QrUrl = qrUrl;
        Token = token;
    }
}