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
    /// Updates the values of the specified entity in the database.
    /// The return value indicates whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entity">The entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>A value indicating whether the update operation succeeded.</returns>
    public static bool Update<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null)
    {
        var sql = BuildUpdateQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.Execute(sql, entity, transaction) > 0;
    }

    /// <summary>
    /// Updates the values of the specified entity in the database.
    /// The return value indicates whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entity">The entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>A value indicating whether the update operation succeeded.</returns>
    public static async Task<bool> UpdateAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var sql = BuildUpdateQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return await connection.ExecuteAsync(new CommandDefinition(sql, entity, transaction: transaction, cancellationToken: cancellationToken)) > 0;
    }

    /// <summary>
    /// Updates all entities of type <typeparamref name="TEntity"/> matching the specified
    /// <paramref name="predicate"/>, assigning the values specified by <paramref name="set"/>.
    /// Returns the number of rows affected.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="set">
    /// An expression specifying the properties to update and their values, e.g.
    /// <c>update => update.SetProperty(p => p.Name, "foo").SetProperty(p => p.Stock, p => p.Stock - 1)</c>.
    /// A value selector may reference columns of the entity and use the <c>+ - * / %</c> operators
    /// with a constant operand; column-to-column arithmetic is not supported.
    /// </param>
    /// <param name="predicate">A predicate to filter which entities are updated.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The number of rows affected.</returns>
    public static int UpdateMultiple<TEntity>(
        this IDbConnection connection,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> set,
        Expression<Func<TEntity, bool>> predicate,
        IDbTransaction? transaction = null)
    {
        var sql = BuildUpdateMultipleQuery<TEntity>(GetSqlBuilder(connection), set, predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return connection.Execute(sql, parameters, transaction);
    }

    /// <summary>
    /// Updates all entities of type <typeparamref name="TEntity"/> matching the specified
    /// <paramref name="predicate"/>, assigning the values specified by <paramref name="set"/>.
    /// Returns the number of rows affected.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="set">
    /// An expression specifying the properties to update and their values, e.g.
    /// <c>update => update.SetProperty(p => p.Name, "foo").SetProperty(p => p.Stock, p => p.Stock - 1)</c>.
    /// A value selector may reference columns of the entity and use the <c>+ - * / %</c> operators
    /// with a constant operand; column-to-column arithmetic is not supported.
    /// </param>
    /// <param name="predicate">A predicate to filter which entities are updated.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The number of rows affected.</returns>
    public static async Task<int> UpdateMultipleAsync<TEntity>(
        this IDbConnection connection,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> set,
        Expression<Func<TEntity, bool>> predicate,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        var sql = BuildUpdateMultipleQuery<TEntity>(GetSqlBuilder(connection), set, predicate, out var parameters);
        LogQuery<TEntity>(sql);
        return await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction: transaction, cancellationToken: cancellationToken));
    }

    internal static string BuildUpdateMultipleQuery<TEntity>(
        ISqlBuilder sqlBuilder,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> set,
        Expression<Func<TEntity, bool>> predicate,
        out DynamicParameters parameters)
    {
        var tableName = Resolvers.Table(typeof(TEntity), sqlBuilder);

        // Use a single SqlExpression instance for both the set-clause and the where-clause so
        // their auto-numbered parameters share numbering and never collide. Going through the
        // factory keeps the where-clause JSON-aware when Dommel.Json is used.
        var expression = CreateSqlExpression<TEntity>(sqlBuilder);
        var setClause = expression.ToSetClause(set);
        var whereSql = expression.Where(predicate).ToSql(out parameters);
        return $"update {tableName} set {setClause}{whereSql}";
    }

    internal static string BuildUpdateQuery(ISqlBuilder sqlBuilder, Type type)
    {
        var cacheKey = new QueryCacheKey(QueryCacheType.Update, sqlBuilder, type);
        if (!QueryCache.TryGetValue(cacheKey, out var sql))
        {
            var tableName = Resolvers.Table(type, sqlBuilder);

            // Use all non-key and non-generated properties for updates
            var keyProperties = Resolvers.KeyProperties(type);
            var typeProperties = Resolvers.Properties(type)
                .Where(x => !x.IsGenerated)
                .Select(x => x.Property)
                .Except(keyProperties.Where(p => p.IsGenerated).Select(p => p.Property));

            var columnNames = typeProperties.Select(p => $"{Resolvers.Column(p, sqlBuilder, false)} = {sqlBuilder.PrefixParameter(p.Name)}").ToArray();
            var whereClauses = keyProperties.Select(p => $"{Resolvers.Column(p.Property, sqlBuilder, false)} = {sqlBuilder.PrefixParameter(p.Property.Name)}");
            sql = $"update {tableName} set {string.Join(", ", columnNames)} where {string.Join(" and ", whereClauses)}";

            QueryCache.TryAdd(cacheKey, sql);
        }

        return sql;
    }
}
