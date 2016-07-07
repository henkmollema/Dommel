using System;
using System.Data;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        /// Adds a custom implementation of <see cref="ISqlBuilder"/> for the specified ADO.NET connection type.
        /// </summary>
        /// <param name="connectionType">
        /// The ADO.NET connection type to use the <paramref name="builder"/> with.
        /// </param>
        /// <param name="builder">An implementation of the <see cref="ISqlBuilder"/> interface.</param>
        public static void AddSqlBuilder(Type connectionType, ISqlBuilder builder)
        {
            if (connectionType == null)
            {
                throw new ArgumentNullException(nameof(connectionType));
            }

            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _sqlBuilders[connectionType.Name.ToLower()] = builder;
        }

        private static ISqlBuilder GetBuilder(IDbConnection connection)
        {
            var connectionName = connection.GetType().Name.ToLower();
            ISqlBuilder builder;
            return _sqlBuilders.TryGetValue(connectionName, out builder) ? builder : new SqlServerSqlBuilder();
        }

        /// <summary>
        /// Defines methods for building specialized SQL queries.
        /// </summary>
        public interface ISqlBuilder
        {
            /// <summary>
            /// Builds an insert query using the specified table name, column names and parameter names.
            /// A query to fetch the new id will be included as well.
            /// </summary>
            /// <param name="tableName">The name of the table to query.</param>
            /// <param name="columnNames">The names of the columns in the table.</param>
            /// <param name="paramNames">The names of the parameters in the database command.</param>
            /// <param name="keyProperty">
            /// The key property. This can be used to query a specific column for the new id.
            /// This is optional.
            /// </param>
            /// <returns>An insert query including a query to fetch the new id.</returns>
            string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty);
        }
    }
}
