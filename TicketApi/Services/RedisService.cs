using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using TicketApi.Interfaces.Services;

namespace TicketApi.Services;

public class RedisService : IRedisService
{
    private readonly IRedisDatabase _redisDb;

    private const string ProverkaKeyName = "ProverkaCheckaRequests";
    private const string DateFormat = "yyyy-MM-dd";
    private const int MaxRequestsPerDay = 15;

    private readonly RedisKey _proverkaKey = new (ProverkaKeyName);
    
    public RedisService(IRedisDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    public async Task<int> IncreaseRequestCountAsync(DateTime dt)
    {
        var key = new RedisKey(dt.ToString(DateFormat));
        var score = await _redisDb.Database.StringGetAsync(key);
        if (score.HasValue)
        {
            score.TryParse(out int val);
            await _redisDb.Database.StringIncrementAsync(key);
            return ++val;
        }

        await _redisDb.Database.StringSetAsync(key, 1, TimeSpan.FromDays(30));
        return 1;
    }

    public async Task<bool> CanMakeRequestAsync(DateTime dt)
    {
        var key = new RedisKey(dt.ToString(DateFormat));
        var score = await _redisDb.Database.StringGetAsync(key);
        if (!score.HasValue) return true;
        score.TryParse(out int val);
        return val < MaxRequestsPerDay;
    }
}