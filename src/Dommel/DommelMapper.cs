using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Dommel
{
    /// <summary>
    /// Simple CRUD commands for Dapper.
    /// </summary>
    public static class DommelMapper
    {
        private static readonly IDictionary<Type, string> _typeTableNameCache = new Dictionary<Type, string>();
        private static readonly IDictionary<string, string> _columnNameCache = new Dictionary<string, string>();
        private static readonly IDictionary<Type, PropertyInfo> _typeKeyPropertyCache = new Dictionary<Type, PropertyInfo>();
        private static readonly IDictionary<Type, PropertyInfo[]> _typePropertiesCache = new Dictionary<Type, PropertyInfo[]>();
        private static readonly IDictionary<string, ISqlBuilder> _sqlBuilders = new Dictionary<string, ISqlBuilder>
        {
            { "sqlconnection", new SqlServerSqlBuilder() },
            { "sqlceconnection", new SqlServerCeSqlBuilder() },
            { "sqliteconnection", new SqliteSqlBuilder() },
            { "npgsqlconnection", new PostgresSqlBuilder() },
            { "mysqlconnection", new MySqlSqlBuilder() }
        };

        private static readonly IDictionary<Type, string> _getQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _getAllQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _insertQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _updateQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _deleteQueryCache = new Dictionary<Type, string>();

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static TEntity Get<TEntity>(this IDbConnection connection, object id) where TEntity : class
        {
            var type = typeof(TEntity);

            string sql;
            if (!_getQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                var keyProperty = GetKeyProperty(type);
                string keyColumnName = GetColumnName(type, keyProperty);

                sql = string.Format("select * from {0} where {1} = @Id", tableName, keyColumnName);
                _getQueryCache[type] = sql;
            }

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            return connection.Query<TEntity>(sql: sql, param: parameters).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>A collection of entities of type <typeparamref name="TEntity"/>.</returns>
        public static IEnumerable<TEntity> GetAll<TEntity>(this IDbConnection connection, bool buffered = true) where TEntity : class
        {
            var type = typeof(TEntity);

            string sql;
            if (!_getAllQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                sql = string.Format("select * from {0}", tableName);
                _getAllQueryCache[type] = sql;
            }

            return connection.Query<TEntity>(sql: sql, buffered: buffered);
        }

        /// <summary>
        /// Inserts the specified entity into the database and returns the id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be inserted.</param>
        /// <returns>The id of the inserted entity.</returns>
        public static int Insert<TEntity>(this IDbConnection connection, TEntity entity) where TEntity : class
        {
            var type = typeof(TEntity);

            string sql;
            if (!_insertQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                var keyProperty = GetKeyProperty(type);
                var typeProperties = GetTypeProperties(type).Where(p => p != keyProperty).ToList();

                string[] columnNames = typeProperties.Select(p => GetColumnName(type, p)).ToArray();
                string[] paramNames = typeProperties.Select(p => "@" + p.Name).ToArray();

                var builder = GetBuilder(connection);

                sql = builder.BuildInsert(tableName, columnNames, paramNames, keyProperty);

                _insertQueryCache[type] = sql;
            }

            var result = connection.Query<int>(sql, entity);
            return result.Single();
        }

        /// <summary>
        /// Updates the values of the specified entity in the database. 
        /// The return value indicates whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity in the database.</param>
        /// <returns>A value indicating whether the update operation succeeded.</returns>
        public static bool Update<TEntity>(this IDbConnection connection, TEntity entity)
        {
            var type = typeof(TEntity);

            string sql;
            if (!_updateQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                var keyProperty = GetKeyProperty(type);
                var typeProperties = GetTypeProperties(type).Where(p => p != keyProperty).ToList();

                string[] columnNames = typeProperties.Select(p => string.Format("{0} = @{1}", GetColumnName(type, p), p.Name)).ToArray();

                sql = string.Format("update {0} set {1} where {2} = @{3}",
                    tableName,
                    string.Join(", ", columnNames),
                    _columnNameResolver.ResolveColumnName(keyProperty),
                    keyProperty.Name);

                _updateQueryCache[type] = sql;
            }

            return connection.Execute(sql: sql, param: entity) > 0;
        }

        /// <summary>
        /// Deletes the specified entity from the database. 
        /// Returns a value indicating whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be deleted.</param>
        /// <returns>A value indicating whether the delete operation succeeded.</returns>
        public static bool Delete<TEntity>(this IDbConnection connection, TEntity entity)
        {
            var type = typeof(TEntity);

            string sql;
            if (!_deleteQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                var keyProperty = GetKeyProperty(type);
                string keyColumnName = GetColumnName(type, keyProperty);

                sql = string.Format("delete from {0} where {1} = @{2}", tableName, keyColumnName, keyProperty.Name);
            }
            return connection.Execute(sql, entity) > 0;
        }

        private static PropertyInfo GetKeyProperty(Type type)
        {
            PropertyInfo keyProperty;
            if (!_typeKeyPropertyCache.TryGetValue(type, out keyProperty))
            {
                keyProperty = _keyPropertyResolver.ResolveKeyProperty(type);
                _typeKeyPropertyCache[type] = keyProperty;
            }

            return keyProperty;
        }

        private static IEnumerable<PropertyInfo> GetTypeProperties(Type type)
        {
            PropertyInfo[] properties;
            if (!_typePropertiesCache.TryGetValue(type, out properties))
            {
                properties = type.GetProperties();
                _typePropertiesCache[type] = properties;
            }

            return properties;
        }

        private static string GetTableName(Type type)
        {
            string name;
            if (!_typeTableNameCache.TryGetValue(type, out name))
            {
                name = _tableNameResolver.ResolveTableName(type);
                _typeTableNameCache[type] = name;
            }
            return name;
        }

        private static string GetColumnName(Type type, PropertyInfo propertyInfo)
        {
            string key = string.Format("{0}.{1}", type.FullName, propertyInfo.Name);

            string columnName;
            if (!_columnNameCache.TryGetValue(key, out columnName))
            {
                columnName = _columnNameResolver.ResolveColumnName(propertyInfo);
                _columnNameCache[key] = columnName;
            }
            return columnName;
        }

        #region Key property resolving

        private static IKeyPropertyResolver _keyPropertyResolver = new DefaultKeyPropertyResolver();

        /// <summary>
        /// Sets the <see cref="DommelMapper.IKeyPropertyResolver"/> implementation for resolving key properties of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="DommelMapper.IKeyPropertyResolver"/>.</param>
        public static void SetKeyPropertyResolver(IKeyPropertyResolver resolver)
        {
            _keyPropertyResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving the key property of entities. 
        /// Custom implementations can be registerd with <see cref="M:SetKeyPropertyResolver()"/>.
        /// </summary>
        public interface IKeyPropertyResolver
        {
            /// <summary>
            /// Resolves the key property for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the key property for.</param>
            /// <returns>A <see cref="PropertyInfo"/> instance of the key property of <paramref name="type"/>.</returns>
            PropertyInfo ResolveKeyProperty(Type type);
        }

        /// <summary>
        /// Implements the <see cref="DommelMapper.IKeyPropertyResolver"/> interface by resolving key properties
        /// with the [Key] attribute or with the name 'Id'.
        /// </summary>
        private sealed class DefaultKeyPropertyResolver : IKeyPropertyResolver
        {
            /// <summary>
            /// Finds the key property by looking for a property with the [Key] attribute or with the name 'Id'.
            /// </summary>
            public PropertyInfo ResolveKeyProperty(Type type)
            {
                List<PropertyInfo> allProps = GetTypeProperties(type).ToList();

                // Look for properties with the [Key] attribute.
                List<PropertyInfo> keyProps = allProps.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

                if (keyProps.Count == 0)
                {
                    // Search for properties named as 'Id' as fallback.
                    keyProps = allProps.Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (keyProps.Count == 0)
                {
                    throw new Exception(string.Format("Could not find the key property for type '{0}'.", type.FullName));
                }

                if (keyProps.Count > 1)
                {
                    throw new Exception(string.Format("Multiple key properties were found for type '{0}'.", type.FullName));
                }

                return keyProps[0];
            }
        }

        #endregion

        #region Table name resolving

        private static ITableNameResolver _tableNameResolver = new DefaultTableNameResolver();

        /// <summary>
        /// Sets the <see cref="T:Dommel.ITableNameResolver"/> implementation for resolving table names for entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="T:Dommel.ITableNameResolver"/>.</param>
        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving table names of entities. 
        /// Custom implementations can be registerd with <see cref="M:SetTableNameResolver()"/>.
        /// </summary>
        public interface ITableNameResolver
        {
            /// <summary>
            /// Resolves the table name for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the table name for.</param>
            /// <returns>A string containing the resolved table name for for <paramref name="type"/>.</returns>
            string ResolveTableName(Type type);
        }

        /// <summary>
        /// Implements the <see cref="T:Dommel.ITableNameResolver"/> interface by resolving table names 
        /// by making the type name plural and removing the 'I' prefix for interfaces. 
        /// </summary>
        private sealed class DefaultTableNameResolver : ITableNameResolver
        {
            /// <summary>
            /// Resolves the table name by making the type plural (+ 's', Product -> Products) 
            /// and removing the 'I' prefix for interfaces.
            /// </summary>
            public string ResolveTableName(Type type)
            {
                string name = type.Name + "s";

                if (type.IsInterface && name.StartsWith("I"))
                {
                    name = name.Substring(1);
                }

                // todo: add [Table] attribute support.
                return name;
            }
        }

        #endregion

        #region Column name resolving

        private static IColumnNameResolver _columnNameResolver = new DefaultColumnNameResolver();

        /// <summary>
        /// Sets the <see cref="T:Dommel.IColumnNameResolver"/> implementation for resolving column names.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="T:Dommel.IColumnNameResolver"/>.</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver)
        {
            _columnNameResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving column names for entities. 
        /// Custom implementations can be registerd with <see cref="M:SetColumnNameResolver()"/>.
        /// </summary>
        public interface IColumnNameResolver
        {
            /// <summary>
            /// Resolves the column name for the specified property.
            /// </summary>
            /// <param name="propertyInfo">The property of the entity.</param>
            /// <returns>The column name for the property.</returns>
            string ResolveColumnName(PropertyInfo propertyInfo);
        }

        /// <summary>
        /// Implements the <see cref="DommelMapper.IKeyPropertyResolver"/>.
        /// </summary>
        private sealed class DefaultColumnNameResolver : IColumnNameResolver
        {
            /// <summary>
            /// Resolves the column name for the property. This is just the name of the property.
            /// </summary>
            public string ResolveColumnName(PropertyInfo propertyInfo)
            {
                return propertyInfo.Name;
            }
        }

        #endregion

        #region Sql builders

        /// <summary>
        /// Adds a custom implementation of <see cref="T:DommelMapper.ISqlBuilder"/> 
        /// for the specified ADO.NET connection type.
        /// </summary>
        /// <param name="connectionType">
        /// The ADO.NET conncetion type to use the <paramref name="builder"/> with. 
        /// Example: <c>typeof(SqlConnection)</c>.
        /// </param>
        /// <param name="builder">An implementation of the <see cref="T:DommelMapper.ISqlBuilder interface"/>.</param>
        public static void AddSqlBuilder(Type connectionType, ISqlBuilder builder)
        {
            _sqlBuilders[connectionType.Name.ToLower()] = builder;
        }

        private static ISqlBuilder GetBuilder(IDbConnection connection)
        {
            string connectionName = connection.GetType().Name.ToLower();
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
            /// <param name="keyProperty">The key property. This can be used to query a specific column for the new id. This is optional.</param>
            /// <returns>An insert query including a query to fetch the new id.</returns>
            string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty);
        }

        private sealed class SqlServerSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return string.Format("set nocount on insert into {0} ({1}) values ({2}) select cast(scope_identity() as int)",
                    tableName,
                    string.Join(", ", columnNames),
                    string.Join(", ", paramNames));
            }
        }

        private sealed class SqlServerCeSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return string.Format("insert into {0} ({1}) values ({2}) select cast(@@IDENTITY as int)",
                    tableName,
                    string.Join(", ", columnNames),
                    string.Join(", ", paramNames));
            }
        }

        private sealed class SqliteSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                // todo: this needs testing
                return string.Format("insert into {0} ({1}) values ({2}) select last_insert_rowid() id", 
                    tableName, 
                    string.Join(", ", columnNames), 
                    string.Join(", ", paramNames));
            }
        }

        private sealed class MySqlSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                // todo: this needs testing
                return string.Format("insert into {0} ({1}) values ({2}) select LAST_INSERT_ID() id", 
                    tableName, 
                    string.Join(", ", columnNames), 
                    string.Join(", ", paramNames));
            }
        }

        private sealed class PostgresSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                // todo: this needs testing
                string sql = string.Format("insert into {0} ({1}) values ({2}) select last_insert_rowid() id", 
                                 tableName, 
                                 string.Join(", ", columnNames), 
                                 string.Join(", ", paramNames));

                if (keyProperty != null)
                {
                    string keyColumnName = GetColumnName(keyProperty.DeclaringType, keyProperty);

                    sql += " RETURNING " + keyColumnName;
                }
                else
                {
                    // todo: what behavior is desired here?
                    throw new Exception("A key property is required for the PostgresSqlBuilder.");
                }

                return sql;
            }
        }

        #endregion
    }
}
