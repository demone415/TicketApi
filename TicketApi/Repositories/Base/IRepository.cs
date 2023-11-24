using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TicketApi.Repositories.Base;

/// <summary>
/// IRepository
/// </summary>
/// <typeparam name="TEntity">TEntity</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Возвращает количество элементов <see cref="TEntity"/>, удовлетворяющий условию <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    /// <param name="ct">Токен отмены</param>
    Task<int> CountAsync([NotNull] ISpecification<TEntity> specification, CancellationToken ct = default);

    /// <summary>
    /// Возвращает количество элементов <see cref="TEntity"/>, удовлетворяющий условию <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    int Count([NotNull] ISpecification<TEntity> specification);

    /// <summary>
    /// Возвращает первый объект, подходящий под спецификацию <paramref name="specification"/> или <c>null</c>,
    /// если ни одного подходящего объекта не найдено
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    [CanBeNull]
    TEntity Find([NotNull] ISpecification<TEntity> specification);

    /// <summary>
    /// Возвращает первый объект, подходящий под спецификацию <paramref name="specification"/> или <c>null</c>,
    /// если ни одного подходящего объекта не найдено
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    /// <param name="tracking">Нужно ли отслеживать изменения</param>
    /// <param name="ct">Токен отмены</param>
    [ItemCanBeNull]
    Task<TEntity> FindAsync([NotNull] ISpecification<TEntity> specification, bool tracking = true,
        CancellationToken ct = default);

    /// <summary>
    /// Возвращает список объектов, размером не более <paramref name="take"/> элементов, удовлетворяющих
    /// спецификации <paramref name="specification"/>, пропуская первые <paramref name="skip"/> объектов.
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    /// <param name="take">Максимальное количество возвращаемых объектов</param>
    /// <param name="skip">Количество объектов, которые нужно пропустить</param>
    [NotNull]
    IReadOnlyList<TEntity> Get([NotNull] ISpecification<TEntity> specification, int take, int? skip = null);

    /// <summary>
    /// Возвращает список объектов, размером не более <paramref name="take"/> элементов, удовлетворяющих
    /// спецификации <paramref name="specification"/>, пропуская первые <paramref name="skip"/> объектов.
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    /// <param name="take">Максимальное количество возвращаемых объектов</param>
    /// <param name="skip">Количество объектов, которые нужно пропустить</param>
    /// <param name="ct">Токен отмены</param>
    [ItemNotNull]
    Task<IReadOnlyList<TEntity>> GetAsync([NotNull] ISpecification<TEntity> specification, int take,
        int? skip = null, CancellationToken ct = default);

    /// <summary>
    /// Возвращает список объектов, размером не более <paramref name="take"/> элементов, удовлетворяющих
    /// спецификации <paramref name="specification"/>, пропуская первые <paramref name="skip"/> объектов.
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    /// <param name="take">Максимальное количество возвращаемых объектов</param>
    /// <param name="skip">Количество объектов, которые нужно пропустить</param>
    /// <param name="ct">Токен отмены</param>
    Task<IReadOnlyList<TDest>> GetAsync<TDest>(ISpecification<TEntity> specification, int take,
        int? skip = null, CancellationToken ct = default);

    /// <summary>
    /// Проверяет существование хотя бы одного элемента, удовлетворяющих
    /// спецификации <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    /// <param name="ct">Токен отмены</param>
    [NotNull]
    Task<bool> ExistAsync([NotNull] ISpecification<TEntity> specification, CancellationToken ct = default);

    /// <summary>
    /// Возвращает список всех объектов, удовлетворяющих спецификации <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    [NotNull]
    IReadOnlyList<TEntity> GetAll([NotNull] ISpecification<TEntity> specification);

    /// <summary>
    /// Возвращает список объектов удовлетворяющих
    /// спецификации <paramref name="specification"/>
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    /// <param name="ct">Токен отмены</param>
    [ItemNotNull]
    Task<IReadOnlyList<TEntity>> GetAllAsync([NotNull] ISpecification<TEntity> specification,
        CancellationToken ct = default);

    /// <summary>
    /// Возвращает список объектов, размером не более <paramref name="take"/> элементов, удовлетворяющих
    /// спецификации <paramref name="specification"/>, пропуская первые <paramref name="skip"/> объектов,
    /// а также общее количество объектов, удовлетворяющих спецификации.
    /// </summary>
    /// <param name="specification">Спецификация объектов</param>
    /// <param name="take">Максимальное количество возвращаемых объектов</param>
    /// <param name="skip">Количество объектов, которые нужно пропустить</param>
    /// <param name="ct">Токен отмены</param>
    [ItemNotNull]
    Task<PagedResult<TEntity>> GetPagedAsync([NotNull] ISpecification<TEntity> specification, int take,
        int? skip = null, CancellationToken ct = default);

    /// <summary>
    /// Помечает <paramref name="entity"/> в качестве объекта для добавления в базу данных.
    /// </summary>
    /// <param name="entity">Объект для добавления в базу данных</param>
    void Add([NotNull] TEntity entity);

    /// <summary>
    /// Помечает <paramref name="entity"/> в качестве объекта для добавления в базу данных.
    /// </summary>
    /// <param name="entity">Объект для добавления в базу данных</param>
    /// <param name="ct">Токен отмены</param>
    Task<EntityEntry<TEntity>> AddAsync([NotNull] TEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Помечает список <paramref name="entities"/> в качестве объектов для добавления в базу данных.
    /// </summary>
    /// <param name="entities">Объекты для добавления в базу данных</param>
    void AddRange(IReadOnlyList<TEntity> entities);

    /// <summary>
    /// Помечает список <paramref name="entities"/> в качестве объектов для добавления в базу данных.
    /// </summary>
    /// <param name="entities">Объекты для добавления в базу данных</param>
    /// <param name="ct">Токен отмены</param>
    Task AddRangeAsync(IReadOnlyList<TEntity> entities, CancellationToken ct = default);

    /// <summary>
    /// Сохраняет изменения в базу
    /// </summary>
    int Save();

    /// <summary>
    /// Сохраняет изменения в базу
    /// </summary>
    /// <param name="ct">Токен отмены</param>
    Task<int> SaveAsync(CancellationToken ct = default);

    /// <summary>
    /// Обновляем модель в репозитории
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    EntityEntry<TEntity> Update(TEntity obj);

    /// <summary>
    /// Обновляем массив моделей в репозитории
    /// </summary>
    /// <param name="obj"></param>
    Task UpdateRange(IEnumerable<TEntity> obj);

    /// <summary>
    /// Удаление объекта в бд
    /// </summary>
    /// <param name="specification"></param>
    void Remove(TEntity specification);

    /// <summary>
    /// Помечает список <paramref name="entities"/> в качестве объектов для удаления из базы данных.
    /// </summary>
    /// <param name="entities">Объекты для удаления из базы данных</param>
    void RemoveRange(IReadOnlyList<TEntity> entities);

    /// <summary>
    /// Get New Instance
    /// </summary>
    object GetNewInstance(bool readOnly = false);
}