using Quartz;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Models;

namespace TicketApi.Service.Workers;

public class TicketReFetcher : IJob
{
    private readonly ILogger<TicketReFetcher> _logger;
    private readonly MainContext _mainContext;
    private readonly ITicketService _ticketService;

    public TicketReFetcher(ILogger<TicketReFetcher> logger, MainContext mainContext, ITicketService ticketService)
    {
        _logger = logger;
        _mainContext = mainContext;
        _ticketService = ticketService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var ctx = new CancellationTokenSource(TimeSpan.FromMinutes(30));
        var ct = ctx.Token;
        var headersInQueue = _mainContext.Headers
            .Where(h => h.Status == HeaderStatuses.InQueue);

        foreach (var header in headersInQueue)
        {
            var dataResult = await _ticketService.GetTicketDataAsync(header, ct);
            if (dataResult.Header.NextFetchDateTime == DateTimeOffset.MaxValue.ToUniversalTime())
                header.Status = HeaderStatuses.RequestsExceeded;
        }

        await _mainContext.SaveChangesAsync();
    }
}