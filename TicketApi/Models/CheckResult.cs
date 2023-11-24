using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TicketApi.Models;

public sealed class CheckResult
{
    [JsonProperty("code")]
    public ResultCodes Code { get; set; }

    [JsonProperty("first")]
    public long First { get; set; }

    [JsonProperty("data")]
    public JToken Data { get; set; }

    public CheckResult()
    {
    }

    public CheckResult(ResultCodes code)
    {
        Code = code;
    }
}