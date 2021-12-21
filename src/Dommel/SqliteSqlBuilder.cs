using System;

namespace Dommel;

/// <summary>
/// <see cref="ISqlBuilder"/> implementation for SQLite.
/// </summary>
public class SqliteSqlBuilder : ISqlBuilder
{
    /// <inheritdoc/>
    public virtual string BuildInsert(Type type, string tableName, string[] columnNames, string[] paramNames) =>
        $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select last_insert_rowid() id";

    /// <inheritdoc/>
    public virtual string BuildPaging(string? orderBy, int pageNumber, int pageSize)
    {
        var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
        return $" {orderBy} LIMIT {start}, {pageSize}";
    }

    /// <inheritdoc/>
    public string PrefixParameter(string paramName) => $"@{paramName}";

    /// <inheritdoc/>
    public string QuoteIdentifier(string identifier) => identifier;

    /// <inheritdoc/>
    public string LimitClause(int count) => $"limit {count}";

    /// <inheritdoc/>
    public string LikeExpression(string columnName, string parameterName) => $"{columnName} like {parameterName}";
}
