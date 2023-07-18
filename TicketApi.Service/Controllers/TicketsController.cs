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
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketsController> _logger;
    
    public TicketsController(ITicketService ticketService, ITicketRepository ticketRepository, ILogger<TicketsController> logger)
    {
        _ticketService = ticketService;
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    /// <summary>
    /// Получить список чеков по номеру страницы. На каждой странице 1000 чеков.
    /// </summary>
    /// <param name="pageNum"></param>
    /// <returns></returns>
    [HttpGet("")]
    public async Task<IActionResult> GetTickets(int pageNum = 0)
    {
        return Ok(await _ticketRepository.GetTickets(pageNum));
    }
    
    /// <summary>
    /// Получить детальную информацию о чеке по QR коду
    /// </summary>
    /// <param name="qr"></param>
    /// <returns></returns>
    [HttpGet("data")]
    public async Task<IActionResult> GetTicketData(string qr)
    {
        var ticketDataResult = await _ticketService.GetTicketData(qr);
        if (ticketDataResult.ResultCode == ResultCodes.CheckInvalid) 
            return BadRequest("Invalid QR code");
        return Ok(await _ticketService.GetTicketData(qr));
    }
    
    /// <summary>
    /// Категоризировать позиции чека
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    [HttpPost("data/classify")]
    public async Task<IActionResult> ClassifyTicket(TicketHeader ticket)
    {
        return Ok(await _ticketService.ClassifyTicket(ticket));
    }
    
    /// <summary>
    /// Сохранить данные чека в бд
    /// </summary>
    /// <param name="ticket"></param>
    /// <returns></returns>
    [HttpPost("")]
    public async Task<IActionResult> SaveTicket(TicketHeader ticket)
    {
        return Ok(await _ticketRepository.SaveTicket(ticket));
    }
    
    /// <summary>
    /// Получить информацию по чеку, категоризировать позиции и сохранить в бд 
    /// </summary>
    /// <param name="qr"></param>
    /// <returns></returns>
    [HttpGet("auto")]
    public async Task<IActionResult> GetTicketDataAuto(string qr)
    {
        return Ok(await _ticketService.ProcessQrAuto(qr));
    }
    
}