using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for SQL Server.
        /// </summary>
        public class SqlServerSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public virtual string BuildInsert(string tableName, string[] columnNames, string[] paramNames,
                IEnumerable<PropertyInfo> keyProperties, IEnumerable<PropertyInfo> identityProperties)
            {
                var outputClause = "";
                if (identityProperties != null)
                {
                    var properties = identityProperties.ToArray();
                    if (properties.Any())
                    {
                        foreach (var property in properties)
                        {
                            outputClause += $", inserted.{Resolvers.Column(property, this)}";
                        }

                        outputClause = $" output {outputClause.Substring(2)}";
                    }
                }

                return $"insert into {tableName} ({string.Join(", ", columnNames)}){outputClause} values ({string.Join(", ", paramNames)})";
            }
                

            /// <inheritdoc/>
            public virtual string BuildPaging(string orderBy, int pageNumber, int pageSize)
            {
                var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
                return $" {orderBy} offset {start} rows fetch next {pageSize} rows only";
            }

            /// <inheritdoc/>
            public string PrefixParameter(string paramName)
            {
                return $"@{paramName}";
            }

            /// <inheritdoc/>
            public string QuoteIdentifier(string identifier) => $"[{identifier}]";
        }
    }
}
