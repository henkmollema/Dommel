using System;

namespace Dommel;

/// <summary>
/// <see cref="ISqlBuilder"/> implementation for SQL Server Compact Edition.
/// </summary>
public class SqlServerCeSqlBuilder : ISqlBuilder
{
    /// <inheritdoc/>
    public virtual string BuildInsert(Type type, string tableName, string[] columnNames, string[] paramNames) =>
        $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select @@IDENTITY";

    /// <inheritdoc/>
    public virtual string BuildPaging(string? orderBy, int pageNumber, int pageSize)
    {
        var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
        return $" {orderBy} offset {start} rows fetch next {pageSize} rows only";
    }

    /// <inheritdoc/>
    public string PrefixParameter(string paramName) => $"@{paramName}";

    /// <inheritdoc/>
    public string QuoteIdentifier(string identifier) => $"[{identifier}]";

    /// <inheritdoc/>
    public string LimitClause(int count) => $"order by 1 offset 0 rows fetch next {count} rows only";

    /// <inheritdoc/>
    public string LikeExpression(string columnName, string parameterName) => $"{columnName} like {parameterName}";
}
