using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Dapper;

namespace Dommel
{
    /// <summary>
    /// Simple CRUD operations for Dapper.
    /// </summary>
    public static partial class DommelMapper
    {
        // Connection types mapping
        private static readonly IDictionary<string, ISqlBuilder> _sqlBuilders = new Dictionary<string, ISqlBuilder>
                                                                                {
                                                                                    { "sqlconnection", new SqlServerSqlBuilder() },
                                                                                    { "sqlceconnection", new SqlServerCeSqlBuilder() },
                                                                                    { "sqliteconnection", new SqliteSqlBuilder() },
                                                                                    { "npgsqlconnection", new PostgresSqlBuilder() },
                                                                                    { "mysqlconnection", new MySqlSqlBuilder() }
                                                                                };

        // Query cache
        private static readonly IDictionary<Type, string> _getQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _getAllQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _insertQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _updateQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _deleteQueryCache = new Dictionary<Type, string>();

        // Resolvers
        private static IForeignKeyPropertyResolver _foreignKeyPropertyResolver = new DefaultForeignKeyPropertyResolver();
        private static IKeyPropertyResolver _keyPropertyResolver = new DefaultKeyPropertyResolver();
        private static ITableNameResolver _tableNameResolver = new DefaultTableNameResolver();
        private static IColumnNameResolver _columnNameResolver = new DefaultColumnNameResolver();

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static TEntity Get<TEntity>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TEntity : class
        {
            var type = typeof(TEntity);

            string sql;
            if (!_getQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var keyColumnName = Resolvers.Column(keyProperty);

                sql = $"select * from {tableName} where {keyColumnName} = @Id";
                _getQueryCache[type] = sql;
            }

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            return connection.Query<TEntity>(sql, parameters, transaction).FirstOrDefault();
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, TReturn>(this IDbConnection connection, object id, Func<T1, T2, TReturn> map, IDbTransaction transaction = null) where TReturn : class
        {
            return MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id, transaction).FirstOrDefault();
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, TReturn>(this IDbConnection connection,
                                                       object id,
                                                       Func<T1, T2, T3, TReturn> map,
                                                       IDbTransaction transaction = null) where TReturn : class
        {
            return MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id,transaction).FirstOrDefault();
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, TReturn>(this IDbConnection connection,
                                                           object id,
                                                           Func<T1, T2, T3, T4, TReturn> map,
                                                           IDbTransaction transaction = null) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, id, transaction).FirstOrDefault();
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection,
                                                               object id,
                                                               Func<T1, T2, T3, T4, T5, TReturn> map,
                                                               IDbTransaction transaction = null) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, id,transaction).FirstOrDefault();
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection,
                                                                   object id,
                                                                   Func<T1, T2, T3, T4, T5, T6, TReturn> map,
                                                                   IDbTransaction transaction = null) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, id,transaction).FirstOrDefault();
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection,
                                                                       object id,
                                                                       Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map,
                                                                       IDbTransaction transaction = null) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, id,transaction).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>A collection of entities of type <typeparamref name="TEntity"/>.</returns>
        public static IEnumerable<TEntity> GetAll<TEntity>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true) where TEntity : class
        {
            var type = typeof(TEntity);

            string sql;
            if (!_getAllQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = $"select * from {tableName}";
                _getAllQueryCache[type] = sql;
            }

            return connection.Query<TEntity>(sql, transaction: transaction, buffered: buffered);
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, TReturn>(this IDbConnection connection, Func<T1, T2, TReturn> map, IDbTransaction transaction = null, bool buffered = true)
        {
            return MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, transaction: transaction, buffered: buffered);
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, TReturn>(this IDbConnection connection, Func<T1, T2, T3, TReturn> map, IDbTransaction transaction = null, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, transaction: transaction, buffered: buffered);
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, TReturn> map, IDbTransaction transaction = null, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, transaction: transaction, buffered: buffered);
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, TReturn> map, IDbTransaction transaction = null, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, transaction: transaction, buffered: buffered);
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, T6, TReturn> map, IDbTransaction transaction = null, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, transaction: transaction, buffered: buffered);
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
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/>
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map, IDbTransaction transaction = null, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, transaction: transaction, buffered: buffered);
        }

        private static IEnumerable<TReturn> MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(IDbConnection connection, Delegate map, object id = null, IDbTransaction transaction = null, bool buffered = true)
        {
            var resultType = typeof(TReturn);
            var resultTableName = Resolvers.Table(resultType);
            var resultTableKeyColumnName = Resolvers.Column(Resolvers.KeyProperty(resultType));

            var sql = $"select * from {resultTableName}";

            var includeTypes = new[]
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

            for (var i = 1; i < includeTypes.Length; i++)
            {
                // Determine the table to join with.
                var sourceType = includeTypes[i - 1];
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

                        sql += string.Format(" {0} join {1} on {2}.{3} = {1}.{4}",
                            joinType,
                            foreignKeyTableName,
                            sourceTableName,
                            sourceKeyColumnName,
                            foreignKeyPropertyName);
                        break;

                    case ForeignKeyRelation.ManyToMany:
                        throw new NotImplementedException("Many-to-many relationships are not supported yet.");

                    default:
                        throw new NotImplementedException($"Foreign key relation type '{relation}' is not implemented.");
                }
            }

            DynamicParameters parameters = null;
            if (id != null)
            {
                sql += string.Format(" where {0}.{1} = @{1}", resultTableName, resultTableKeyColumnName);

                parameters = new DynamicParameters();
                parameters.Add("Id", id);
            }

            switch (includeTypes.Length)
            {
                case 2:
                    return connection.Query(sql, (Func<T1, T2, TReturn>)map, parameters, transaction, buffered);
                case 3:
                    return connection.Query(sql, (Func<T1, T2, T3, TReturn>)map, parameters, transaction, buffered);
                case 4:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, TReturn>)map, parameters, transaction, buffered);
                case 5:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, TReturn>)map, parameters, transaction, buffered);
                case 6:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, T6, TReturn>)map, parameters, transaction, buffered);
                case 7:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, T6, T7, TReturn>)map, parameters, transaction, buffered);
            }

            throw new InvalidOperationException($"Invalid amount of include types: {includeTypes.Length}.");
        }

        /// <summary>
        /// Selects all the entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <param name="transaction">The transaction associated with the connection.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TEntity"/> matching the specified
        /// <paramref name="predicate"/>.
        /// </returns>
        public static IEnumerable<TEntity> Select<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null, bool buffered = true)
        {
            var type = typeof(TEntity);

            string sql;
            if (!_getAllQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = $"select * from {tableName}";
                _getAllQueryCache[type] = sql;
            }

            DynamicParameters parameters;
            sql += new SqlExpression<TEntity>()
                .Where(predicate)
                 .ToSql(out parameters);

            return connection.Query<TEntity>(sql, parameters, transaction, buffered);
        }

        /// <summary>
        /// Inserts the specified entity into the database and returns the id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The id of the inserted entity.</returns>
        public static int Insert<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null) where TEntity : class
        {
            var type = typeof(TEntity);

            string sql;
            if (!_insertQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var typeProperties = Resolvers.Properties(type).Where(p => p != keyProperty).ToList();

                var columnNames = typeProperties.Select(Resolvers.Column).ToArray();
                var paramNames = typeProperties.Select(p => "@" + p.Name).ToArray();

                var builder = GetBuilder(connection);

                sql = builder.BuildInsert(tableName, columnNames, paramNames, keyProperty);

                _insertQueryCache[type] = sql;
            }

            return connection.QueryFirst<int>(sql, entity, transaction);
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
            var type = typeof(TEntity);

            string sql;
            if (!_updateQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var typeProperties = Resolvers.Properties(type).Where(p => p != keyProperty).ToList();

                var columnNames = typeProperties.Select(p => $"{Resolvers.Column(p)} = @{p.Name}").ToArray();

                sql = $"update {tableName} set {string.Join(", ", columnNames)} where {Resolvers.Column(keyProperty)} = @{keyProperty.Name}";

                _updateQueryCache[type] = sql;
            }

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
        public static bool Delete<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null)
        {
            var type = typeof(TEntity);

            string sql;
            if (!_deleteQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var keyColumnName = Resolvers.Column(keyProperty);

                sql = $"delete from {tableName} where {keyColumnName} = @{keyProperty.Name}";

                _deleteQueryCache[type] = sql;
            }

            return connection.Execute(sql, entity, transaction) > 0;
        }

        private class DontMap
        {
        }
    }
}
