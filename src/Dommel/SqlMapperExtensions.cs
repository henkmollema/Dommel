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

        public static T Get<T>(this IDbConnection con, object id, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : class
        {
            Type type = typeof(T);

            string tableName = GetTableName(type);
            PropertyInfo keyProperty = GetKeyProperty(type);

            string sql = string.Format("select * from {0} where {1} = @id", tableName, keyProperty.Name);

            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("@id", id);

            return con.Query<T>(sql: sql, param: dynamicParams, transaction: transaction, buffered: buffered, commandTimeout: commandTimeout, commandType: commandType).FirstOrDefault();
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
            private static readonly IDictionary<Type, IEnumerable<PropertyInfo>> _typePropertiesCache = new Dictionary<Type, IEnumerable<PropertyInfo>>();

            public PropertyInfo ResolveKeyProperty(Type type)
            {
                var allProps = GetTypeProperties(type).ToList();

                // Look for properties with the [Key] attribute.
                var keyProps = allProps.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

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

            private static IEnumerable<PropertyInfo> GetTypeProperties(Type type)
            {
                IEnumerable<PropertyInfo> properties;
                if (!_typePropertiesCache.TryGetValue(type, out properties))
                {
                    properties = type.GetProperties();
                    _typePropertiesCache[type] = properties;
                }

                return properties;
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
                // todo: add table attribute.
                return name;
            }
        }
        #endregion
    }
}
