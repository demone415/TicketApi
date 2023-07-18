using AutoFixture;
using Moq;
using StackExchange.Redis;
using TicketApi.Interfaces.Services;
using TicketApi.Services;
using Xunit.Sdk;

namespace TicketApi.Tests;

public class RedisTests
{
    private const string ProverkaKeyName = "ProverkaCheckaRequests";
    private const string DateFormat = "yyyy-MM-dd";
    private const int MaxRequestsPerDay = 15;

    private readonly Mock<IRedisService> _mock;
    private readonly Mock<IDatabase> _db;
    private readonly Fixture _fixture;
    
    public RedisTests()
    {
        _mock = new Mock<IRedisService>();
        _db = new Mock<IDatabase>();
        _fixture = new Fixture();
        _fixture.Inject(_db.Object);
    }

    [Fact]
    public void HashSet_Works()
    {
        var key = new RedisKey("somekey");
        var field = new RedisValue("somefield");
        var value = new RedisValue("somevalue");
        
        _db.Setup(x => x.HashSet(key, field, value, When.Always, CommandFlags.None)).Returns(false);

        var result = _db.Object.HashSet(key, field, value, When.Always, CommandFlags.None);
        
        Assert.True(result);
    }
    
    [Fact]
    public void IncreaseRequestCountAsync_Works()
    {
        var dt = DateTime.Now;
        
        _mock.Setup(r => r.IncreaseRequestCountAsync(dt)).Returns(Task.FromResult(1));

        var service = _mock.Object;
        var result = service.IncreaseRequestCountAsync(dt);
        
        _mock.Verify(r => r.IncreaseRequestCountAsync(dt), Times.Exactly(1));
        Assert.Same(Task.CompletedTask, result);
    }
}