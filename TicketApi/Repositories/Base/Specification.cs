using System.Linq.Expressions;

namespace TicketApi.Repositories.Base;

/// <summary>
/// Базовый класс спецификации
/// </summary>
/// <typeparam name="TEntity">Тип entity</typeparam>
public abstract class Specification<TEntity> : ISpecification<TEntity> where TEntity : class
{
    /// <summary>
    /// .ctor
    /// </summary>
    protected Specification()
    {
        _includes = new List<Expression<Func<TEntity, dynamic>>>();
        _orders = new List<(Expression<Func<TEntity, dynamic>> keySelector, bool isAscending)>();
    }

    /// <inheritdoc />
    public abstract Expression<Func<TEntity, bool>> ToExpression();

    /// <summary>
    /// Помечает ключ из <paramref name="keySelector"/> для загрузки из базы.
    /// </summary>
    /// <param name="keySelector">Поле для загрузки из базы</param>
    /// <remarks>
    /// Поддерживается только один уровень вложенности.
    /// </remarks>
    public Specification<TEntity> Include(Expression<Func<TEntity, dynamic>> keySelector)
    {
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        _includes.Add(keySelector);

        return this;
    }

    /// <summary>
    /// Метод добавление инклудов через строковое обозначение
    /// </summary>
    /// <param name="includeString"></param>
    public void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Добавляет сортировку по возрастанию поля <paramref name="keySelector"/>.
    /// </summary>
    /// <param name="keySelector">Поле для сортировки</param>
    public Specification<TEntity> OrderBy(Expression<Func<TEntity, dynamic>> keySelector)
    {
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        _orders.Add((keySelector, true));

        return this;
    }

    /// <summary>
    /// Добавляет сортировку по убыванию поля <paramref name="keySelector"/>.
    /// </summary>
    /// <param name="keySelector">Поле для сортировки</param>
    public Specification<TEntity> OrderByDescending(Expression<Func<TEntity, dynamic>> keySelector)
    {
        if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

        _orders.Add((keySelector, false));

        return this;
    }

    private readonly List<Expression<Func<TEntity, dynamic>>> _includes;

    /// <inheritdoc />
    public IReadOnlyList<Expression<Func<TEntity, dynamic>>> Includes => _includes;

    private readonly List<string> _includesString = new();

    public List<string> IncludeStrings { get; } = new();

    private readonly List<(Expression<Func<TEntity, dynamic>> keySelector, bool isAscending)> _orders;

    /// <inheritdoc />
    public IReadOnlyList<(Expression<Func<TEntity, dynamic>> keySelector, bool isAscending)> Orders => _orders;

    /// <summary>
    /// Комбинирует данную спецификацию со спецификацией <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">Дополнительная спецификация</param>
    public Specification<TEntity> And(ISpecification<TEntity> specification)
    {
        return new AndSpecification<TEntity>(this, specification);
    }

    /// <summary>
    /// Комбинирует данную спецификацию со спецификацией <paramref name="specification"/> по условию
    /// </summary>
    /// <param name="condition">Условия на комбинацию спецификаций</param>
    /// <param name="getSpecification">Делегат возвращающий дополнительная спецификация</param>
    public Specification<TEntity> AndIf(bool condition, Func<ISpecification<TEntity>> getSpecification)
    {
        return condition
            ? new AndSpecification<TEntity>(this, getSpecification())
            : new AndSpecification<TEntity>(this, new AllSpecification<TEntity>());
    }

    /// <summary>
    /// Комбинирует данную спецификацию со спецификацией <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">Дополнительная спецификация</param>
    public Specification<TEntity> Or(ISpecification<TEntity> specification)
    {
        return new OrSpecification<TEntity>(this, specification);
    }

    /// <summary>
    /// Комбинирует данную спецификацию со спецификацией <paramref name="specification"/> по условию
    /// </summary>
    /// <param name="condition">Условия на комбинацию спецификаций</param>
    /// <param name="getSpecification">Делегат возвращающий дополнительная спецификация</param>
    public Specification<TEntity> OrIf(bool condition, Func<ISpecification<TEntity>> getSpecification)
    {
        return condition
            ? new OrSpecification<TEntity>(this, getSpecification())
            : new OrSpecification<TEntity>(this, new AllSpecification<TEntity>());
    }
}

/// <summary>
/// Спецификация, которую удовлетворяют любые элементы <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">Тип entity</typeparam>
public sealed class AllSpecification<TEntity> : Specification<TEntity> where TEntity : class
{
    /// <inheritdoc />
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return entity => true;
    }
}

/// <summary>
/// Реализует логичесую операцию И для своих операндов.
/// </summary>
/// <typeparam name="TEntity">Тип entity</typeparam>
public sealed class AndSpecification<TEntity> : Specification<TEntity> where TEntity : class
{
    private readonly Expression<Func<TEntity, bool>> _expression;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    public AndSpecification(ISpecification<TEntity> left, ISpecification<TEntity> right)
    {
        if (left.Includes.Any() || right.Includes.Any())
            throw new ArgumentException("Инклюды не могут быть указаны в операндах AndSpecification");

        if (left.Orders.Any() || right.Orders.Any())
            throw new ArgumentException("Порядок сортировки не может быть указан в операндах AndSpecification");

        _expression = Combine(left.ToExpression(), right.ToExpression());
    }

    private static Expression<Func<TEntity, bool>> Combine(Expression<Func<TEntity, bool>> l,
        Expression<Func<TEntity, bool>> r)
    {
        var parameter = Expression.Parameter(typeof(TEntity));

        var leftVisitor = new ReplaceExpressionVisitor(l.Parameters[0], parameter);
        var left = leftVisitor.Visit(l.Body);

        var rightVisitor = new ReplaceExpressionVisitor(r.Parameters[0], parameter);
        var right = rightVisitor.Visit(r.Body);

        return Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(left, right), parameter);
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return _expression;
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}

/// <summary>
/// Реализует логичесую операцию И для своих операндов.
/// </summary>
/// <typeparam name="TEntity">Тип entity</typeparam>
public sealed class OrSpecification<TEntity> : Specification<TEntity> where TEntity : class
{
    private readonly Expression<Func<TEntity, bool>> _expression;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="left">Левый операнд</param>
    /// <param name="right">Правый операнд</param>
    public OrSpecification(ISpecification<TEntity> left, ISpecification<TEntity> right)
    {
        if (left.Includes.Any() || right.Includes.Any())
            throw new ArgumentException("Инклюды не могут быть указаны в операндах OrSpecification");

        if (left.Orders.Any() || right.Orders.Any())
            throw new ArgumentException("Порядок сортировки не может быть указан в операндах OrSpecification");

        _expression = Combine(left.ToExpression(), right.ToExpression());
    }

    private static Expression<Func<TEntity, bool>> Combine(Expression<Func<TEntity, bool>> l,
        Expression<Func<TEntity, bool>> r)
    {
        var parameter = Expression.Parameter(typeof(TEntity));

        var leftVisitor = new ReplaceExpressionVisitor(l.Parameters[0], parameter);
        var left = leftVisitor.Visit(l.Body);

        var rightVisitor = new ReplaceExpressionVisitor(r.Parameters[0], parameter);
        var right = rightVisitor.Visit(r.Body);

        return Expression.Lambda<Func<TEntity, bool>>(Expression.Or(left, right), parameter);
    }

    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return _expression;
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}