using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;

namespace Dommel
{
    public static partial class DommelMapper
    {
        private static readonly ConcurrentDictionary<Type, string> _getQueryCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<Type, string> _getAllQueryCache = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static TEntity Get<TEntity>(this IDbConnection connection, object id) where TEntity : class
        {
            DynamicParameters parameters;
            var sql = BuildGetById(typeof(TEntity), id, out parameters);
            return connection.QueryFirstOrDefault<TEntity>(sql, parameters);
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static Task<TEntity> GetAsync<TEntity>(this IDbConnection connection, object id) where TEntity : class
        {
            DynamicParameters parameters;
            var sql = BuildGetById(typeof(TEntity), id, out parameters);
            return connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters);
        }

        private static string BuildGetById(Type type, object id, out DynamicParameters parameters)
        {
            string sql;
            if (!_getQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var keyColumnName = Resolvers.Column(keyProperty);

                sql = $"select * from {tableName} where {keyColumnName} = @Id";
                _getQueryCache.TryAdd(type, sql);
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
        /// <returns>A collection of entities of type <typeparamref name="TEntity"/>.</returns>
        public static IEnumerable<TEntity> GetAll<TEntity>(this IDbConnection connection, bool buffered = true) where TEntity : class
        {
            var sql = BuildGetAllQuery(typeof(TEntity));
            return connection.Query<TEntity>(sql, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <returns>A collection of entities of type <typeparamref name="TEntity"/>.</returns>
        public static Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(this IDbConnection connection) where TEntity : class
        {
            var sql = BuildGetAllQuery(typeof(TEntity));
            return connection.QueryAsync<TEntity>(sql);
        }

        private static string BuildGetAllQuery(Type type)
        {
            string sql;
            if (!_getAllQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = $"select * from {tableName}";
                _getAllQueryCache.TryAdd(type, sql);
            }

            return sql;
        }
    }
}
