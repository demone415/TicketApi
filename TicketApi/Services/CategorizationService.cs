using TicketApi.Entities;
using TicketApi.Interfaces.Services;
using TicketApi.Shared.SimplifySetting;

namespace TicketApi.Services;

public class CategorizationService : ICategorizationService
{
    public async Task<TicketHeader> CategorizeTicketAsync(TicketHeader header, CancellationToken ct)
    {
        await Parallel.ForEachAsync(header.Lines, ct, (line, token) =>
        {
            token.ThrowIfCancellationRequested();
            line.Name = RemoveDoubleSpaces(line.Name);
            line.Category1 = null;
            line.Category2 = null;
            line.Category3 = null;
            line.Tags = null;
            return ValueTask.CompletedTask;
        });

        return header;
    }

    private static string RemoveDoubleSpaces(string str)
    {
        return string.Join(" ", str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }
}