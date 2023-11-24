using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using TicketApi.Shared.Converters;

namespace TicketApi.Shared;

/// <summary>
/// Дефолтные значения
/// </summary>
public static class Default
{
    /// <summary>Дефолтные настройки сериализации System.Text.Json</summary>
    public static readonly JsonSerializerOptions JsonOptions;

    static Default()
    {
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.PropertyNameCaseInsensitive = true;
        serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        serializerOptions.Converters.Add((JsonConverter)new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        serializerOptions.Converters.Add((JsonConverter)new DateTimeOffsetConverter());
        serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        JsonOptions = serializerOptions;
    }
}