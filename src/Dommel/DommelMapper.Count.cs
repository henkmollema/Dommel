using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dapper;

namespace Dommel
{
    public static partial class DommelMapper
    {
        private static readonly ConcurrentDictionary<Type, string> _countQueryCache = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Returns the number of entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The number of entities matching the specified predicate.</returns>
        public static long Count<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var sql = BuildCountSql(predicate, out var parameters);
            LogQuery<TEntity>(sql);
            return connection.ExecuteScalar<long>(sql, parameters, transaction);
        }

        /// <summary>
        /// Returns the number of entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The number of entities matching the specified predicate.</returns>
        public static Task<long> CountAsync<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, IDbTransaction transaction = null)
        {
            var sql = BuildCountSql(predicate, out var parameters);
            LogQuery<TEntity>(sql);
            return connection.ExecuteScalarAsync<long>(sql, parameters, transaction);
        }

        private static string BuildCountSql<TEntity>(Expression<Func<TEntity, bool>> predicate, out DynamicParameters parameters)
        {
            var type = typeof(TEntity);
            if (!_countQueryCache.TryGetValue(type, out var sql))
            {
                var tableName = Resolvers.Table(type);
                sql = $"select count(*) from {tableName}";
                _countQueryCache.TryAdd(type, sql);
            }

            sql += new SqlExpression<TEntity>()
                .Where(predicate)
                .ToSql(out parameters);
            return sql;
        }
    }
}
