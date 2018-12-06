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
            var result = connection.QuerySingleOrDefault(sql, entity, transaction);


            result = PopulateEntity(entity, result);

            return result;
        }

        /// <summary>
        /// Inserts the specified entity into the database and returns the ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The ID of the inserted entity.</returns>
        public static async Task<object> InsertAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildInsertQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            var result = await connection.QuerySingleOrDefaultAsync(sql, entity, transaction).ConfigureAwait(false);

            result = PopulateEntity(entity, result);

            return result;
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
            return connection.ExecuteAsync(sql, entities, transaction);
        }

        private static string BuildInsertQuery(IDbConnection connection, Type type)
        {
            var cacheKey = new QueryCacheKey(QueryCacheType.Insert, connection, type);
            if (!QueryCache.TryGetValue(cacheKey, out var sql))
            {
                var tableName = Resolvers.Table(type, connection);
                var keyProperties = Resolvers.KeyProperties(type);
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
                var paramNames = typeProperties.Select(p => $"@{p.Name}").ToArray();

                var builder = GetSqlBuilder(connection);
                sql = builder.BuildInsert(tableName, columnNames, paramNames, keyProperties, identityProperties);

                QueryCache.TryAdd(cacheKey, sql);
            }

            return sql;
        }

        private static dynamic PopulateEntity<TEntity>(TEntity entity, dynamic result)
        {
            // populate the identity values back into the entity
            if (result is IDictionary<string, object> row)
            {
                foreach (var column in row)
                {
                    var typeMap = SqlMapper.GetTypeMap(typeof(TEntity));
                    var memberMap = typeMap.GetMember(column.Key);

                    var identityProperty = memberMap != null ? memberMap.Property : Resolvers.IdentityProperty(typeof(TEntity));
                    var value = identityProperty.PropertyType != column.Value.GetType()
                        ? Convert.ChangeType(column.Value, identityProperty.PropertyType)
                        : column.Value;
                    identityProperty.SetValue(entity, value);

                    if (row.Count == 1)
                    {
                        // if the entity only contains 1 identity property, return just the identity value instead of a list of key/value.
                        result = value;
                    }
                }
            }

            return result;
        }
    }
}
