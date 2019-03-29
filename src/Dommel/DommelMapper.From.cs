using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Dommel
{
    public static partial class DommelMapper
    {
        public static async Task<IEnumerable<TEntity>> FromAsync<TEntity>(this IDbConnection con, Action<SqlExpression<TEntity>> sqlBuilder)
            where TEntity : class, new()
        {
            var sqlExpression = new SqlExpression<TEntity>(GetSqlBuilder(con));
            sqlBuilder(sqlExpression);
            var sql = sqlExpression.ToSql(out var parameters);
            return await con.QueryAsync<TEntity>(sql, parameters);
        }
    }
}
