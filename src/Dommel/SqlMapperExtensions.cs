using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Dommel
{
    public static class SqlMapperExtensions
    {
        private static readonly IDictionary<Type, string> _typeTableNameCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, PropertyInfo> _typeKeyPropertyCache = new Dictionary<Type, PropertyInfo>();
        private static readonly IDictionary<Type, PropertyInfo[]> _typePropertiesCache = new Dictionary<Type, PropertyInfo[]>();

        private static readonly IDictionary<Type, string> _getQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _insertQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _updateQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _deleteQueryCache = new Dictionary<Type, string>();

        /// <summary>
        /// Retrieves the entity with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static TEntity Get<TEntity>(this IDbConnection connection, object id) where TEntity : class
        {
            var type = typeof (TEntity);

            string sql;
            if (!_getQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                var keyProperty = GetKeyProperty(type);

                // todo: support custom id colum name.
                sql = string.Format("select * from {0} where {1} = @Id", tableName, keyProperty.Name);
                _getQueryCache[type] = sql;
            }

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            return connection.Query<TEntity>(sql: sql, param: parameters).FirstOrDefault();
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
            var type = typeof (TEntity);

            string sql;
            if (!_insertQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                var typeProperties = GetTypeProperties(type);
                var keyProperty = GetKeyProperty(type);

                // todo: support custom column names.
                string[] names = typeProperties.Where(p => p != keyProperty).Select(p => p.Name).ToArray();
                string[] paramNams = names.Select(s => "@" + s).ToArray();

                // todo: scope_identity() is not supported in sql ce.
                sql = string.Format("set nocount on insert into {0} ({1}) values ({2}) select cast(scope_identity() as int)",
                    tableName,
                    string.Join(", ", names),
                    string.Join(", ", paramNams));

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
            var type = typeof (TEntity);

            string sql;
            if (!_updateQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                var typeProperties = GetTypeProperties(type);
                var keyProperty = GetKeyProperty(type);

                // todo: support custom column names.
                string[] names = typeProperties.Where(p => p != keyProperty).Select(p => p.Name).ToArray();

                sql = string.Format("update {0} set {1} where {2} = @{2}",
                    tableName,
                    string.Join(", ", names.Select(n => n + " = @" + n)),
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
            var type = typeof (TEntity);

            string sql;
            if (!_deleteQueryCache.TryGetValue(type, out sql))
            {
                string tableName = GetTableName(type);
                var keyProperty = GetKeyProperty(type);

                sql = string.Format("delete from {0} where {1} = @{1}", tableName, keyProperty.Name);
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

        #region Key property resolving
        private static IKeyPropertyResolver _keyPropertyResolver = new DefaultKeyPropertyResolver();

        public static void SetKeyPropertyResolver(IKeyPropertyResolver resolver)
        {
            _keyPropertyResolver = resolver;
        }

        public interface IKeyPropertyResolver
        {
            PropertyInfo ResolveKeyProperty(Type type);
        }

        private sealed class DefaultKeyPropertyResolver : IKeyPropertyResolver
        {
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

        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }

        public interface ITableNameResolver
        {
            string ResolveTableName(Type type);
        }

        private sealed class DefaultTableNameResolver : ITableNameResolver
        {
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
    }
}
