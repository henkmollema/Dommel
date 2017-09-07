using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dommel
{
    internal sealed class MySqlSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            return $"insert into {tableName} (`{string.Join("`, `", columnNames)}`) values ({string.Join(", ", paramNames)}); select LAST_INSERT_ID() id";
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var start = pageNo >= 1 ? (pageNo - 1) * pageSize : 0;
            return $" {sql} {orderBySql} LIMIT {start}, {pageSize} ";
        }
    }
}
