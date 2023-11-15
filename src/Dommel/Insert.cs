using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Inserts the specified entity into the database and returns the ID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entity">The entity to be inserted.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The ID of the inserted entity.</returns>
    public static object Insert<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null)
        where TEntity : class
    {
        var sql = BuildInsertQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalar(sql, entity, transaction)!;
    }

    /// <summary>
    /// Inserts the specified entity into the database and returns the ID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entity">The entity to be inserted.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The ID of the inserted entity.</returns>
    public static Task<object> InsertAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var sql = BuildInsertQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.ExecuteScalarAsync(new CommandDefinition(sql, entity, transaction: transaction, cancellationToken: cancellationToken))!;
    }

    /// <summary>
    /// Inserts the specified collection of entities into the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entities">The entities to be inserted.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    public static void InsertAll<TEntity>(this IDbConnection connection, IEnumerable<TEntity> entities, IDbTransaction? transaction = null)
        where TEntity : class
    {
        var sql = BuildInsertQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        connection.Execute(sql, entities, transaction);
    }

    /// <summary>
    /// Inserts the specified collection of entities into the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entities">The entities to be inserted.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    public static Task InsertAllAsync<TEntity>(this IDbConnection connection, IEnumerable<TEntity> entities, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var sql = BuildInsertQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.ExecuteAsync(new CommandDefinition(sql, entities, transaction: transaction, cancellationToken: cancellationToken));
    }

    internal static string BuildInsertQuery(ISqlBuilder sqlBuilder, Type type)
    {
        var cacheKey = new QueryCacheKey(QueryCacheType.Insert, sqlBuilder, type);
        if (!QueryCache.TryGetValue(cacheKey, out var sql))
        {
            var tableName = Resolvers.Table(type, sqlBuilder);

            // Use all non-key and non-generated properties for inserts
            var keyProperties = Resolvers.KeyProperties(type);
            var typeProperties = Resolvers.Properties(type)
                .Where(x => !x.IsGenerated)
                .Select(x => x.Property)
                .Except(keyProperties.Where(p => p.IsGenerated).Select(p => p.Property));

            var columnNames = typeProperties.Select(p => Resolvers.Column(p, sqlBuilder, false)).ToArray();
            var paramNames = typeProperties.Select(p => sqlBuilder.PrefixParameter(p.Name)).ToArray();

            sql = sqlBuilder.BuildInsert(type, tableName, columnNames, paramNames);

            QueryCache.TryAdd(cacheKey, sql);
        }

        return sql;
    }
}
