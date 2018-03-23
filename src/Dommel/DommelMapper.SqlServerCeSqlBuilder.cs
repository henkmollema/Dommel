using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for SQL Server Compact Edition.
        /// </summary>
        public sealed class SqlServerCeSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) select cast(@@IDENTITY as int)";
            }
        }
    }
}
