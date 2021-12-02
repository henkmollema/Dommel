using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Deletes the specified entity from the database.
    /// Returns a value indicating whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entity">The entity to be deleted.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>A value indicating whether the delete operation succeeded.</returns>
    public static bool Delete<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null)
    {
        var sql = BuildDeleteQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.Execute(sql, entity, transaction) > 0;
    }

    /// <summary>
    /// Deletes the specified entity from the database.
    /// Returns a value indicating whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entity">The entity to be deleted.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>A value indicating whether the delete operation succeeded.</returns>
    public static async Task<bool> DeleteAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var sql = BuildDeleteQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return await connection.ExecuteAsync(new CommandDefinition(sql, entity, transaction: transaction, cancellationToken: cancellationToken)) > 0;
    }

    internal static string BuildDeleteQuery(ISqlBuilder sqlBuilder, Type type)
    {
        var cacheKey = new QueryCacheKey(QueryCacheType.Delete, sqlBuilder, type);
        if (!QueryCache.TryGetValue(cacheKey, out var sql))
        {
            var tableName = Resolvers.Table(type, sqlBuilder);
            var keyProperties = Resolvers.KeyProperties(type);
            var whereClauses = keyProperties.Select(p => $"{Resolvers.Column(p.Property, sqlBuilder, false)} = {sqlBuilder.PrefixParameter(p.Property.Name)}");

            sql = $"delete from {tableName} where {string.Join(" and ", whereClauses)}";

            QueryCache.TryAdd(cacheKey, sql);
        }

        return sql;
    }

    /// <summary>
    /// Deletes all entities of type <typeparamref name="TEntity"/> matching the specified predicate from the database.
    /// Returns the number of rows affected.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter which entities are deleted.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The number of rows affected.</returns>
    public static int DeleteMultiple<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction? transaction = null)
    {
        var sql = BuildDeleteMultipleQuery(GetSqlBuilder(connection), predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.Execute(sql, parameters, transaction);
    }

    /// <summary>
    /// Deletes all entities of type <typeparamref name="TEntity"/> matching the specified predicate from the database.
    /// Returns the number of rows affected.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter which entities are deleted.</param>
    /// <param name="transaction">Optional transaction for the command.</param> 
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> DeleteMultipleAsync<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var sql = BuildDeleteMultipleQuery(GetSqlBuilder(connection), predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction: transaction, cancellationToken: cancellationToken));
    }

    private static string BuildDeleteMultipleQuery<TEntity>(ISqlBuilder sqlBuilder, Expression<Func<TEntity, bool>> predicate, out DynamicParameters parameters)
    {
        // Build the delete all query
        var type = typeof(TEntity);
        var sql = BuildDeleteAllQuery(sqlBuilder, type);

        // Append the where statement
        sql += CreateSqlExpression<TEntity>(sqlBuilder)
            .Where(predicate)
            .ToSql(out parameters);
        return sql;
    }

    /// <summary>
    /// Deletes all entities of type <typeparamref name="TEntity"/> from the database.
    /// Returns the number of rows affected.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The number of rows affected.</returns>
    public static int DeleteAll<TEntity>(this IDbConnection connection, IDbTransaction? transaction = null)
    {
        var sql = BuildDeleteAllQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.Execute(sql, transaction: transaction);
    }

    /// <summary>
    /// Deletes all entities of type <typeparamref name="TEntity"/> from the database.
    /// Returns the number of rows affected.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> DeleteAllAsync<TEntity>(this IDbConnection connection, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var sql = BuildDeleteAllQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return await connection.ExecuteAsync(new CommandDefinition(sql, transaction: transaction, cancellationToken: cancellationToken));
    }

    internal static string BuildDeleteAllQuery(ISqlBuilder sqlBuilder, Type type)
    {
        var cacheKey = new QueryCacheKey(QueryCacheType.DeleteAll, sqlBuilder, type);
        if (!QueryCache.TryGetValue(cacheKey, out var sql))
        {
            var tableName = Resolvers.Table(type, sqlBuilder);
            sql = $"delete from {tableName}";
            QueryCache.TryAdd(cacheKey, sql);
        }

        return sql;
    }
}
