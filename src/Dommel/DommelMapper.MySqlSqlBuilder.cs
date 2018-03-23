using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for MySQL.
        /// </summary>
        public sealed class MySqlSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                if (EscapeCharacterStart == char.MinValue && EscapeCharacterEnd == char.MinValue)
                {
                    // Fall back to the default behavior.
                    return $"insert into `{tableName}` (`{string.Join("`, `", columnNames)}`) values ({string.Join(", ", paramNames)}); select LAST_INSERT_ID() id";
                }

                // Table and column names are already escaped.
                return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select LAST_INSERT_ID() id";
            }
        }
    }
}
