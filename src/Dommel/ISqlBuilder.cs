using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dommel
{
    /// <summary>
    /// Defines methods for building specialized SQL queries.
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// Builds an insert query using the specified table name, column names and parameter names.
        /// A query to fetch the new id will be included as well.
        /// </summary>
        /// <param name="tableName">The name of the table to query.</param>
        /// <param name="columnNames">The names of the columns in the table.</param>
        /// <param name="paramNames">The names of the parameters in the database command.</param>
        /// <param name="keyProperty">
        /// The key property. This can be used to query a specific column for the new id. This is
        /// optional.
        /// </param>
        /// <returns>An insert query including a query to fetch the new id.</returns>
        string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty);

        /// <summary>
        /// Builds an pagination query using the specified page number and page size.
        /// </summary>
        /// <param name="tableName">The table name.</param>
        /// <param name="whereSql">Sql including where</param>
        /// <param name="orderBySql">Order by clause.</param>
        /// <param name="orderByAsc">Order by type.</param>
        /// <param name="pageNo">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The pagination query.</returns>
        string BuildPagination(string tableName, string whereSql, string orderBySql, bool orderByAsc, int pageNo, int pageSize);
    }
}
