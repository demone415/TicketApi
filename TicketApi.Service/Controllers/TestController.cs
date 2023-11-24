using Microsoft.AspNetCore.Mvc;
using TicketApi.Interfaces.Services;

namespace TicketApi.Service.Controllers;

#if DEBUG

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IRedisService _redisService;

    public TestController(IRedisService redisService)
    {
        _redisService = redisService;
    }

    [HttpGet("currentRequestCount")]
    public async Task<IActionResult> GetCurrentRequestCount(DateTime? date)
    {
        date ??= DateTime.Today;
        var currentScore = await _redisService.GetCurrentRequestCountAsync(date.Value);
        return Ok(currentScore);
    }

    [HttpGet("increaseRequestCount")]
    public async Task<IActionResult> IncreaseRequestCountForDate(DateTime? date)
    {
        date ??= DateTime.Today;
        var newScore = await _redisService.IncreaseRequestCountAsync(date.Value);
        return Ok(newScore);
    }

    [HttpGet("canMakeRequest")]
    public async Task<IActionResult> CanMakeRequestForDate(DateTime? date)
    {
        date ??= DateTime.Today;
        var can = await _redisService.CanMakeRequestAsync(date.Value);
        return Ok(can);
    }
}

#endif