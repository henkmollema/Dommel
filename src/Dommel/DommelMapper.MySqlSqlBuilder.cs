using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        private sealed class MySqlSqlBuilder : ISqlBuilder
        {
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
