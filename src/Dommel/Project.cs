using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="id">The id of the entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id.</returns>
    public static TEntity? Project<TEntity>(this IDbConnection connection, object id, IDbTransaction? transaction = null) where TEntity : class
    {
        var sql = BuildProjectById(GetSqlBuilder(connection), typeof(TEntity), id, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.QueryFirstOrDefault<TEntity>(sql, parameters, transaction);
    }

    /// <summary>
    /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="id">The id of the entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id.</returns>
    public static async Task<TEntity?> ProjectAsync<TEntity>(this IDbConnection connection, object id, IDbTransaction? transaction = null) where TEntity : class
    {
        var sql = BuildProjectById(GetSqlBuilder(connection), typeof(TEntity), id, out var parameters);
        LogQuery<TEntity>(sql);
        return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, transaction);
    }

    internal static string BuildProjectById(ISqlBuilder sqlBuilder, Type type, object id, out DynamicParameters parameters)
    {
        var cacheKey = new QueryCacheKey(QueryCacheType.Project, sqlBuilder, type);
        if (!QueryCache.TryGetValue(cacheKey, out var sql))
        {
            var keyProperty = Resolvers.KeyProperties(type).Single().Property;
            var keyColumnName = Resolvers.Column(keyProperty, sqlBuilder);

            sql = BuildProjectAllQuery(sqlBuilder, type);
            sql += $" where {keyColumnName} = @Id";
            QueryCache.TryAdd(cacheKey, sql);
        }

        parameters = new DynamicParameters();
        parameters.Add("Id", id);

        return sql;
    }

    /// <summary>
    /// Retrieves all the entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>A collection of entities of type <typeparamref name="TEntity"/>.</returns>
    public static IEnumerable<TEntity> ProjectAll<TEntity>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true) where TEntity : class
    {
        var sql = BuildProjectAllQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.Query<TEntity>(sql, transaction: transaction, buffered: buffered);
    }

    /// <summary>
    /// Retrieves all the entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>A collection of entities of type <typeparamref name="TEntity"/>.</returns>
    public static Task<IEnumerable<TEntity>> ProjectAllAsync<TEntity>(this IDbConnection connection, IDbTransaction? transaction = null) where TEntity : class
    {
        var sql = BuildProjectAllQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.QueryAsync<TEntity>(sql, transaction: transaction);
    }

    internal static string BuildProjectAllQuery(ISqlBuilder sqlBuilder, Type type)
    {
        var cacheKey = new QueryCacheKey(QueryCacheType.ProjectAll, sqlBuilder, type);
        if (!QueryCache.TryGetValue(cacheKey, out var sql))
        {
            var tableName = Resolvers.Table(type, sqlBuilder);
            var properties = Resolvers.Properties(type)
                .Where(x => !x.IsGenerated)
                .Select(x => x.Property)
                .Where(p => p.GetSetMethod() != null)
                .Select(p => Resolvers.Column(p, sqlBuilder, false));

            sql = $"select {string.Join(", ", properties)} from {tableName}";
            QueryCache.TryAdd(cacheKey, sql);
        }

        return sql;
    }

    /// <summary>
    /// Retrieves a paged set of entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="pageNumber">The number of the page to fetch, starting at 1.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>A paged collection of entities of type <typeparamref name="TEntity"/>.</returns>
    public static IEnumerable<TEntity> ProjectPaged<TEntity>(this IDbConnection connection, int pageNumber, int pageSize, IDbTransaction? transaction = null, bool buffered = true) where TEntity : class
    {
        var sql = BuildProjectPagedQuery(GetSqlBuilder(connection), typeof(TEntity), pageNumber, pageSize);
        LogQuery<TEntity>(sql);
        return connection.Query<TEntity>(sql, transaction: transaction, buffered: buffered);
    }

    /// <summary>
    /// Retrieves a paged set of entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="pageNumber">The number of the page to fetch, starting at 1.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>A paged collection of entities of type <typeparamref name="TEntity"/>.</returns>
    public static Task<IEnumerable<TEntity>> ProjectPagedAsync<TEntity>(this IDbConnection connection, int pageNumber, int pageSize, IDbTransaction? transaction = null) where TEntity : class
    {
        var sql = BuildProjectPagedQuery(GetSqlBuilder(connection), typeof(TEntity), pageNumber, pageSize);
        LogQuery<TEntity>(sql);
        return connection.QueryAsync<TEntity>(sql, transaction: transaction);
    }

    internal static string BuildProjectPagedQuery(ISqlBuilder sqlBuilder, Type type, int pageNumber, int pageSize)
    {
        // Start with the select query part
        var sql = BuildProjectAllQuery(sqlBuilder, type);

        // Append the paging part including the order by
        var keyColumns = Resolvers.KeyProperties(type).Select(p => Resolvers.Column(p.Property, sqlBuilder));
        var orderBy = "order by " + string.Join(", ", keyColumns);
        sql += sqlBuilder.BuildPaging(orderBy, pageNumber, pageSize);
        return sql;
    }
}
