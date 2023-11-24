using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketApi.Entities;

namespace TicketApi.Repositories.Base;

/// <summary>
/// Repository Base
/// </summary>
/// <typeparam name="TEntity">Тип entity</typeparam>
public class RepositoryBase<TEntity> : IRepository<TEntity>, IDisposable where TEntity : class
{
    private MainContext _context;

    /// <summary>
    /// .ctor
    /// </summary>
    protected RepositoryBase(MainContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        return set.CountAsync(ct);
    }

    /// <inheritdoc />
    public int Count(ISpecification<TEntity> specification)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        return set.Count();
    }

    public TEntity Find(ISpecification<TEntity> specification, bool tracking = true)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        if (!tracking)
            set = set.AsNoTracking();

        return set.FirstOrDefault();
    }

    public async Task<TEntity> FindAsync(ISpecification<TEntity> specification, bool tracking = true,
        CancellationToken ct = default)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        if (!tracking)
            set = set.AsNoTracking();

        return await set.FirstOrDefaultAsync(ct);
    }

    public IReadOnlyList<TEntity> Get(ISpecification<TEntity> specification, int take, int? skip = null,
        bool tracking = true)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        set = set.Take(take);

        if (skip.HasValue) set = set.Skip(skip.Value);

        if (!tracking)
            set.AsNoTracking();

        return set.ToList();
    }

    public async Task<IReadOnlyList<TEntity>> GetAsync(ISpecification<TEntity> specification, int take,
        int? skip = null, CancellationToken ct = default,
        bool tracking = true)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        set = set.Take(take);

        if (skip.HasValue) set = set.Skip(skip.Value);

        if (!tracking)
            set = set.AsNoTracking();

        return await set.ToListAsync(ct);
    }

    public IReadOnlyList<TEntity> GetAll(ISpecification<TEntity> specification, bool tracking = true)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        if (!tracking)
            set = set.AsNoTracking();

        return set.ToList();
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity> specification,
        CancellationToken ct = default, bool tracking = true)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        if (!tracking)
            set = set.AsNoTracking();

        return await set.ToListAsync(ct);
    }

    public async Task<PagedResult<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int take,
        int? skip = null, CancellationToken ct = default,
        bool tracking = true)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        var total = await set.CountAsync(ct);

        if (skip.HasValue) set = set.Skip(skip.Value);

        set = set.Take(take);

        if (!tracking)
            set = set.AsNoTracking();

        var page = await set.ToListAsync(ct);

        return new PagedResult<TEntity>(page, total);
    }

    /// <inheritdoc />
    public TEntity Find(ISpecification<TEntity> specification)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        return set.FirstOrDefault();
    }

    public void Add(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        _context.Set<TEntity>().Add(entity);
    }

    public async Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return await _context.Set<TEntity>().AddAsync(entity, ct);
    }


    public void AddRange(IReadOnlyList<TEntity> entities)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        _context.Set<TEntity>().AddRange(entities);
    }

    public async Task AddRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken ct = default)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        await _context.Set<TEntity>().AddRangeAsync(entities, ct);
    }

    /// <summary>
    /// Помечает <paramref name="entity"/> в качестве объекта для удаления из базы данных.
    /// </summary>
    /// <param name="entity">Объект для добавления в базу данных</param>
    public void Remove(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        _context.Set<TEntity>().Remove(entity);
    }

    /// <inheritdoc />
    public int Save()
    {
        return _context.SaveChanges();
    }

    /// <inheritdoc />
    public async Task<int> SaveAsync(CancellationToken ct = default)
    {
        return await _context.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public IReadOnlyList<TEntity> Get(ISpecification<TEntity> specification, int take, int? skip = null)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        set = set.Take(take);

        if (skip.HasValue) set = set.Skip(skip.Value);

        return set.ToList();
    }

    public async Task<bool> ExistAsync(ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        var set = ApplySpec(specification);
        return await set.AnyAsync(ct);
    }

    /// <inheritdoc />
    public IReadOnlyList<TEntity> GetAll(ISpecification<TEntity> specification)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        return set.ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetAsync(ISpecification<TEntity> specification, int take,
        int? skip = null, CancellationToken ct = default)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        set = set.Take(take);

        if (skip.HasValue) set = set.Skip(skip.Value);

        return await set.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TDest>> GetAsync<TDest>(ISpecification<TEntity> specification, int take,
        int? skip = null, CancellationToken ct = default)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);


        if (skip.HasValue) set = set.Skip(skip.Value);

        set = set.Take(take);
        return await set.ProjectToType<TDest>().ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecification<TEntity> specification,
        CancellationToken ct = default)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        return await set.ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<PagedResult<TEntity>> GetPagedAsync(ISpecification<TEntity> specification, int take,
        int? skip = null, CancellationToken ct = default)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var set = ApplySpec(specification);

        var total = await set.CountAsync(ct);


        if (skip.HasValue) set = set.Skip(skip.Value);

        set = set.Take(take);
        var page = await set.ToListAsync(ct);

        return new PagedResult<TEntity>(page, total);
    }

    public void RemoveRange(IReadOnlyList<TEntity> entities)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        _context.Set<TEntity>().RemoveRange(entities);
    }

    /// <inheritdoc />
    public EntityEntry<TEntity> Update(TEntity obj)
    {
        return _context.Update(obj);
    }

    /// <inheritdoc />
    public Task UpdateRange(IEnumerable<TEntity> obj)
    {
        _context.UpdateRange(obj);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    private IQueryable<TEntity> ApplySpec(ISpecification<TEntity> spec)
    {
        IQueryable<TEntity> set = _context.Set<TEntity>();

        // Поддерживаются только инклюды первого уровня.
        foreach (var include in spec.Includes) set = set.Include(include);

        foreach (var includeString in spec.IncludeStrings) set = set.Include(includeString);

        set = set.Where(spec.ToExpression());

        if (spec.Orders.Any())
        {
            var orderedSet = spec.Orders[0].isAscending
                ? set.OrderBy(spec.Orders[0].keySelector)
                : set.OrderByDescending(spec.Orders[0].keySelector);

            foreach (var (keySelector, isAscending) in spec.Orders.Skip(1))
                orderedSet = isAscending ? orderedSet.ThenBy(keySelector) : orderedSet.ThenByDescending(keySelector);

            return orderedSet;
        }

        return set;
    }

    public object GetNewInstance(bool readOnly = false)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var context in assemblies
                     .Where(x => x.FullName != null)
                     .SelectMany(s => s.GetTypes())
                     .Where(p => typeof(IRepository<TEntity>).IsAssignableFrom(p)
                                 && p.IsClass
                                 && (!p.FullName?.Contains("RepositoryBase") ?? false)))
            return Activator.CreateInstance(context, _context.GetNewInstance(readOnly), readOnly);

        return null;
    }
}