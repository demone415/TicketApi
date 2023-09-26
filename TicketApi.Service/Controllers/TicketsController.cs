using System.Net;
using Microsoft.AspNetCore.Mvc;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Models;
using TicketApi.Repositories;
using TicketApi.Services;

namespace TicketApi.Service.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class TicketsController : ControllerBase
{
    private readonly IClassifierService _classifierService;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketsController> _logger;
    
    public TicketsController(
        ITicketService ticketService,
        ITicketRepository ticketRepository,
        IClassifierService classifierService,
        ILogger<TicketsController> logger)
    {
        _classifierService = classifierService;
        _ticketRepository = ticketRepository;
        _ticketService = ticketService;
        _logger = logger;
    }

    /// <summary>
    /// Получить список чеков по номеру страницы. На каждой странице 1000 чеков.
    /// </summary>
    /// <param name="ct"></param>
    /// <param name="pageNum"></param>
    /// <returns></returns>
    [HttpGet("")]
    public async Task<IActionResult> GetTickets(CancellationToken ct, int pageNum = 0)
    {
        return Ok(await _ticketRepository.GetTicketsAsync(pageNum, ct));
    }
    
    /// <summary>
    /// Получить детальную информацию о чеке по QR коду
    /// </summary>
    /// <param name="qr"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("data")]
    public async Task<IActionResult> GetTicketData(string qr, CancellationToken ct)
    {
        var ticketDataResult = await _ticketService.GetTicketDataAsync(qr, ct);
        if (ticketDataResult.ResultCode == ResultCodes.CheckInvalid) 
            return BadRequest("Invalid QR code");
        return Ok(await _ticketService.GetTicketDataAsync(qr, ct));
    }
    
    /// <summary>
    /// Категоризировать позиции чека
    /// </summary>
    /// <param name="ticket"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("data/classify")]
    public async Task<IActionResult> ClassifyTicket(TicketHeader ticket, CancellationToken ct)
    {
        return Ok(await _classifierService.ClassifyTicketAsync(ticket, ct));
    }
    
    /// <summary>
    /// Сохранить данные чека в бд
    /// </summary>
    /// <param name="ticket"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<IActionResult> SaveTicket(TicketHeader ticket, CancellationToken ct)
    {
        return Ok(await _ticketRepository.SaveTicketAsync(ticket, ct));
    }
    
    /// <summary>
    /// Получить информацию по чеку, категоризировать позиции и сохранить в бд 
    /// </summary>
    /// <param name="qr"></param>
    /// <returns></returns>
    [HttpGet("auto")]
    public async Task<IActionResult> GetTicketDataAuto(string qr, CancellationToken ct)
    {
        return Ok(await _ticketService.ProcessQrAutoAsync(qr, ct));
    }
    
}