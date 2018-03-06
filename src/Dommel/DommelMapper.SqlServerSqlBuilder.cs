using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        private sealed class SqlServerSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return $"set nocount on insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) select cast(scope_identity() as int)";
            }
        }
    }
}
