using System.Collections.Generic;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for SQLite.
        /// </summary>
        public class SqliteSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public virtual string BuildInsert(string tableName, string[] columnNames, string[] paramNames,
                IEnumerable<PropertyInfo> identityProperties)
            {
                return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select last_insert_rowid() id";
            }

            /// <inheritdoc/>
            public virtual string BuildPaging(string orderBy, int pageNumber, int pageSize)
            {
                var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
                return $" {orderBy} LIMIT {start}, {pageSize}";
            }

            /// <inheritdoc/>
            public string PrefixParameter(string paramName)
            {
                return $"@{paramName}";
            }

            /// <inheritdoc/>
            public string QuoteIdentifier(string identifier) => identifier;
        }
    }
}
