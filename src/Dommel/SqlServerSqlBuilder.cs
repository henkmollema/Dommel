using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dommel
{
    internal sealed class SqlServerSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            return $"set nocount on insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) select cast(scope_identity() as int)";
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var offset = pageNo > 0 ? (pageNo * pageSize) : 0;

            if (string.IsNullOrWhiteSpace(orderBySql))
            {
                orderBySql = " ORDER BY ID ";
            }

            return
            $@" SELECT  * FROM {tableName} {orderBySql}
                    OFFSET {offset} FETCH NEXT {pageSize} ROWS ONLY";
        }
    }
}
