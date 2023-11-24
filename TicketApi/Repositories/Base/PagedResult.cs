using System.Collections;

namespace TicketApi.Repositories.Base;

/// <summary>
/// Страница результатов постраничного поиска
/// </summary>
public sealed class PagedResult<TEntity> : IReadOnlyList<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Общее количество объектов, удовлетворяющих запросу
    /// </summary>
    public int Total { get; }

    private readonly IReadOnlyList<TEntity> _result;

    public PagedResult(IReadOnlyList<TEntity> result, int total)
    {
        _result = result ?? throw new ArgumentNullException(nameof(result));
        Total = total;
    }

    /// <inheritdoc />
    public IEnumerator<TEntity> GetEnumerator()
    {
        return _result.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public int Count => _result.Count;

    /// <inheritdoc />
    public TEntity this[int index] => _result[index];
}