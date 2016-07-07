﻿using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        private sealed class SqliteSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select last_insert_rowid() id";
            }
        }
    }
}
