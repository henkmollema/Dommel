using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dommel
{
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
        public static object Insert<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildInsertQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            return connection.QuerySingleOrDefault(sql, entity, transaction);
        }

        /// <summary>
        /// Inserts the specified entity into the database and returns the ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The ID of the inserted entity.</returns>
        public static Task<object> InsertAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildInsertQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            return connection.QuerySingleOrDefaultAsync(sql, entity, transaction);
        }

        /// <summary>
        /// Inserts the specified collection of entities into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entities">The entities to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The number of records affected.</returns>
        public static int InsertAll<TEntity>(this IDbConnection connection, IEnumerable<TEntity> entities, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildInsertQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            return connection.Execute(sql, entities, transaction);
        }

        /// <summary>
        /// Inserts the specified collection of entities into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entities">The entities to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The number of records affected.</returns>
        public static Task<int> InsertAllAsync<TEntity>(this IDbConnection connection, IEnumerable<TEntity> entities, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildInsertQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            return connection.ExecuteAsync(sql, entities);
        }

        private static string BuildInsertQuery(IDbConnection connection, Type type)
        {
            var cacheKey = new QueryCacheKey(QueryCacheType.Insert, connection, type);
            if (!QueryCache.TryGetValue(cacheKey, out var sql))
            {
                var tableName = Resolvers.Table(type, connection);
                var identityProperties = Resolvers.IdentityProperties(type).ToArray();

                var typeProperties = new List<PropertyInfo>();
                foreach (var typeProperty in Resolvers.Properties(type))
                {
                    if (identityProperties.Contains(typeProperty))
                    {
                        // Skip key properties marked as an identity column.
                        continue;
                    }

                    if (typeProperty.GetSetMethod() != null)
                    {
                        typeProperties.Add(typeProperty);
                    }
                }

                var columnNames = typeProperties.Select(p => Resolvers.Column(p, connection)).ToArray();
                var paramNames = typeProperties.Select(p => "@" + p.Name).ToArray();

                var builder = GetSqlBuilder(connection);
                sql = builder.BuildInsert(tableName, columnNames, paramNames, identityProperties);

                QueryCache.TryAdd(cacheKey, sql);
            }

            return sql;
        }
    }
}
