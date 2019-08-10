using System;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for MySQL.
        /// </summary>
        public class MySqlSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public virtual string BuildInsert(Type type, string tableName, string[] columnNames, string[] paramNames) =>
                $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select LAST_INSERT_ID() id";

            /// <inheritdoc/>
            public virtual string BuildPaging(string orderBy, int pageNumber, int pageSize)
            {
                var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
                return $" {orderBy} LIMIT {start}, {pageSize}";
            }

            /// <inheritdoc/>
            public string PrefixParameter(string paramName) => $"@{paramName}";

            /// <inheritdoc/>
            public string QuoteIdentifier(string identifier) => $"`{identifier}`";
        }
    }
}
