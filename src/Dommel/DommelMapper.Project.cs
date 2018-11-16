using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;

namespace Dommel
{
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
        public static TEntity Project<TEntity>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildProjectById(connection, typeof(TEntity), id, out var parameters);
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
        public static Task<TEntity> ProjectAsync<TEntity>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildProjectById(connection, typeof(TEntity), id, out var parameters);
            LogQuery<TEntity>(sql);
            return connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, transaction);
        }

        private static string BuildProjectById(IDbConnection connection, Type type, object id, out DynamicParameters parameters)
        {
            var cacheKey = new QueryCacheKey(QueryCacheType.Get, connection, type);
            if (!QueryCache.TryGetValue(cacheKey, out var sql))
            {
                var keyProperty = Resolvers.KeyProperty(type);
                var keyColumnName = Resolvers.Column(keyProperty, connection);

                sql = BuildProjectAllQuery(connection, type);
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
        public static IEnumerable<TEntity> ProjectAll<TEntity>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true) where TEntity : class
        {
            var sql = BuildProjectAllQuery(connection, typeof(TEntity));
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
        public static Task<IEnumerable<TEntity>> ProjectAllAsync<TEntity>(this IDbConnection connection, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildProjectAllQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            return connection.QueryAsync<TEntity>(sql, transaction: transaction);
        }

        private static string BuildProjectAllQuery(IDbConnection connection, Type type)
        {
            var cacheKey = new QueryCacheKey(QueryCacheType.GetAll, connection, type);
            if (!QueryCache.TryGetValue(cacheKey, out var sql))
            {
                var tableName = Resolvers.Table(type, connection);
                var keyProperty = Resolvers.KeyProperty(type);
                var properties = Resolvers.Properties(type)
                                              .Where(p => p != keyProperty)
                                              .Where(p => p.GetSetMethod() != null)
                                              .ToArray();

                var projectedCols = string.Join(", ", properties.Select(p => Resolvers.Column(p, connection)));

                sql = $"select ({projectedCols}) from {tableName}";
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
        public static IEnumerable<TEntity> ProjectPaged<TEntity>(this IDbConnection connection, int pageNumber, int pageSize, IDbTransaction transaction = null, bool buffered = true) where TEntity : class
        {
            var sql = BuildProjectPagedQuery(connection, typeof(TEntity), pageNumber, pageSize);
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
        public static Task<IEnumerable<TEntity>> ProjectPagedAsync<TEntity>(this IDbConnection connection, int pageNumber, int pageSize, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildProjectPagedQuery(connection, typeof(TEntity), pageNumber, pageSize);
            LogQuery<TEntity>(sql);
            return connection.QueryAsync<TEntity>(sql, transaction: transaction);
        }

        private static string BuildProjectPagedQuery(IDbConnection connection, Type type, int pageNumber, int pageSize)
        {
            // Start with the select query part.
            var sql = BuildProjectAllQuery(connection, type);

            // Append the paging part including the order by.
            var orderBy = "order by " + Resolvers.Column(Resolvers.KeyProperty(type), connection);
            sql += GetSqlBuilder(connection).BuildPaging(orderBy, pageNumber, pageSize);
            return sql;
        }
    }
}
