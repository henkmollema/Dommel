using System;
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
            var sql = BuildUpdateQuery(connection, typeof(TEntity));
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
        /// <returns>A value indicating whether the update operation succeeded.</returns>
        public static async Task<bool> UpdateAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null)
        {
            var sql = BuildUpdateQuery(connection, typeof(TEntity));
            LogQuery<TEntity>(sql);
            return await connection.ExecuteAsync(sql, entity, transaction) > 0;
        }

        private static string BuildUpdateQuery(IDbConnection connection, Type type)
        {
            var cacheKey = new QueryCacheKey(QueryCacheType.Update, connection, type);
            if (!QueryCache.TryGetValue(cacheKey, out var sql))
            {
                var tableName = Resolvers.Table(type, connection);
                var keyProperty = Resolvers.KeyProperty(type);
                var builder = GetSqlBuilder(connection);

                // Use all properties which are settable.
                var typeProperties = Resolvers.Properties(type)
                                              .Where(p => p != keyProperty)
                                              .Where(p => p.GetSetMethod() != null)
                                              .ToArray();

                var columnNames = typeProperties.Select(p => $"{Resolvers.Column(p, connection)} = {builder.PrefixParameter(p.Name)}").ToArray();
                sql = $"update {tableName} set {string.Join(", ", columnNames)} where {Resolvers.Column(keyProperty, connection)} = {builder.PrefixParameter(keyProperty.Name)}";

                QueryCache.TryAdd(cacheKey, sql);
            }

            return sql;
        }
    }
}
