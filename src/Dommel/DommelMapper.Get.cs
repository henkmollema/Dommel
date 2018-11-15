using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public static TEntity Get<TEntity>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildGetById(connection, typeof(TEntity), id, out var parameters);
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
        public static Task<TEntity> GetAsync<TEntity>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildGetById(connection, typeof(TEntity), id, out var parameters);
            LogQuery<TEntity>(sql);
            return connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, transaction);
        }

        private static string BuildGetById(IDbConnection connection, Type type, object id, out DynamicParameters parameters)
        {
            var cacheKey = new QueryCacheKey(QueryCacheType.Get, connection, type);
            if (!QueryCache.TryGetValue(cacheKey, out var sql))
            {
                var tableName = Resolvers.Table(type, connection);
                var keyProperty = Resolvers.KeyProperty(type);
                var keyColumnName = Resolvers.Column(keyProperty, connection);

                sql = $"select * from {tableName} where {keyColumnName} = @Id";
                QueryCache.TryAdd(cacheKey, sql);
            }

            parameters = new DynamicParameters();
            parameters.Add("Id", id);

            return sql;
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="ids">The id of the entity in the database.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static TEntity Get<TEntity>(this IDbConnection connection, params object[] ids) where TEntity : class
            => Get<TEntity>(connection, ids, transaction: null);

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="ids">The id of the entity in the database.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static TEntity Get<TEntity>(this IDbConnection connection, object[] ids, IDbTransaction transaction = null) where TEntity : class
        {
            if (ids.Length == 1)
            {
                return Get<TEntity>(connection, ids[0], transaction);
            }

            var sql = BuildGetByIds(connection, typeof(TEntity), ids, out var parameters);
            LogQuery<TEntity>(sql);
            return connection.QueryFirstOrDefault<TEntity>(sql, parameters);
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="ids">The id of the entity in the database.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static Task<TEntity> GetAsync<TEntity>(this IDbConnection connection, params object[] ids) where TEntity : class
            => GetAsync<TEntity>(connection, ids, transaction: null);

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="ids">The id of the entity in the database.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static Task<TEntity> GetAsync<TEntity>(this IDbConnection connection, object[] ids, IDbTransaction transaction = null) where TEntity : class
        {
            if (ids.Length == 1)
            {
                return GetAsync<TEntity>(connection, ids[0], transaction);
            }

            var sql = BuildGetByIds(connection, typeof(TEntity), ids, out var parameters);
            LogQuery<TEntity>(sql);
            return connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters);
        }

        private static string BuildGetByIds(IDbConnection connection, Type type, object[] ids, out DynamicParameters parameters)
        {
            var cacheKey = new QueryCacheKey(QueryCacheType.GetByMultipleIds, connection, type);
            if (!QueryCache.TryGetValue(cacheKey, out var sql))
            {
                var tableName = Resolvers.Table(type, connection);
                var keyProperties = Resolvers.KeyProperties(type);
                var keyColumnsNames = keyProperties.Select(p => Resolvers.Column(p, connection)).ToArray();
                if (keyColumnsNames.Length != ids.Length)
                {
                    throw new InvalidOperationException($"Number of key columns ({keyColumnsNames.Length}) of type {type.Name} does not match with the number of specified IDs ({ids.Length}).");
                }

                var sb = new StringBuilder("select * from ").Append(tableName).Append(" where");
                var i = 0;
                foreach (var keyColumnName in keyColumnsNames)
                {
                    if (i != 0)
                    {
                        sb.Append(" and");
                    }

                    sb.Append(" ").Append(keyColumnName).Append(" = @Id").Append(i);
                    i++;
                }

                sql = sb.ToString();
                QueryCache.TryAdd(cacheKey, sql);
            }

            parameters = new DynamicParameters();
            for (var i = 0; i < ids.Length; i++)
            {
                parameters.Add("Id" + i, ids[i]);
            }

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
        public static IEnumerable<TEntity> GetAll<TEntity>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true) where TEntity : class
        {
            var sql = BuildGetAllQuery(connection, typeof(TEntity));
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
        public static Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this IDbConnection connection, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildGetAllQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            return connection.QueryAsync<TEntity>(sql, transaction: transaction);
        }

        private static string BuildGetAllQuery(IDbConnection connection, Type type)
        {
            var cacheKey = new QueryCacheKey(QueryCacheType.GetAll, connection, type);
            if (!QueryCache.TryGetValue(cacheKey, out var sql))
            {
                sql = "select * from " + Resolvers.Table(type, connection);
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
        public static IEnumerable<TEntity> GetPaged<TEntity>(this IDbConnection connection, int pageNumber, int pageSize, IDbTransaction transaction = null, bool buffered = true) where TEntity : class
        {
            var sql = BuildPagedQuery(connection, typeof(TEntity), pageNumber, pageSize);
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
        public static Task<IEnumerable<TEntity>> GetPagedAsync<TEntity>(this IDbConnection connection, int pageNumber, int pageSize, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildPagedQuery(connection, typeof(TEntity), pageNumber, pageSize);
            LogQuery<TEntity>(sql);
            return connection.QueryAsync<TEntity>(sql, transaction: transaction);
        }

        private static string BuildPagedQuery(IDbConnection connection, Type type, int pageNumber, int pageSize)
        {
            // Start with the select query part.
            var sql = BuildGetAllQuery(connection, type);

            // Append the paging part including the order by.
            var orderBy = "order by " + Resolvers.Column(Resolvers.KeyProperty(type), connection);
            sql += GetSqlBuilder(connection).BuildPaging(orderBy, pageNumber, pageSize);
            return sql;
        }
    }
}
