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

    internal class SqlServerSqlBuilder : DommelMapper.SqlServerSqlBuilder, IJsonSqlBuilder
    {
        public string JsonValue(string column, string path) => $"JSON_VALUE({column}, '$.{path}')";
    }

    internal class MySqlSqlBuilder : DommelMapper.MySqlSqlBuilder, IJsonSqlBuilder
    {
        public string JsonValue(string column, string path) => $"{column}->'$.{path}'";
    }

    internal class PostgresSqlBuiler : DommelMapper.PostgresSqlBuilder, IJsonSqlBuilder
    {
        public string JsonValue(string column, string path) => $"{column}->>'{path}'";
    }

    internal class SqliteSqlBuilder : DommelMapper.SqliteSqlBuilder, IJsonSqlBuilder
    {
        public string JsonValue(string column, string path) => $"JSON_EXTRACT({column}, '$.{path}')";
    }

    internal class SqlServerCeSqlBuilder : DommelMapper.SqlServerCeSqlBuilder, IJsonSqlBuilder
    {
        public string JsonValue(string column, string path) => $"JSON_VALUE({column}, '$.{path}')";
    }
}
