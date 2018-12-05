using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for Postgres.
        /// </summary>
        public class PostgresSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public virtual string BuildInsert(string tableName, string[] columnNames, string[] paramNames,
                IEnumerable<PropertyInfo> identityProperties)
            {
                var sql = $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)})";

                if (identityProperties != null)
                {
                    var properties = identityProperties.ToArray();

                    // We know it's Postgres here
                    var identityColumnNames = properties.Select(p => Resolvers.Column(p, this));
                    sql += " RETURNING " + string.Join(", ", identityColumnNames);
                }

                return sql;
            }

            /// <inheritdoc/>
            public virtual string BuildPaging(string orderBy, int pageNumber, int pageSize)
            {
                var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
                return $" {orderBy} OFFSET {start} LIMIT {pageSize}";
            }

            /// <inheritdoc/>
            public string PrefixParameter(string paramName)
            {
                return $"@{paramName}";
            }

            /// <inheritdoc/>
            public string QuoteIdentifier(string identifier) => $"\"{identifier}\"";
        }
    }
}
