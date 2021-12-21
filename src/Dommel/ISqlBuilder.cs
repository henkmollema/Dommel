using System;

namespace Dommel;

/// <summary>
/// Defines methods for building specialized SQL queries.
/// </summary>
public interface ISqlBuilder
{
    /// <summary>
    /// Adds a prefix to the specified parameter.
    /// </summary>
    /// <param name="paramName">The name of the parameter to prefix.</param>
    string PrefixParameter(string paramName);

    /// <summary>
    /// Builds an insert query using the specified table name, column names and parameter names.
    /// A query to fetch the new ID will be included as well.
    /// </summary>
    /// <param name="type">The type of the entity to generate the insert query for.</param>
    /// <param name="tableName">The name of the table to query.</param>
    /// <param name="columnNames">The names of the columns in the table.</param>
    /// <param name="paramNames">The names of the parameters in the database command.</param>
    /// <returns>An insert query including a query to fetch the new ID.</returns>
    string BuildInsert(Type type, string tableName, string[] columnNames, string[] paramNames);

    /// <summary>
    /// Builds the paging part to be appended to an existing select query.
    /// </summary>
    /// <param name="orderBy">The order by part of the query.</param>
    /// <param name="pageNumber">The number of the page to fetch, starting at 1.</param>
    /// <param name="pageSize">The page size.</param>
    /// <returns>The paging part of a query.</returns>
    string BuildPaging(string? orderBy, int pageNumber, int pageSize);

    /// <summary>
    /// Adds quotes around (or at the start) of an identifier such as a table or column name.
    /// </summary>
    /// <param name="identifier">The identifier add quotes around. E.g. a table or column name.</param>
    /// <returns>The quoted <paramref name="identifier"/>.</returns>
    string QuoteIdentifier(string identifier);

    /// <summary>
    /// Returns a limit clause for the specified <paramref name="count"/>.
    /// </summary>
    /// <param name="count">The count of limit clause.</param>
    /// <returns>A limit clause of the specified count.</returns>
    string LimitClause(int count);

    /// <summary>
    /// Returns a like-expresion for the specified <paramref name="columnName"/> and <paramref name="parameterName"/>.
    /// </summary>
    /// <param name="columnName">The column name of the like-expression.</param>
    /// <param name="parameterName">The parameter name of the like-expression.</param>
    /// <returns>A like-expression.</returns>
    string LikeExpression(string columnName, string parameterName);
}
