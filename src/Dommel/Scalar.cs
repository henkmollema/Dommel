using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Executes an expression to query a scalar value from <typeparamref name="TEntity"/>.
    /// This is useful for aggregate queries such as <c>max</c>, <c>min</c>, <c>sum</c>, etc.
    /// </summary>
    /// <typeparam name="TEntity">The entity to query data from.</typeparam>
    /// <typeparam name="TResult">The type of the scalar result.</typeparam>
    /// <param name="con">The connection to query data from.</param>
    /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <remarks>Make sure that both <typeparamref name="TResult"/> and the selected columns in <paramref name="sqlBuilder"/> represent a scalar (single) value.</remarks>
    /// <returns>The scalar value returned from the query.</returns>
    public static TResult? Scalar<TEntity, TResult>(this IDbConnection con, Action<SqlExpression<TEntity>> sqlBuilder, IDbTransaction? transaction = null)
    {
        var sqlExpression = CreateSqlExpression<TEntity>(GetSqlBuilder(con));
        sqlBuilder(sqlExpression);
        var sql = sqlExpression.ToSql(out var parameters);
        LogReceived?.Invoke(sql);
        return con.ExecuteScalar<TResult>(sql, parameters, transaction);
    }

    /// <summary>
    /// Executes an expression to query a scalar value from <typeparamref name="TEntity"/>.
    /// This is useful for aggregate queries such as <c>max</c>, <c>min</c>, <c>sum</c>, etc.
    /// </summary>
    /// <typeparam name="TEntity">The entity to query data from.</typeparam>
    /// <typeparam name="TResult">The type of the scalar result.</typeparam>
    /// <param name="con">The connection to query data from.</param>
    /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <remarks>Make sure that both <typeparamref name="TResult"/> and the selected columns in <paramref name="sqlBuilder"/> represent a scalar (single) value.</remarks>
    /// <returns>The scalar value returned from the query.</returns>
    public static async Task<TResult?> ScalarAsync<TEntity, TResult>(this IDbConnection con, Action<SqlExpression<TEntity>> sqlBuilder, IDbTransaction? transaction = null)
    {
        var sqlExpression = CreateSqlExpression<TEntity>(GetSqlBuilder(con));
        sqlBuilder(sqlExpression);
        var sql = sqlExpression.ToSql(out var parameters);
        LogReceived?.Invoke(sql);
        return await con.ExecuteScalarAsync<TResult>(sql, parameters, transaction);
    }
}
