namespace Dommel.Json
{
    /// <summary>
    /// Extends the <see cref="DommelMapper.ISqlBuilder"/> with support for
    /// creating JSON value expressions.
    /// </summary>
    public interface IJsonSqlBuilder : DommelMapper.ISqlBuilder
    {
        /// <summary>
        /// Creates a JSON value expression for the specified <paramref name="column"/> and <paramref name="path"/>.
        /// </summary>
        /// <param name="column">The column which contains the JSON data.</param>
        /// <param name="path">The path of the JSON value to query.</param>
        /// <returns>A JSON value expression.</returns>
        string JsonValue(string column, string path);
    }

    /// <summary>
    /// JSON SQL builder for SQL server.
    /// </summary>
    public class SqlServerSqlBuilder : DommelMapper.SqlServerSqlBuilder, IJsonSqlBuilder
    {
        /// <inheritdoc />
        public string JsonValue(string column, string path) => $"JSON_VALUE({column}, '$.{path}')";
    }

    /// <summary>
    /// JSON SQL builder for MySQL.
    /// </summary>
    public class MySqlSqlBuilder : DommelMapper.MySqlSqlBuilder, IJsonSqlBuilder
    {
        /// <inheritdoc />
        public string JsonValue(string column, string path) => $"{column}->'$.{path}'";
    }

    /// <summary>
    /// JSON SQL builder for PostgreSQL.
    /// </summary>
    public class PostgresSqlBuiler : DommelMapper.PostgresSqlBuilder, IJsonSqlBuilder
    {
        /// <inheritdoc />
        public string JsonValue(string column, string path) => $"{column}->>'{path}'";
    }

    /// <summary>
    /// JSON SQL builder for SQLite.
    /// </summary>
    public class SqliteSqlBuilder : DommelMapper.SqliteSqlBuilder, IJsonSqlBuilder
    {
        /// <inheritdoc />
        public string JsonValue(string column, string path) => $"JSON_EXTRACT({column}, '$.{path}')";
    }

    /// <summary>
    /// JSON SQL builder for SQL Server CE.
    /// </summary>
    public class SqlServerCeSqlBuilder : DommelMapper.SqlServerCeSqlBuilder, IJsonSqlBuilder
    {
        /// <inheritdoc />
        public string JsonValue(string column, string path) => $"JSON_VALUE({column}, '$.{path}')";
    }
}
