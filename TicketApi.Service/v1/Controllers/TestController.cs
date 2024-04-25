using Microsoft.AspNetCore.Mvc;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Models;

namespace TicketApi.Service.v1.Controllers;

#if DEBUG

[ApiController]
[ApiVersion("1.0")]
[Produces("application/json")]
[Route("v1/test")]
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

    [HttpPost("saveTicketToCache")]
    public async Task<IActionResult> SaveTicketToCache([FromBody] TicketHeader header)
    {
        await _redisService.SaveTicketAsync(header);
        return Ok();
    }

    [HttpGet("getTicketFromCache")]
    public async Task<IActionResult> GetTicketFromCache(QrData data)
    {
        var header = await _redisService.GetTicketAsync(data);
        return Ok(header);
    }
}

#endif