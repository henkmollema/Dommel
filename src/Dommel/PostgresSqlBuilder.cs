using System;
using System.Linq;

namespace Dommel;

/// <summary>
/// <see cref="ISqlBuilder"/> implementation for Postgres.
/// </summary>
public class PostgresSqlBuilder : ISqlBuilder
{
    /// <inheritdoc/>
    public virtual string BuildInsert(Type type, string tableName, string[] columnNames, string[] paramNames)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var sql = $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}) ";

        var keyColumns = Resolvers.KeyProperties(type).Where(p => p.IsGenerated).Select(p => Resolvers.Column(p.Property, this, false));
        if (keyColumns.Any())
        {
            sql += $"returning ({string.Join(", ", keyColumns)})";
        }

        return sql;
    }

    /// <inheritdoc/>
    public virtual string BuildPaging(string? orderBy, int pageNumber, int pageSize)
    {
        var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
        return $" {orderBy} offset {start} limit {pageSize}";
    }

    /// <inheritdoc/>
    public string PrefixParameter(string paramName) => $"@{paramName}";

    /// <inheritdoc/>
    public string QuoteIdentifier(string identifier) => $"\"{identifier}\"";

    /// <inheritdoc/>
    public string LimitClause(int count) => $"limit {count}";

    /// <inheritdoc/>
    public string LikeExpression(string columnName, string parameterName) => $"{columnName} ilike {parameterName}";
}
