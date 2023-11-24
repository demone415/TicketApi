using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TicketApi.Shared.Converters;

/// <summary>
/// Более толерантный конвертер <see cref="T:System.DateTimeOffset" /> для совместимости с Newtonsoft.Json.
/// </summary>
public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    private static readonly CultureInfo CI = CultureInfo.GetCultureInfo("ru-RU");

    /// <inheritdoc />
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        DateTimeOffset dateTimeOffset;
        if (!reader.TryGetDateTimeOffset(out dateTimeOffset))
            dateTimeOffset = DateTimeOffset.Parse(reader.GetString(), (IFormatProvider)CI);
        return dateTimeOffset;
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        DateTimeOffset value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(
            value.ToString("yyyy-MM-ddTHH:mm:sszzzz", (IFormatProvider)CultureInfo.InvariantCulture));
    }
}