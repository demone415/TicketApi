using Quartz;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Models;

namespace TicketApi.Service.Workers;

public class TicketReFetcher : IJob
{
    private readonly ILogger<TicketReFetcher> _logger;
    private readonly PostgresContext _postgresContext;
    private readonly ITicketService _ticketService;

    public TicketReFetcher(ILogger<TicketReFetcher> logger, PostgresContext postgresContext, ITicketService ticketService)
    {
        _logger = logger;
        _postgresContext = postgresContext;
        _ticketService = ticketService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        var headersInQueue = _postgresContext.Headers
            .Where(h => h.Status == HeaderStatuses.InQueue);

        foreach (var header in headersInQueue)
        {
            var dataResult = await _ticketService.GetTicketData(header);
            if (dataResult.Header.NextFetchDateTime == DateTimeOffset.MaxValue.ToUniversalTime())
            {
                header.Status = HeaderStatuses.RequestsExceeded;
            }
        }
        
        await _postgresContext.SaveChangesAsync();
    }
}