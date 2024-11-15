using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;

namespace Dommel;

/// <summary>
/// Represents a typed SQL expression.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class SqlExpression<TEntity>
{
    private static readonly Type EntityType = typeof(TEntity);
    private static readonly Func<TEntity> NewEntityFunc = Expression.Lambda<Func<TEntity>>(
        Expression.New(typeof(TEntity).GetConstructors()[0])).Compile();
    private readonly SqlVisitor _visitor;
    private readonly List<(string, string?)> _whereStatements = [];
    private readonly StringBuilder _orderByBuilder = new();
    private string? _selectQuery;
    private string? _pagingQuery;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlExpression{TEntity}"/>
    /// class using the specified <see cref="ISqlBuilder"/>.
    /// </summary>
    /// <param name="sqlBuilder">The <see cref="ISqlBuilder"/> instance.</param>
    public SqlExpression(ISqlBuilder sqlBuilder)
    {
        SqlBuilder = sqlBuilder;
        _visitor = new SqlVisitor(sqlBuilder);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlExpression{TEntity}"/>
    /// class using the specified <see cref="ISqlBuilder"/>.
    /// </summary>
    /// <param name="sqlBuilder">The <see cref="ISqlBuilder"/> instance.</param>
    /// <param name="sqlVisitor">The SQL visitor to use.</param>
    public SqlExpression(ISqlBuilder sqlBuilder, SqlVisitor sqlVisitor)
    {
        SqlBuilder = sqlBuilder;
        _visitor = sqlVisitor;
    }

    /// <summary>
    /// Gets the <see cref="ISqlBuilder"/> instance used by this SQL expression.
    /// </summary>
    protected virtual ISqlBuilder SqlBuilder { get; }

    /// <summary>
    /// Gets the <see cref="IColumnNameResolver"/> instance used by this SQL expression.
    /// </summary>
    protected virtual IColumnNameResolver ColumnNameResolver => DommelMapper.ColumnNameResolver;

    /// <summary>
    /// Selects all columns from <typeparamref name="TEntity"/>.
    /// </summary>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> Select()
    {
        _selectQuery = $"select * from {Resolvers.Table(typeof(TEntity), SqlBuilder)}";
        return this;
    }

    /// <summary>
    /// Selects the specified set of columns from <typeparamref name="TEntity"/>.
    /// </summary>
    /// <param name="selector">The columns to select.
    /// E.g. <code>x => new { x.Foo, x.Bar }</code>.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> Select(Expression<Func<TEntity, object>> selector)
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        PropertyInfo[]? props = null;

        // Get properties from expression
        if (selector.NodeType == ExpressionType.Lambda && selector.Body?.NodeType == ExpressionType.New)
        {
            if (selector.Body is NewExpression newExpression)
            {
                props = newExpression
                    .Arguments
                    .OfType<MemberExpression>()
                    .Select(x => x.Expression?.Type.GetProperty(x.Member.Name)!)
                    .ToArray();
            }
        }

        if (props?.Any() != true)
        {
            // Invoke the selector expression to obtain an object instance of anonymous type
            var obj = selector.Compile().Invoke(NewEntityFunc());

            // Resolve properties of anonymous type
            props = obj.GetType().GetProperties();
        }

        if (props.Length == 0)
        {
            throw new ArgumentException($"Projection over type '{typeof(TEntity).Name}' yielded no properties.", nameof(selector));
        }

        var columns = props.Select(p => Resolvers.Column(p, SqlBuilder));

        // Create the select query
        var tableName = Resolvers.Table(EntityType, SqlBuilder);
        _selectQuery = $"select {string.Join(", ", columns)} from {tableName}";
        return this;
    }

    /// <summary>
    /// Builds a SQL expression for the specified filter expression.
    /// </summary>
    /// <param name="expression">The filter expression on the entity.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> Where(Expression<Func<TEntity, bool>>? expression)
    {
        if (expression != null)
        {
            AppendToWhere("and", expression);
        }

        return this;
    }

    /// <summary>
    /// Adds another where-statement with the 'and' operator.
    /// </summary>
    /// <param name="expression">The filter expression on the entity.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> AndWhere(Expression<Func<TEntity, bool>>? expression)
    {
        if (_whereStatements.Count == 0)
        {
            throw new InvalidOperationException("Start the where statement with the 'Where' method.");
        }
        if (expression != null)
        {
            AppendToWhere("and", expression);
        }
        return this;
    }

    /// <summary>
    /// Adds another where-statement with the 'or' operator to the current expression.
    /// </summary>
    /// <param name="expression">The filter expression on the entity.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> OrWhere(Expression<Func<TEntity, bool>>? expression)
    {
        if (_whereStatements.Count == 0)
        {
            throw new InvalidOperationException("Start the where statement with the 'Where' method.");
        }
        if (expression != null)
        {
            AppendToWhere("or", expression);
        }
        return this;
    }

    private void AppendToWhere(string? conditionOperator, Expression expression)
    {
        var sqlExpression = _visitor.VisitExpression(expression).ToString()!;
        _whereStatements.Add((sqlExpression, _whereStatements.Count == 0 ? null : conditionOperator));
    }

    /// <summary>
    /// Adds a paging-statement to the current expression.
    /// </summary>
    /// <param name="pageNumber">The number of the page to fetch, starting at 1.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> Page(int pageNumber, int pageSize)
    {
        _pagingQuery = SqlBuilder.BuildPaging(null, pageNumber, pageSize).Substring(1);
        return this;
    }

    /// <summary>
    /// Adds an order-by-statement (ascending) to the current expression.
    /// </summary>
    /// <param name="selector">The column to order by. E.g. <code>x => x.Name</code>.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> OrderBy(Expression<Func<TEntity, object?>> selector)
    {
        OrderByCore(selector, "asc");
        return this;
    }

    /// <summary>
    /// Adds an order-by-statement (ascending) to the current expression.
    /// </summary>
    /// <param name="property">The property info of the column to order by.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> OrderBy(PropertyInfo property)
    {
        AppendOrderBy(Resolvers.Column(property, SqlBuilder), direction: "asc");
        return this;
    }

    /// <summary>
    /// Adds an order-by-statement (descending) to the current expression.
    /// </summary>
    /// <param name="selector">The column to order by. E.g. <code>x => x.Name</code>.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> OrderByDescending(Expression<Func<TEntity, object?>> selector)
    {
        OrderByCore(selector, "desc");
        return this;
    }

    /// <summary>
    /// Adds an order-by-statement (descending) to the current expression.
    /// </summary>
    /// <param name="property">The property info of the column to order by.</param>
    /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
    public virtual SqlExpression<TEntity> OrderByDescending(PropertyInfo property)
    {
        AppendOrderBy(Resolvers.Column(property, SqlBuilder), direction: "desc");
        return this;
    }

    private void OrderByCore(Expression<Func<TEntity, object?>> selector, string direction)
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        var column = _visitor.VisitExpression(selector.Body) as string;
        AppendOrderBy(column, direction);
    }

    private void AppendOrderBy(string? column, string direction, bool prepend = false)
    {
        if (string.IsNullOrEmpty(column))
        {
            throw new ArgumentNullException(nameof(column));
        }
        if (string.IsNullOrEmpty(direction))
        {
            throw new ArgumentNullException(nameof(direction));
        }
        if (_orderByBuilder.Length == 0)
        {
            _orderByBuilder.Append($" order by {column} {direction}");
        }
        else if (prepend)
        {
            // Insert the column sort option right after 'order by '
            _orderByBuilder.Insert(10, $"{column} {direction}, ");
        }
        else
        {
            _orderByBuilder.Append($", {column} {direction}");
        }
    }



    /// <summary>
    /// Returns the current SQL query.
    /// </summary>
    /// <returns>The current SQL query.</returns>
    public string ToSql()
    {
        var query = "";
        if (!string.IsNullOrEmpty(_selectQuery))
        {
            query += _selectQuery;
        }

        if (_whereStatements.Count > 0)
        {
            var whereBuilder = new StringBuilder();
            foreach (var (sql, conditionOperator) in _whereStatements)
            {
                if (whereBuilder.Length == 0)
                {
                    whereBuilder.Append(" where ");
                }
                else if (!string.IsNullOrEmpty(conditionOperator))
                {
                    whereBuilder.Append(' ').Append(conditionOperator).Append(' ');
                }
                else
                {
                    throw new Exception("Expected condition operator with multiple where statements.");
                }

                if (_whereStatements.Count > 1)
                {
                    whereBuilder.Append('(');
                }

                whereBuilder.Append(sql);

                if (_whereStatements.Count > 1)
                {
                    whereBuilder.Append(')');
                }
            }

            if (whereBuilder.Length > 0)
            {
                query += whereBuilder.ToString();
            }
        }

        var orderBy = _orderByBuilder.ToString();
        if (!string.IsNullOrEmpty(orderBy))
        {
            query += orderBy;
        }

        if (!string.IsNullOrEmpty(_pagingQuery))
        {
            if (string.IsNullOrEmpty(orderBy))
            {
                // When we're paging we'll need an order to guarantee consistent paging results, when
                // the user did not specified an order themself we'll order on the PKs of the table.
                var keyColumns = Resolvers.KeyProperties(typeof(TEntity)).Select(p => Resolvers.Column(p.Property, SqlBuilder));
                AppendOrderBy(string.Join(", ", keyColumns), direction: "asc", prepend: true);
                query += _orderByBuilder.ToString();
            }

            query += _pagingQuery;
        }
        return query;
    }

    /// <summary>
    /// Returns the current SQL query.
    /// </summary>
    /// <param name="parameters">When this method returns, contains the parameters for the query.</param>
    /// <returns>The current SQL query.</returns>
    public string ToSql(out DynamicParameters parameters)
    {
        parameters = _visitor.Parameters;
        return ToSql();
    }

    /// <summary>
    /// Returns the current SQL query.
    /// </summary>
    /// <returns>The current SQL query.</returns>
    public override string ToString() => ToSql();
}