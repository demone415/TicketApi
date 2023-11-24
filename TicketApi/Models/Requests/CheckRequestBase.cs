using Newtonsoft.Json;

namespace TicketApi.Models.Requests;

public record CheckRequestBase
{
    /// <summary>
    /// Токен доступа
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; init; }
}