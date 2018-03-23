using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for SQLite.
        /// </summary>
        public sealed class SqliteSqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select last_insert_rowid() id";
            }
        }
    }
}
