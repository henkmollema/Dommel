using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Executes an expression to query data from <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity to query data from.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static async Task<IEnumerable<TEntity>> FromAsync<TEntity>(this IDbConnection con, Action<SqlExpression<TEntity>> sqlBuilder)
        {
            var sqlExpression = new SqlExpression<TEntity>(GetSqlBuilder(con));
            sqlBuilder(sqlExpression);
            var sql = sqlExpression.ToSql(out var parameters);
            return await con.QueryAsync<TEntity>(sql, parameters);
        }
    }
}
