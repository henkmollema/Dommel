using System;
using System.Collections.Generic;
using System.Data;

namespace Dommel
{
    public static partial class DommelMapper
    {
        internal static readonly Dictionary<string, ISqlBuilder> SqlBuilders = new Dictionary<string, ISqlBuilder>
        {
            ["sqlconnection"] = new SqlServerSqlBuilder(),
            ["sqlceconnection"] = new SqlServerCeSqlBuilder(),
            ["sqliteconnection"] = new SqliteSqlBuilder(),
            ["npgsqlconnection"] = new PostgresSqlBuilder(),
            ["mysqlconnection"] = new MySqlSqlBuilder()
        };

        /// <summary>
        /// Adds a custom implementation of <see cref="ISqlBuilder"/>
        /// for the specified ADO.NET connection type.
        /// </summary>
        /// <param name="connectionType">
        /// The ADO.NET conncetion type to use the <paramref name="builder"/> with.
        /// Example: <c>typeof(SqlConnection)</c>.
        /// </param>
        /// <param name="builder">An implementation of the <see cref="ISqlBuilder"/> interface.</param>
        public static void AddSqlBuilder(Type connectionType, ISqlBuilder builder) => AddSqlBuilder(connectionType.Name.ToLower(), builder);

        /// <summary>
        /// Adds a custom implementation of <see cref="ISqlBuilder"/>
        /// for the specified connection name        ///
        /// </summary>
        /// <param name="connectionName">The name of the connection. E.g. "sqlconnection".</param>
        /// <param name="builder">An implementation of the <see cref="ISqlBuilder"/> interface.</param>
        public static void AddSqlBuilder(string connectionName, ISqlBuilder builder) => SqlBuilders[connectionName] = builder;

        /// <summary>
        /// Gets the configured <see cref="ISqlBuilder"/> for the specified <see cref="IDbConnection"/> instance.
        /// </summary>
        /// <param name="connection">The database connection instance.</param>
        /// <returns>The <see cref="ISqlBuilder"/> interface for the specified <see cref="IDbConnection"/> instance.</returns>
        public static ISqlBuilder GetSqlBuilder(IDbConnection connection)
        {
            var connectionTypeName = connection.GetType().Name;
            var builder = SqlBuilders.TryGetValue(connectionTypeName.ToLower(), out var b) ? b : new SqlServerSqlBuilder();
            LogReceived?.Invoke($"Selected SQL Builder '{builder.GetType().Name}' for connection type '{connectionTypeName}'");
            return builder;
        }

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
        }
    }
}
