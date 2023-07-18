using Newtonsoft.Json;
using TicketApi.Models;

namespace TicketApi.Tests;

public class ParsingTest
{
    [Fact]
    public void ProverkaResponseParsing_NotThrows()
    {
        var testqr1 = File.ReadAllText("testqr1.json");
        var checkResult = JsonConvert.DeserializeObject<CheckResult>(testqr1);
        checkResult.Should().NotBeNull(string.Empty);
        var data = JsonConvert.DeserializeObject<Check>(checkResult!.Data.ToString());
        data.Should().NotBeNull(string.Empty);
    }
}