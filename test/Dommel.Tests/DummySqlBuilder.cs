using System;

namespace Dommel.Tests
{
    internal class DummySqlBuilder : ISqlBuilder
    {
        /// <inheritdoc/>
        public string PrefixParameter(string paramName) => $"#{paramName}";

        /// <inheritdoc/>
        public string QuoteIdentifier(string identifier) => identifier;

        /// <inheritdoc/>
        public string BuildInsert(Type type, string tableName, string[] columnNames, string[] paramNames) =>
            $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select last_insert_rowid() id";

        /// <inheritdoc/>
        public string BuildPaging(string? orderBy, int pageNumber, int pageSize)
        {
            var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
            return $" {orderBy} LIMIT {start}, {pageSize}";
        }
    }
}
