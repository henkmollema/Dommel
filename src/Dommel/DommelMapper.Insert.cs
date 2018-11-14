using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<Type, string> _insertQueryCache = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Inserts the specified entity into the database and returns the ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The id of the inserted entity.</returns>
        public static object Insert<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null) where TEntity : class
        {
            var sql = BuildInsertQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            return connection.ExecuteScalar(sql, entity, transaction);
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
            return connection.ExecuteScalarAsync(sql, entity, transaction);
        }

        /// <summary>
        /// Inserts the specified collection of entities into the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entities">The entities to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The id of the inserted entity.</returns>
        public static void InsertAll<TEntity>(this IDbConnection connection, IEnumerable<TEntity> entities, IDbTransaction transaction = null) where TEntity : class
        {
            var entity = entities.FirstOrDefault();
            if (entity == null)
            {
                return;
            }

            var sql = BuildInsertQuery(connection, typeof(TEntity));
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
        /// <returns>The id of the inserted entity.</returns>
        public static async Task InsertAsyncAll<TEntity>(this IDbConnection connection, IEnumerable<TEntity> entities, IDbTransaction transaction = null) where TEntity : class
        {
            var entity = entities.FirstOrDefault();
            if (entity == null)
            {
                return;
            }

            var sql = BuildInsertQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            await connection.ExecuteAsync(sql, entities);
        }

        private static string BuildInsertQuery(IDbConnection connection, Type type)
        {
            if (!_insertQueryCache.TryGetValue(type, out var sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type, out var isIdentity);

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

                var builder = GetSqlBuilder(connection);
                sql = builder.BuildInsert(tableName, columnNames, paramNames, keyProperty);

                _insertQueryCache.TryAdd(type, sql);
            }

            return sql;
        }
    }
}
