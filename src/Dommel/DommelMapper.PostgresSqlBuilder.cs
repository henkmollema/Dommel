using System;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        private sealed class PostgresSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                var sql = $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) select last_insert_rowid() id";

                if (keyProperty != null)
                {
                    var keyColumnName = Resolvers.Column(keyProperty);

                    sql += " RETURNING " + keyColumnName;
                }
                else
                {
                    throw new Exception("A key property is required for the PostgresSqlBuilder.");
                }

                return sql;
            }
        }
    }
}
