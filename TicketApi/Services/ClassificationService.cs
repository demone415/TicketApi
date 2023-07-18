using TicketApi.Entities;
using TicketApi.Interfaces.Services;

namespace TicketApi.Services;

public class ClassificationService : IClassificationService
{
    public async Task<TicketHeader> Classify(TicketHeader header)
    {
        await Parallel.ForEachAsync(header.Lines, (line, token) =>
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
        return string.Join(" ", str.Split( new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries ));
    }
}