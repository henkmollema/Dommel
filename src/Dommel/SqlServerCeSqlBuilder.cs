using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dommel
{
    internal sealed class SqlServerCeSqlBuilder : ISqlBuilder
    {
        public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
        {
            return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) select cast(@@IDENTITY as int)";
        }

        public string BuildPagination(string tableName, string sql, string orderBySql, bool orderByAsc, int pageNo, int pageSize)
        {
            var start = pageNo >= 1 ? (pageNo - 1) * pageSize : 0;
            var end = pageNo * pageSize;

            return
                $" SELECT  * FROM ( SELECT ROW_NUMBER() OVER ( {orderBySql} ) AS RowNum, * " +
                $" FROM {tableName} " +
                " ) AS RowConstrainedResult " +
                $" WHERE RowNum >= {start} " +
                $" AND RowNum <= {end} " +
                " ORDER BY RowNum " + (orderByAsc ? "ASC" : "DESC");
        }
    }
}
