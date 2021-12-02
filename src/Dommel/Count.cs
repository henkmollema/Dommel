using System;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Returns the number of all entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The number of entities matching the specified predicate.</returns>
    public static long Count<TEntity>(this IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildCountAllSql(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalar<long>(sql, transaction);
    }

    /// <summary>
    /// Returns the number of all entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The number of entities matching the specified predicate.</returns>
    public static Task<long> CountAsync<TEntity>(this IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildCountAllSql(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalarAsync<long>(sql, transaction);
    }

    /// <summary>
    /// Returns the number of entities matching the specified predicate.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The number of entities matching the specified predicate.</returns>
    public static long Count<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction? transaction = null)
    {
        var sql = BuildCountSql(GetSqlBuilder(connection), predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalar<long>(sql, parameters, transaction);
    }

    /// <summary>
    /// Returns the number of entities matching the specified predicate.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The number of entities matching the specified predicate.</returns>
    public static Task<long> CountAsync<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction? transaction = null)
    {
        var sql = BuildCountSql(GetSqlBuilder(connection), predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalarAsync<long>(sql, parameters, transaction);
    }

    internal static string BuildCountAllSql(ISqlBuilder sqlBuilder, Type type)
    {
        var cacheKey = new QueryCacheKey(QueryCacheType.Count, sqlBuilder, type);
        if (!QueryCache.TryGetValue(cacheKey, out var sql))
        {
            var tableName = Resolvers.Table(type, sqlBuilder);
            sql = $"select count(*) from {tableName}";
            QueryCache.TryAdd(cacheKey, sql);
        }

        return sql;
    }

    internal static string BuildCountSql<TEntity>(ISqlBuilder sqlBuilder, Expression<Func<TEntity, bool>> predicate, out DynamicParameters parameters)
    {
        var sql = BuildCountAllSql(sqlBuilder, typeof(TEntity));
        sql += CreateSqlExpression<TEntity>(sqlBuilder)
            .Where(predicate)
            .ToSql(out parameters);
        return sql;
    }
}
