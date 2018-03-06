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
        private static readonly ConcurrentDictionary<Type, string> _getCountCache = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Returns the number of entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <returns>The number of entities matching the specified predicate.</returns>
        public static long Count<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate)
        {
            DynamicParameters parameters;
            var sql = BuildCountSql(predicate, out parameters);
            return connection.ExecuteScalar<long>(sql, parameters);
        }

        /// <summary>
        /// Returns the number of entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <returns>The number of entities matching the specified predicate.</returns>
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
    }
}
