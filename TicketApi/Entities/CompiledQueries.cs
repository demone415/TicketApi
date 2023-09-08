using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace TicketApi.Entities;

public static class CompiledQueries
{
    public static readonly Func<PostgresContext, Expression<Func<TicketLine, string>>, IAsyncEnumerable<string>>
        GetTopCategoriesAsync =
            EF.CompileAsyncQuery((PostgresContext db, Expression<Func<TicketLine, string>> groupByField) =>
                db.Lines.AsNoTracking()
                    .GroupBy(l => l.Category1)
                    .OrderByDescending(gp => gp.Count())
                    .Take(10)
                    .Select(l => l.Key));

    public static readonly Func<PostgresContext, int, int, IAsyncEnumerable<TicketHeader>>
        GetTicketsAsync =
            EF.CompileAsyncQuery((PostgresContext db, int pageNum, int pageSize) =>
                db.Headers.AsNoTracking()
                    .Include(header => header.Lines)
                    .OrderByDescending(x => x.Id)
                    .Skip(pageNum * pageSize)
                    .Take(pageSize));
}