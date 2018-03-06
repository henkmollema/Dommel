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
        private static readonly ConcurrentDictionary<Type, string> _updateQueryCache = new ConcurrentDictionary<Type, string>();

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
            if (!_updateQueryCache.TryGetValue(type, out var sql))
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
    }
}
