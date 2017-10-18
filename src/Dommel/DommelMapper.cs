using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dommel
{
    /// <summary>
    /// Simple CRUD operations for Dapper.
    /// </summary>
    public static class DommelMapper
    {
        private static readonly Dictionary<string, ISqlBuilder> _sqlBuilders = new Dictionary<string, ISqlBuilder>
                                                                                {
                                                                                    { "sqlconnection", new SqlServerSqlBuilder() },
                                                                                    { "sqlceconnection", new SqlServerCeSqlBuilder() },
                                                                                    { "sqliteconnection", new SqliteSqlBuilder() },
                                                                                    { "npgsqlconnection", new PostgresSqlBuilder() },
                                                                                    { "mysqlconnection", new MySqlSqlBuilder() }
                                                                                };

        private static readonly ConcurrentDictionary<Type, string> _getQueryCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<Type, string> _getAllQueryCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<Type, string> _insertQueryCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<Type, string> _updateQueryCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<Type, string> _deleteQueryCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<Type, string> _deleteAllQueryCache = new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<Type, string> _getCountCache = new ConcurrentDictionary<Type, string>();

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
        public static Task<TEntity> GetAsync<TEntity>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TEntity : class
        {
            DynamicParameters parameters;
            var sql = BuildGetById(typeof(TEntity), id, out parameters);
            return connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters, transaction);
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

        public static Task<IEnumerable<TEntity>> FindAllAsync<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null )
        {
            DynamicParameters parameters;
            string splitOn;
            var sql = Dommel.DommelMapper.BuildQueryMultiple<TEntity>(predicate, out parameters, out splitOn);

            return connection.QueryAsync<TEntity>(sql, parameters, transaction);
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

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, TReturn>(this IDbConnection connection, object id, Func<T1, T2, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static async Task<TReturn> GetAsync<T1, T2, TReturn>(this IDbConnection connection, object id, Func<T1, T2, TReturn> map) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, TReturn>(this IDbConnection connection,
                                                       object id,
                                                       Func<T1, T2, T3, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static async Task<TReturn> GetAsync<T1, T2, T3, TReturn>(this IDbConnection connection,
                                                                        object id,
                                                                        Func<T1, T2, T3, TReturn> map) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, TReturn>(this IDbConnection connection,
                                                           object id,
                                                           Func<T1, T2, T3, T4, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static async Task<TReturn> GetAsync<T1, T2, T3, T4, TReturn>(this IDbConnection connection,
                                                                            object id,
                                                                            Func<T1, T2, T3, T4, TReturn> map) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, id)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection,
                                                               object id,
                                                               Func<T1, T2, T3, T4, T5, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static async Task<TReturn> GetAsync<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection,
                                                                                object id,
                                                                                Func<T1, T2, T3, T4, T5, TReturn> map) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, id)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection,
                                                                   object id,
                                                                   Func<T1, T2, T3, T4, T5, T6, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static async Task<TReturn> GetAsync<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection,
                                                                                    object id,
                                                                                    Func<T1, T2, T3, T4, T5, T6, TReturn> map) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, id)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="T7">The seventh type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection,
                                                                       object id,
                                                                       Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="T7">The seventh type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static async Task<TReturn> GetAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection,
                                                                                        object id,
                                                                                        Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, id)).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, TReturn>(this IDbConnection connection, Func<T1, T2, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, TReturn>(this IDbConnection connection, Func<T1, T2, TReturn> map, bool buffered = true)
        {
            return MultiMapAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, TReturn>(this IDbConnection connection, Func<T1, T2, T3, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, TReturn>(this IDbConnection connection, Func<T1, T2, T3, TReturn> map, bool buffered = true)
        {
            return MultiMapAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, TReturn> map, bool buffered = true)
        {
            return MultiMapAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, TReturn> map, bool buffered = true)
        {
            return MultiMapAsync<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, T6, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, T6, TReturn> map, bool buffered = true)
        {
            return MultiMapAsync<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="T7">The seventh type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/>
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="T7">The seventh type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map, bool buffered = true)
        {
            return MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, buffered: buffered);
        }

        private static IEnumerable<TReturn> MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(IDbConnection connection, Delegate map, object id = null, bool buffered = true)
        {
            var resultType = typeof(TReturn);
            var
                includeTypes = new[]
                               {
                                   typeof(T1),
                                   typeof(T2),
                                   typeof(T3),
                                   typeof(T4),
                                   typeof(T5),
                                   typeof(T6),
                                   typeof(T7)
                               }
                    .Where(t => t != typeof(DontMap))
                    .ToArray();

            DynamicParameters parameters;
            var sql = BuildMultiMapQuery(resultType, includeTypes, id, out parameters);

            switch (includeTypes.Length)
            {
                case 2:
                    return connection.Query(sql, (Func<T1, T2, TReturn>)map, parameters, buffered: buffered, splitOn: Resolvers.ForeignKeyColumn(typeof(T1), typeof(T2)));
                case 3:
                    return connection.Query(sql, (Func<T1, T2, T3, TReturn>)map, parameters, buffered: buffered);
                case 4:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, TReturn>)map, parameters, buffered: buffered);
                case 5:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, TReturn>)map, parameters, buffered: buffered);
                case 6:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, T6, TReturn>)map, parameters, buffered: buffered);
                case 7:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, T6, T7, TReturn>)map, parameters, buffered: buffered);
            }

            throw new InvalidOperationException($"Invalid amount of include types: {includeTypes.Length}.");
        }

        private static Task<IEnumerable<TReturn>> MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(IDbConnection connection, Delegate map, object id = null, bool buffered = true)
        {
            var resultType = typeof(TReturn);
            var
                includeTypes = new[]
                               {
                                   typeof(T1),
                                   typeof(T2),
                                   typeof(T3),
                                   typeof(T4),
                                   typeof(T5),
                                   typeof(T6),
                                   typeof(T7)
                               }
                    .Where(t => t != typeof(DontMap))
                    .ToArray();

            DynamicParameters parameters;
            var sql = BuildMultiMapQuery(resultType, includeTypes, id, out parameters);

            var splitIds = new List< string >( );
            for ( int i = 1; i < includeTypes.Length; i++ )
            {
                splitIds.Add( Resolvers.ForeignKeyColumn( typeof( T1 ), includeTypes[ i ] ) );
            }

            switch (includeTypes.Length)
            {
                case 2:
                    return connection.QueryAsync(sql, (Func<T1, T2, TReturn>)map, parameters, buffered: buffered, splitOn: string.Join(",", splitIds.ToArray()));
                case 3:
                    return connection.QueryAsync(sql, (Func<T1, T2, T3, TReturn>)map, parameters, buffered: buffered, splitOn: string.Join(",", splitIds.ToArray()));
                case 4:
                    return connection.QueryAsync(sql, (Func<T1, T2, T3, T4, TReturn>)map, parameters, buffered: buffered, splitOn: string.Join(",", splitIds.ToArray()));
                case 5:
                    return connection.QueryAsync(sql, (Func<T1, T2, T3, T4, T5, TReturn>)map, parameters, buffered: buffered, splitOn: string.Join(",", splitIds.ToArray()));
                case 6:
                    return connection.QueryAsync(sql, (Func<T1, T2, T3, T4, T5, T6, TReturn>)map, parameters, buffered: buffered, splitOn: string.Join(",", splitIds.ToArray()));
                case 7:
                    return connection.QueryAsync(sql, (Func<T1, T2, T3, T4, T5, T6, T7, TReturn>)map, parameters, buffered: buffered, splitOn: string.Join(",", splitIds.ToArray()));
            }

            throw new InvalidOperationException($"Invalid amount of include types: {includeTypes.Length}.");
        }

        private static string BuildMultiMapQuery(Type resultType, Type[] includeTypes, object id, out DynamicParameters parameters)
        {
            var resultTableName = Resolvers.Table(resultType);
            var resultTableKeyColumnName = Resolvers.Column(Resolvers.KeyProperty(resultType));

            var sql = $"select * from {resultTableName}";

            for (var i = 1; i < includeTypes.Length; i++)
            {
                // Determine the table to join with.
                var sourceType = includeTypes[0];
                var sourceTableName = Resolvers.Table(sourceType);

                // Determine the table name of the joined table.
                var includeType = includeTypes[i];
                var foreignKeyTableName = Resolvers.Table(includeType);

                // Determine the foreign key and the relationship type.
                ForeignKeyRelation relation;
                var foreignKeyProperty = Resolvers.ForeignKeyProperty(sourceType, includeType, out relation);
                var foreignKeyPropertyName = Resolvers.Column(foreignKeyProperty);

                // If the foreign key property is nullable, use a left-join.
                var joinType = Nullable.GetUnderlyingType(foreignKeyProperty.PropertyType) != null
                                   ? "left"
                                   : "inner";

                switch (relation)
                {
                    case ForeignKeyRelation.OneToOne:
                        // Determine the primary key of the foreign key table.
                        var foreignKeyTableKeyColumName = Resolvers.Column(Resolvers.KeyProperty(includeType));

                        sql += string.Format(" {0} join {1} on {2}.{3} = {1}.{4}",
                                             joinType,
                                             foreignKeyTableName,
                                             sourceTableName,
                                             foreignKeyPropertyName,
                                             foreignKeyTableKeyColumName);
                        break;

                    case ForeignKeyRelation.OneToMany:
                        // Determine the primary key of the source table.
                        var sourceKeyColumnName = Resolvers.Column(Resolvers.KeyProperty(sourceType));

                        sql += $" {joinType} join {foreignKeyTableName} on {sourceTableName}.{sourceKeyColumnName} = {foreignKeyTableName}.{foreignKeyPropertyName}";
                        break;

                    case ForeignKeyRelation.ManyToMany:
                        throw new NotImplementedException("Many-to-many relationships are not supported yet.");

                    default:
                        throw new NotImplementedException($"Foreign key relation type '{relation}' is not implemented.");
                }
            }

            parameters = null;
            if (id != null)
            {
                sql += string.Format(" where {0}.{1} = @{1}", resultTableName, resultTableKeyColumnName);

                parameters = new DynamicParameters();
                parameters.Add($"{resultTableKeyColumnName}", id);
            }

            return sql;
        }
        
        private static string GenerateMultiMapQuery<T1, T2, T3, T4, T5, T6, T7, TReturn>(Expression<Func<TReturn, bool>> predicate, out DynamicParameters parameters, out string splitOn, IEnumerable<IRelationshipMapping> relationships = null)
        {
            var resultType = typeof(TReturn);
            var
                includeTypes = new[]
                    {
                        typeof(T1),
                        typeof(T2),
                        typeof(T3),
                        typeof(T4),
                        typeof(T5),
                        typeof(T6),
                        typeof(T7)
                    }
                    .Where(t => t != typeof(DontMap))
                    .ToArray();


            var resultTableName = Resolvers.Table(resultType);
            var resultTableKeyColumnName = Resolvers.Column(Resolvers.KeyProperty(resultType));

            // need to make this querymultiple so it can do the joining on the one to one and querymultiple on the one to many relationships.
            var whereCondition = new SqlExpression< TReturn >( ).Where( predicate ).ToSql( out parameters );

            // Determine the table to join with.
            var sourceType = includeTypes[0];
            var sourceTableName = Resolvers.Table(sourceType);

            var fieldNames = new List<string>()
            {
                $"{Resolvers.Table(resultType)}.{string.Join($", {Resolvers.Table(resultType)}.", Resolvers.Properties(resultType).Select(Resolvers.Column))}"
            };

            var sqlArray = new List<string>()
            {
                string.Empty
            };

            var oneToManySql = $"SELECT {resultTableKeyColumnName} FROM {resultTableName} {whereCondition}";
            var splitIds = new List<string>();

            for (var i = 1; i < includeTypes.Length; i++)
            {
                // Determine the table name of the joined table.
                var includeType = includeTypes[i];
                var foreignKeyTableName = Resolvers.Table(includeType);

                // Determine the foreign key and the relationship type.
                
                var relationship = relationships?.SingleOrDefault(x => x.RelatedType == includeType);
                if (relationship == null)
                {
                    relationship = ForeignRelationship(typeof(T1), includeType);
                    if (relationship.Relation == ForeignKeyRelation.OneToMany)
                    {
                        relationship.JoinClause = $"{relationship.ForeignKeyPropertyName} IN ({oneToManySql})";   
                    }
                }
                
                

                switch ( relationship.Relation )
                {
                    case ForeignKeyRelation.OneToOne:

                        // Determine the primary key of the foreign key table.
                        fieldNames.Add($"{Resolvers.Table(includeType)}.{string.Join($", {Resolvers.Table(includeType)}.", Resolvers.Properties(includeType).Select(Resolvers.Column))}");
                        splitIds.Add(relationship.SplitId);

                        sqlArray[0] += $" {relationship.JoinType} join {Resolvers.Table(includeType)} ON {relationship.JoinClause}";
                        
                        break;

                    case ForeignKeyRelation.OneToMany:
                        // Determine the primary key of the source table.
                        sqlArray.Add( $"SELECT {Resolvers.Table(includeType)}.{string.Join($", {Resolvers.Table(includeType)}.", Resolvers.Properties(includeType).Select(Resolvers.Column))} from {foreignKeyTableName} WHERE {relationship.JoinClause}" );

                        break;

                    case ForeignKeyRelation.ManyToMany:
                        throw new NotImplementedException("Many-to-many relationships are not supported yet.");

                    default:
                        throw new NotImplementedException($"Foreign key relation type '{relationship.Relation}' is not implemented.");
                }
            }

            sqlArray[0] = $"SELECT {string.Join(", ", fieldNames)} from {resultTableName} {sqlArray[0]}";
            sqlArray[0] += whereCondition;

            splitOn = string.Join(",", splitIds.ToArray());

            return string.Join( "; ", sqlArray );
        }

        private static IRelationshipMapping ForeignRelationship(Type sourceType, Type relatedType)
        {
            ForeignKeyRelation relation;
            var foreignKeyProperty = Resolvers.ForeignKeyProperty(sourceType, relatedType, out relation);
            // If the foreign key property is nullable, use a left-join.
            var relationship = new RelationshipMapping()
            {
                RelatedType = relatedType,
                Relation = relation,
                ForeignKeyPropertyName = Resolvers.Column(foreignKeyProperty)
            };

            if (relationship.Relation == ForeignKeyRelation.OneToOne)
            {
                relationship.SplitId = Resolvers.ForeignKeyColumn(sourceType, relatedType);
                relationship.JoinType = Nullable.GetUnderlyingType(foreignKeyProperty.PropertyType) != null
                           ? "left"
                           : "inner";
                relationship.JoinClause = $"{Resolvers.Table(sourceType)}.{relationship.ForeignKeyPropertyName} = {Resolvers.Table(relatedType)}.{Resolvers.Column(Resolvers.KeyProperty(relatedType))}";
            }

            return relationship;
        }

        /// <summary>
        /// Selects all the entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TEntity"/> matching the specified
        /// <paramref name="predicate"/>.
        /// </returns>
        public static IEnumerable<TEntity> Select<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, bool buffered = true)
        {
            DynamicParameters parameters;
            var sql = BuildSelectSql(predicate, out parameters);
            return connection.Query<TEntity>(sql, parameters, buffered: buffered);
        }

        /// <summary>
        /// Selects all the entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TEntity"/> matching the specified
        /// <paramref name="predicate"/>.
        /// </returns>
        public static Task<IEnumerable<TEntity>> SelectAsync<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            DynamicParameters parameters;
            var sql = BuildSelectSql(predicate, out parameters);
            return connection.QueryAsync<TEntity>(sql, parameters, transaction: transaction);
        }

        private static string BuildSelectSql<TEntity>(Expression<Func<TEntity, bool>> predicate, out DynamicParameters parameters)
        {
            var type = typeof(TEntity);
            string sql;
            if (!_getAllQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = $"select * from {tableName}";
                _getAllQueryCache.TryAdd(type, sql);
            }

            sql += new SqlExpression<TEntity>()
                .Where(predicate)
                .ToSql(out parameters);
            return sql;
        }

        /// <summary>
        /// Selects the first entity matching the specified predicate, or a default value if no entity matched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <returns>
        /// A instance of type <typeparamref name="TEntity"/> matching the specified
        /// <paramref name="predicate"/>.
        /// </returns>
        public static TEntity FirstOrDefault<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            DynamicParameters parameters;
            var sql = BuildSelectSql(predicate, out parameters);
            return connection.QueryFirstOrDefault<TEntity>(sql, parameters);
        }

        /// <summary>
        /// Selects the first entity matching the specified predicate, or a default value if no entity matched.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <returns>
        /// A instance of type <typeparamref name="TEntity"/> matching the specified
        /// <paramref name="predicate"/>.
        /// </returns>
        public static Task<TEntity> FirstOrDefaultAsync<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            DynamicParameters parameters;
            var sql = BuildSelectSql(predicate, out parameters);
            return connection.QueryFirstOrDefaultAsync<TEntity>(sql, parameters);
        }

        /// <summary>
        /// Count the entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <returns>
        /// Count of entities of type <typeparamref name="TEntity"/> matching the specified
        /// <paramref name="predicate"/>.
        /// </returns>
        public static long Count<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            DynamicParameters parameters;
            var sql = BuildCountSql(predicate, out parameters);
            return connection.ExecuteScalar<long>(sql, parameters);
        }

        /// <summary>
        /// Selects all the entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TEntity"/> matching the specified
        /// <paramref name="predicate"/>.
        /// </returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            DynamicParameters parameters;
            var sql = BuildCountSql(predicate, out parameters);
            return connection.ExecuteScalarAsync<long>(sql, parameters);
        }

        private static string BuildCountSql<TEntity>(Expression<Func<TEntity, bool>> predicate, out DynamicParameters parameters)
        {
            var type = typeof(TEntity);
            string sql;
            if (!_getCountCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = $"select count(*) from {tableName}";
                _getCountCache.TryAdd(type, sql);
            }

            sql += new SqlExpression<TEntity>()
                .Where(predicate)
                .ToSql(out parameters);
            return sql;
        }

        public static string BuildQueryMultiple<TEntity>(Expression<Func<TEntity, bool>> predicate,  out DynamicParameters parameters, out string splitOn)
        {
            return GenerateMultiMapQuery<TEntity, DontMap, DontMap, DontMap, DontMap, DontMap, DontMap, TEntity>(predicate, out parameters, out splitOn);
        }

        public static string BuildQueryMultiple< T1, T2, TReturn >( Expression<Func<TReturn, bool>> predicate, out DynamicParameters parameters, out string splitOn, IEnumerable<IRelationshipMapping> relationships = null)
        {
            // In this situation T1 is TReturn.
            return GenerateMultiMapQuery<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(predicate, out parameters, out splitOn, relationships);
        }

        /// <summary>
        /// Builds Multiple query related to T1 into sql and parameters for QueryMultiple execution.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="parameters"></param>
        /// <param name="splitOn"></param>
        /// <returns></returns>
        public static string BuildQueryMultiple<T1, T2, T3, TReturn>(Expression<Func<TReturn, bool>> predicate, out DynamicParameters parameters, out string splitOn, IEnumerable<IRelationshipMapping> relationships = null)
        {
            // In this situation T1 is TReturn.
            return GenerateMultiMapQuery<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(predicate, out parameters, out splitOn, relationships);
        }

        public static string BuildQueryMultiple<T1, T2, T3, T4, TReturn>(Expression<Func<TReturn, bool>> predicate, out DynamicParameters parameters, out string splitOn, IEnumerable<IRelationshipMapping> relationships = null)
        {
            // In this situation T1 is TReturn.
            return GenerateMultiMapQuery<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(predicate, out parameters, out splitOn, relationships);
        }

        public static string BuildQueryMultiple<T1, T2, T3, T4, T5, TReturn>(Expression<Func<TReturn, bool>> predicate, out DynamicParameters parameters, out string splitOn, IEnumerable<IRelationshipMapping> relationships = null)
        {
            // In this situation T1 is TReturn.
            return GenerateMultiMapQuery<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(predicate, out parameters, out splitOn, relationships);
        }

        public static string BuildQueryMultiple<T1, T2, T3, T4, T5, T6, TReturn>(Expression<Func<TReturn, bool>> predicate, out DynamicParameters parameters, out string splitOn, IEnumerable<IRelationshipMapping> relationships = null)
        {
            // In this situation T1 is TReturn.
            return GenerateMultiMapQuery<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(predicate, out parameters, out splitOn, relationships);
        }

        public static string BuildQueryMultiple<T1, T2, T3, T4, T5, T6, T7, TReturn>(Expression<Func<TReturn, bool>> predicate, out DynamicParameters parameters, out string splitOn, IEnumerable<IRelationshipMapping> relationships = null)
        {
            // In this situation T1 is TReturn.
            return GenerateMultiMapQuery<T1, T2, T3, T4, T5, T6, T7, TReturn>(predicate, out parameters, out splitOn, relationships);
        }

        /// <summary>
        /// Selects the paginated entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <param name="sortFields">Array of fields to be used in sorting.</param>
        /// <param name="pageNo">The page number of the results.</param>
        /// <param name="pageSize">The page size of the results.</param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TEntity"/> matching the specified
        /// <paramref name="predicate"/>.
        /// </returns>
        public static IEnumerable<TEntity> Query<TEntity>(this IDbConnection connection,
            Expression<Func<TEntity, bool>> predicate,
            IList<SortField> sortFields = null,
            int? pageNo = null, int? pageSize = null)
        {
            var tableName = Resolvers.Table(typeof(TEntity));
            var orderBySql = BuildOrderSql<TEntity>(sortFields);

            var sql = BuildSelectSql(predicate, out DynamicParameters parameters);
            if (pageNo == null || pageSize == null)
            {
                sql = $"{sql} {orderBySql}";
            }
            else
            {
                sql = BuildPaginationSql(connection, tableName, sql, orderBySql, true, pageNo.Value, pageSize.Value);
            }

            return connection.Query<TEntity>(sql, (object)parameters);
        }
        private static string BuildOrderSql<TEntity>(IList<SortField> sortFields = null)
        {
            new SqlExpression<TEntity>().BuildOrderSql(sortFields).ToSql(out var orderBy, out var paginationOffset);
            return orderBy;
        }

        private static string BuildPaginationSql(IDbConnection connection, string tableName, string sql,
            string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var builder = GetBuilder(connection);
            return builder.BuildPagination(tableName, sql, orderBySql, orderByAsc, pageNo, pageSize);
        }

        

        /// <summary>
        /// Inserts the specified entity into the database and returns the id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The id of the inserted entity.</returns>
        public static object Insert<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildInsertQuery(connection, typeof(TEntity));
            return connection.ExecuteScalar(sql, entity, transaction);
        }

        /// <summary>
        /// Inserts the specified entity into the database and returns the id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The id of the inserted entity.</returns>
        public static Task<object> InsertAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildInsertQuery(connection, typeof(TEntity));
            return connection.ExecuteScalarAsync(sql, entity, transaction);
        }

        private static string BuildInsertQuery(IDbConnection connection, Type type)
        {
            string sql;
            if (!_insertQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);

                bool isIdentity;
                var keyProperty = Resolvers.KeyProperty(type, out isIdentity);

                var typeProperties = new List<PropertyInfo>();
                foreach (var typeProperty in Resolvers.Properties(type))
                {
                    if (typeProperty == keyProperty)
                    {
                        if (isIdentity)
                        {
                            // Skip key properties marked as an identity column.
                            continue;
                        }
                    }

                    if (typeProperty.GetSetMethod() != null)
                    {
                        typeProperties.Add(typeProperty);
                    }
                }

                var columnNames = typeProperties.Select(Resolvers.Column).ToArray();
                var paramNames = typeProperties.Select(p => "@" + p.Name).ToArray();

                var builder = GetBuilder(connection);
                sql = builder.BuildInsert(tableName, columnNames, paramNames, keyProperty);

                _insertQueryCache.TryAdd(type, sql);
            }

            return sql;
        }

        /// <summary>
        /// Updates the values of the specified entity in the database.
        /// The return value indicates whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity in the database.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>A value indicating whether the update operation succeeded.</returns>
        public static bool Update<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null)
        {
            var sql = BuildUpdateQuery(typeof(TEntity));
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
        /// <returns>A value indicating whether the update operation succeeded.</returns>
        public static async Task<bool> UpdateAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null)
        {
            var sql = BuildUpdateQuery(typeof(TEntity));
            return await connection.ExecuteAsync(sql, entity, transaction) > 0;
        }

        private static string BuildUpdateQuery(Type type)
        {
            string sql;
            if (!_updateQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);

                // Use all properties which are settable.
                var typeProperties = Resolvers.Properties(type)
                                              .Where(p => p != keyProperty)
                                              .Where(p => p.GetSetMethod() != null)
                                              .ToArray();

                var columnNames = typeProperties.Select(p => $"{Resolvers.Column(p)} = @{p.Name}").ToArray();

                sql = $"update {tableName} set {string.Join(", ", columnNames)} where {Resolvers.Column(keyProperty)} = @{keyProperty.Name}";

                _updateQueryCache.TryAdd(type, sql);
            }

            return sql;
        }

        /// <summary>
        /// Deletes the specified entity from the database.
        /// Returns a value indicating whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be deleted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>A value indicating whether the delete operation succeeded.</returns>
        public static bool Delete<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null)
        {
            var sql = BuildDeleteQuery(typeof(TEntity));
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
        /// <returns>A value indicating whether the delete operation succeeded.</returns>
        public static async Task<bool> DeleteAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null)
        {
            var sql = BuildDeleteQuery(typeof(TEntity));
            return await connection.ExecuteAsync(sql, entity, transaction) > 0;
        }

        private static string BuildDeleteQuery(Type type)
        {
            string sql;
            if (!_deleteQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var keyColumnName = Resolvers.Column(keyProperty);

                sql = $"delete from {tableName} where {keyColumnName} = @{keyProperty.Name}";

                _deleteQueryCache.TryAdd(type, sql);
            }

            return sql;
        }

        /// <summary>
        /// Deletes all entities of type <typeparamref name="TEntity"/> matching the specified predicate from the database.
        /// Returns a value indicating whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter which entities are deleted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>A value indicating whether the delete operation succeeded.</returns>
        public static bool DeleteMultiple<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            DynamicParameters parameters;
            var sql = BuildDeleteMultipleQuery(predicate, out parameters);
            return connection.Execute(sql, parameters, transaction) > 0;
        }

        /// <summary>
        /// Deletes all entities of type <typeparamref name="TEntity"/> matching the specified predicate from the database.
        /// Returns a value indicating whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter which entities are deleted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>A value indicating whether the delete operation succeeded.</returns>
        public static async Task<bool> DeleteMultipleAsync<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            DynamicParameters parameters;
            var sql = BuildDeleteMultipleQuery(predicate, out parameters);
            return await connection.ExecuteAsync(sql, parameters, transaction) > 0;
        }

        private static string BuildDeleteMultipleQuery<TEntity>(Expression<Func<TEntity, bool>> predicate, out DynamicParameters parameters)
        {
            var type = typeof(TEntity);
            string sql;
            if (!_deleteAllQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = $"delete from {tableName}";
                _deleteAllQueryCache.TryAdd(type, sql);
            }

            sql += new SqlExpression<TEntity>()
                .Where(predicate)
                .ToSql(out parameters);
            return sql;
        }

        /// <summary>
        /// Deletes all entities of type <typeparamref name="TEntity"/> from the database.
        /// Returns a value indicating whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>A value indicating whether the delete operation succeeded.</returns>
        public static bool DeleteAll<TEntity>(this IDbConnection connection, IDbTransaction transaction = null)
        {
            var sql = BuildDeleteAllQuery(typeof(TEntity));
            return connection.Execute(sql, transaction: transaction) > 0;
        }

        /// <summary>
        /// Deletes all entities of type <typeparamref name="TEntity"/> from the database.
        /// Returns a value indicating whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>A value indicating whether the delete operation succeeded.</returns>
        public static async Task<bool> DeleteAllAsync<TEntity>(this IDbConnection connection, IDbTransaction transaction = null)
        {
            var sql = BuildDeleteAllQuery(typeof(TEntity));
            return await connection.ExecuteAsync(sql, transaction: transaction) > 0;
        }

        private static string BuildDeleteAllQuery(Type type)
        {
            string sql;
            if (!_deleteAllQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = $"delete from {tableName}";
                _deleteAllQueryCache.TryAdd(type, sql);
            }

            return sql;
        }

        
        

        #region Property resolving
        internal static IPropertyResolver PropertyResolver { get; private set; } = new DefaultPropertyResolver();

        

        /// <summary>
        /// Sets the <see cref="DommelMapper.IPropertyResolver"/> implementation for resolving key of entities.
        /// </summary>
        /// <param name="propertyResolver">An instance of <see cref="DommelMapper.IPropertyResolver"/>.</param>
        public static void SetPropertyResolver(IPropertyResolver propertyResolver)
        {
            PropertyResolver = propertyResolver;
        }

        #endregion

        #region Key property resolving
        internal static IKeyPropertyResolver KeyPropertyResolver { get; private set; } = new DefaultKeyPropertyResolver();

        /// <summary>
        /// Sets the <see cref="DommelMapper.IKeyPropertyResolver"/> implementation for resolving key properties of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="DommelMapper.IKeyPropertyResolver"/>.</param>
        public static void SetKeyPropertyResolver(IKeyPropertyResolver resolver)
        {
            KeyPropertyResolver = resolver;
        }
        
        #endregion

        #region Foreign key property resolving
        

        internal static IForeignKeyPropertyResolver ForeignKeyPropertyResolver { get; private set; } = new DefaultForeignKeyPropertyResolver();

        /// <summary>
        /// Sets the <see cref="T:DommelMapper.IForeignKeyPropertyResolver"/> implementation for resolving foreign key properties.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="T:DommelMapper.IForeignKeyPropertyResolver"/>.</param>
        public static void SetForeignKeyPropertyResolver(IForeignKeyPropertyResolver resolver)
        {
            ForeignKeyPropertyResolver = resolver;
        }

        

        
        #endregion

        #region Table name resolving
        internal static ITableNameResolver TableNameResolver { get; private set; } = new DefaultTableNameResolver();

        /// <summary>
        /// Sets the <see cref="T:Dommel.ITableNameResolver"/> implementation for resolving table names for entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="T:Dommel.ITableNameResolver"/>.</param>
        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            TableNameResolver = resolver;
        }

        

        
        #endregion

        #region Column name resolving
        internal static IColumnNameResolver ColumnNameResolver { get; private set; } = new DefaultColumnNameResolver();

        /// <summary>
        /// Sets the <see cref="T:Dommel.IColumnNameResolver"/> implementation for resolving column names.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="T:Dommel.IColumnNameResolver"/>.</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver)
        {
            ColumnNameResolver = resolver;
        }

        

        
        #endregion

        #region Sql builders
        /// <summary>
        /// Adds a custom implementation of <see cref="T:DommelMapper.ISqlBuilder"/>
        /// for the specified ADO.NET connection type.
        /// </summary>
        /// <param name="connectionType">
        /// The ADO.NET conncetion type to use the <paramref name="builder"/> with.
        /// Example: <c>typeof(SqlConnection)</c>.
        /// </param>
        /// <param name="builder">An implementation of the <see cref="T:DommelMapper.ISqlBuilder interface"/>.</param>
        public static void AddSqlBuilder(Type connectionType, ISqlBuilder builder)
        {
            _sqlBuilders[connectionType.Name.ToLower()] = builder;
        }

        private static ISqlBuilder GetBuilder(IDbConnection connection)
        {
            var connectionName = connection.GetType().Name.ToLower();
            ISqlBuilder builder;
            return _sqlBuilders.TryGetValue(connectionName, out builder) ? builder : new SqlServerSqlBuilder();
        }

        

        

        
        #endregion
    }
}