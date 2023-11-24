﻿using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TicketApi.Entities;
using TicketApi.Interfaces.Repositories;
using TicketApi.Interfaces.Services;
using TicketApi.Models;
using TicketApi.Repositories;
using TicketApi.Services;

namespace TicketApi.Service.Controllers;

[ApiController]
//[ApiVersion("1")]
[Produces("application/json")]
//[Route("v{version:apiVersion}/[controller]")]
[Route("[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ICategorizerServiceClient _categorizerServiceClient;
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketService _ticketService;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(
        ITicketService ticketService,
        ITicketRepository ticketRepository,
        ICategorizerServiceClient categorizerServiceClient,
        ILogger<TicketsController> logger)
    {
        _categorizerServiceClient = categorizerServiceClient;
        _ticketRepository = ticketRepository;
        _ticketService = ticketService;
        _logger = logger;
    }

    /// <summary>
    /// Получить список чеков по номеру страницы. На каждой странице 1000 чеков.
    /// </summary>
    /// <param name="pageNum">Номер страницы</param>
    /// <param name="ct"></param>
    /// <returns>Набор чеков</returns>
    [HttpGet("")]
    [ProducesResponseType(statusCode: 200, type: typeof(IList<TicketHeader>))]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IList<TicketHeader>>> GetTickets(CancellationToken ct, int pageNum = 0)
    {
        var tickets = await _ticketRepository.GetTicketsAsync(pageNum, ct);
        if (tickets == null || tickets.Count == 0)
            return NoContent();
        return Ok(tickets);
    }

    /// <summary>
    /// Получить детальную информацию о чеке по QR коду
    /// </summary>
    /// <param name="qr">Содержимое QR-кода 54-фз</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet("data")]
    [ProducesResponseType(statusCode: 200, type: typeof(TicketDataResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<TicketDataResult>> GetTicketData(string qr, CancellationToken ct)
    {
        var ticketDataResult = await _ticketService.GetTicketDataAsync(qr, ct);
        if (ticketDataResult.ResultCode == ResultCodes.CheckInvalid)
            return BadRequest("Invalid QR code");
        var ticketData = await _ticketService.GetTicketDataAsync(qr, ct);
        return Ok(ticketData);
    }

    /// <summary>
    /// Категоризировать позиции чека
    /// </summary>
    /// <param name="ticket">Чек</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPost("data/categorize")]
    [ProducesResponseType(statusCode: 200, type: typeof(TicketHeader))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<TicketHeader>> ClassifyTicket(TicketHeader ticket, CancellationToken ct)
    {
        var classifiedTicket = await _categorizerServiceClient.CategorizeTicketAsync(ticket, ct);
        return Ok(classifiedTicket);
    }

    /// <summary>
    /// Добавить новый чек
    /// </summary>
    /// <param name="ticket">Чек</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpPost("")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<bool>> SaveTicket(TicketHeader ticket, CancellationToken ct)
    {
        var saveResult = await _ticketRepository.SaveTicketAsync(ticket, ct);
        return new StatusCodeResult(201);
    }

    /// <summary>
    /// Получить информацию по чеку, категоризировать позиции и сохранить в бд
    /// </summary>
    /// <param name="qr"></param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet("auto")]
    [ProducesResponseType(statusCode: 200, type: typeof(AutoTicketResult))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<AutoTicketResult>> GetTicketDataAuto(string qr, CancellationToken ct)
    {
        var autoResult = await _ticketService.ProcessQrAutoAsync(qr, ct);
        return Ok(autoResult);
    }

    /// <summary>
    /// Получить топ-10 категорий в чеках
    /// </summary>
    /// <remarks>Используется в выпадающих списках при ручном выборе категорий</remarks>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    [HttpGet("top-categories")]
    [ProducesResponseType(statusCode: 200, type: typeof(TopCategories))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<TopCategories>> GetTopCategories(CancellationToken ct)
    {
        var categories = await _ticketService.GetTopCategoriesAsync(ct);
        return Ok(categories);
    }
}