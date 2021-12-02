using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Executes an expression to query data from <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity to query data from.</typeparam>
    /// <param name="con">The connection to query data from.</param>
    /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The collection of entities returned from the query.</returns>
    public static IEnumerable<TEntity> From<TEntity>(this IDbConnection con, Action<SqlExpression<TEntity>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
    {
        var sqlExpression = CreateSqlExpression<TEntity>(GetSqlBuilder(con));
        sqlBuilder(sqlExpression);
        var sql = sqlExpression.ToSql(out var parameters);
        LogReceived?.Invoke(sql);
        return con.Query<TEntity>(sql, parameters, transaction, buffered);
    }

    /// <summary>
    /// Executes an expression to query data from <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity to query data from.</typeparam>
    /// <param name="con">The connection to query data from.</param>
    /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The collection of entities returned from the query.</returns>
    public static async Task<IEnumerable<TEntity>> FromAsync<TEntity>(this IDbConnection con, Action<SqlExpression<TEntity>> sqlBuilder, IDbTransaction? transaction = null)
    {
        var sqlExpression = CreateSqlExpression<TEntity>(GetSqlBuilder(con));
        sqlBuilder(sqlExpression);
        var sql = sqlExpression.ToSql(out var parameters);
        LogReceived?.Invoke(sql);
        return await con.QueryAsync<TEntity>(sql, parameters, transaction);
    }
}
