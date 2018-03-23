using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for SQL Server.
        /// </summary>
        public sealed class SqlServerSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return $"set nocount on insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) select cast(scope_identity() as int)";
            }
        }
    }
}
