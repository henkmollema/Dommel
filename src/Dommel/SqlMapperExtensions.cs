using System;
using System.Collections.Generic;
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
        private static readonly IDictionary<Type, IEnumerable<PropertyInfo>> _typePropertiesCache = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        public static T Get<T>(this IDbConnection con, object id, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : class
        {
            Type type = typeof(T);

            string tableName = ResolveTableName(type);
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
                // todo: create KeyPropertyResolver.
                var keyProps = GetTypeProperties(type).Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)).ToList();

                if (keyProps.Count == 0)
                {
                    throw new Exception(string.Format("Could not find the key property for type '{0}'.", type.FullName));
                }

                if (keyProps.Count > 1)
                {
                    throw new Exception(string.Format("Multiple key properties were found for type '{0}'.", type.FullName));
                }

                keyProperty = keyProps[0];
                _typeKeyPropertyCache[type] = keyProperty;
            }

            return keyProperty;
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

        private static string ResolveTableName(Type type)
        {
            string result;
            if (!_typeTableNameCache.TryGetValue(type, out result))
            {
                result = _tableNameResolver.GetTableName(type);
                _typeTableNameCache[type] = result;
            }
            return result;
        }

        private static ITableNameResolver _tableNameResolver = new DefaultTableNameResolver();

        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }

        public interface ITableNameResolver
        {
            string GetTableName(Type type);
        }

        private sealed class DefaultTableNameResolver : ITableNameResolver
        {
            public string GetTableName(Type type)
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
    }
}
