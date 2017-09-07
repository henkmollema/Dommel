using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dommel
{
    internal sealed class PostgresSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            var sql = $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)})";

            if (keyProperty != null)
            {
                var keyColumnName = Resolvers.Column(keyProperty);

                sql += " RETURNING " + keyColumnName;
            }
            else
            {
                // todo: what behavior is desired here?
                throw new Exception("A key property is required for the PostgresSqlBuilder.");
            }

            return sql;
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var start = pageNo >= 1 ? (pageNo - 1) * pageSize : 0;
            return $" {sql} {orderBySql} OFFSET {start} LIMIT {pageSize} ";
        }
    }
}
