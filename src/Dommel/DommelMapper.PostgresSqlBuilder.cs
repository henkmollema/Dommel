using System;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for Postgres.
        /// </summary>
        public sealed class PostgresSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                var sql = $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)})";

                if (keyProperty != null)
                {
                    // We know it's Postgres here
                    var keyColumnName = Resolvers.Column(keyProperty, new PostgresSqlBuilder());
                    sql += " RETURNING " + keyColumnName;
                }
                else
                {
                    // todo: what behavior is desired here?
                    throw new Exception("A key property is required for the PostgresSqlBuilder.");
                }

                return sql;
            }

            /// <inheritdoc/>
            public string BuildPaging(string orderBy, int pageNumber, int pageSize)
            {
                var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
                return $" {orderBy} OFFSET {start} LIMIT {pageSize}";
            }

            /// <inheritdoc/>s
            public string PrefixParameter(string paramName)
            {
                return $"@{paramName}";
            }

            /// <inheritdoc/>
            public string QuoteIdentifier(string identifier) => $"\"{identifier}\"";
        }
    }
}
