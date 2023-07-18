using Microsoft.AspNetCore.Mvc;
using TicketApi.Interfaces.Services;
using TicketApi.Services;

namespace TicketApi.Service.Controllers;

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

    [HttpGet("increaseRequestCount")]
    public async Task<IActionResult> IncreaseRequestCountForDate(DateTime date)
    {
        var newScore = await _redisService.IncreaseRequestCountAsync(date);
        return Ok(newScore);
    }

    [HttpGet("canMakeRequest")]
    public async Task<IActionResult> CanMakeRequestInDate(DateTime date)
    {
        var can = await _redisService.CanMakeRequestAsync(date);
        return Ok(can);
    }
}