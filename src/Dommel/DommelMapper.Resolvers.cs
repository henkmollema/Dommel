using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        private static IPropertyResolver _propertyResolver = new DefaultPropertyResolver();
        private static IKeyPropertyResolver _keyPropertyResolver = new DefaultKeyPropertyResolver();
        private static IForeignKeyPropertyResolver _foreignKeyPropertyResolver = new DefaultForeignKeyPropertyResolver();
        private static ITableNameResolver _tableNameResolver = new DefaultTableNameResolver();
        private static IColumnNameResolver _columnNameResolver = new DefaultColumnNameResolver();

        /// <summary>
        /// Sets the <see cref="IPropertyResolver"/> implementation for resolving key of entities.
        /// </summary>
        /// <param name="propertyResolver">An instance of <see cref="IPropertyResolver"/>.</param>
        public static void SetPropertyResolver(IPropertyResolver propertyResolver) => _propertyResolver = propertyResolver;

        /// <summary>
        /// Sets the <see cref="IKeyPropertyResolver"/> implementation for resolving key properties of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IKeyPropertyResolver"/>.</param>
        public static void SetKeyPropertyResolver(IKeyPropertyResolver resolver) => _keyPropertyResolver = resolver;

        /// <summary>
        /// Sets the <see cref="IForeignKeyPropertyResolver"/> implementation for resolving foreign key properties.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IForeignKeyPropertyResolver"/>.</param>
        public static void SetForeignKeyPropertyResolver(IForeignKeyPropertyResolver resolver) => _foreignKeyPropertyResolver = resolver;

        /// <summary>
        /// Sets the <see cref="ITableNameResolver"/> implementation for resolving table names for entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="ITableNameResolver"/>.</param>
        public static void SetTableNameResolver(ITableNameResolver resolver) => _tableNameResolver = resolver;

        /// <summary>
        /// Sets the <see cref="IColumnNameResolver"/> implementation for resolving column names.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IColumnNameResolver"/>.</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver) => _columnNameResolver = resolver;

        /// <summary>
        /// Helper class for retrieving type metadata to build sql queries using configured resolvers.
        /// </summary>
        public static class Resolvers
        {
            private static readonly ConcurrentDictionary<Type, string> _typeTableNameCache = new ConcurrentDictionary<Type, string>();
            private static readonly ConcurrentDictionary<string, string> _columnNameCache = new ConcurrentDictionary<string, string>();
            private static readonly ConcurrentDictionary<Type, KeyPropertyInfo> _typeKeyPropertyCache = new ConcurrentDictionary<Type, KeyPropertyInfo>();
            private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _typePropertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
            private static readonly ConcurrentDictionary<string, ForeignKeyInfo> _typeForeignKeyPropertyCache = new ConcurrentDictionary<string, ForeignKeyInfo>();

            /// <summary>
            /// Gets the key property for the specified type, using the configured <see cref="IKeyPropertyResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="Type"/> to get the key property for.</param>
            /// <returns>The key property for <paramref name="type"/>.</returns>
            public static PropertyInfo KeyProperty(Type type) => KeyProperty(type, out _);

            /// <summary>
            /// Gets the key property for the specified type, using the configured <see cref="IKeyPropertyResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="Type"/> to get the key property for.</param>
            /// <param name="isIdentity">A value indicating whether the key is an identity.</param>
            /// <returns>The key property for <paramref name="type"/>.</returns>
            public static PropertyInfo KeyProperty(Type type, out bool isIdentity)
            {
                if (!_typeKeyPropertyCache.TryGetValue(type, out var keyProperty))
                {
                    var propertyInfo = _keyPropertyResolver.ResolveKeyProperty(type, out isIdentity);
                    keyProperty = new KeyPropertyInfo(propertyInfo, isIdentity);
                    _typeKeyPropertyCache.TryAdd(type, keyProperty);
                }

                isIdentity = keyProperty.IsIdentity;

                LogReceived?.Invoke($"Resolved property '{keyProperty.PropertyInfo}' (Identity: {isIdentity}) as key property for '{type.Name}'");
                return keyProperty.PropertyInfo;
            }

            /// <summary>
            /// Gets the foreign key property for the specified source type and including type
            /// using the configured <see cref="IForeignKeyPropertyResolver"/>.
            /// </summary>
            /// <param name="sourceType">The source type which should contain the foreign key property.</param>
            /// <param name="includingType">The type of the foreign key relation.</param>
            /// <param name="foreignKeyRelation">The foreign key relationship type.</param>
            /// <returns>The foreign key property for <paramref name="sourceType"/> and <paramref name="includingType"/>.</returns>
            public static PropertyInfo ForeignKeyProperty(Type sourceType, Type includingType, out ForeignKeyRelation foreignKeyRelation)
            {
                var key = $"{sourceType.FullName};{includingType.FullName}";
                if (!_typeForeignKeyPropertyCache.TryGetValue(key, out var foreignKeyInfo))
                {
                    // Resolve the property and relation.
                    var foreignKeyProperty = _foreignKeyPropertyResolver.ResolveForeignKeyProperty(sourceType, includingType, out foreignKeyRelation);

                    // Cache the info.
                    foreignKeyInfo = new ForeignKeyInfo(foreignKeyProperty, foreignKeyRelation);
                    _typeForeignKeyPropertyCache.TryAdd(key, foreignKeyInfo);
                }

                foreignKeyRelation = foreignKeyInfo.Relation;

                LogReceived?.Invoke($"Resolved property '{foreignKeyInfo.PropertyInfo.Name}' ({foreignKeyInfo.Relation.ToString()}) as foreign key between '{sourceType.Name}' and '{includingType.Name}'");
                return foreignKeyInfo.PropertyInfo;
            }

            /// <summary>
            /// Gets the properties to be mapped for the specified type, using the configured
            /// <see cref="IPropertyResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="Type"/> to get the properties from.</param>
            /// <returns>>The collection of to be mapped properties of <paramref name="type"/>.</returns>
            public static IEnumerable<PropertyInfo> Properties(Type type)
            {
                if (!_typePropertiesCache.TryGetValue(type, out var properties))
                {
                    properties = _propertyResolver.ResolveProperties(type).ToArray();
                    _typePropertiesCache.TryAdd(type, properties);
                }

                return properties;
            }

            /// <summary>
            /// Gets the name of the table in the database for the specified type,
            /// using the configured <see cref="ITableNameResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="Type"/> to get the table name for.</param>
            /// <returns>The table name in the database for <paramref name="type"/>.</returns>
            public static string Table(Type type)
            {
                if (!_typeTableNameCache.TryGetValue(type, out var name))
                {
                    name = _tableNameResolver.ResolveTableName(type);
                    if (EscapeCharacterStart != char.MinValue || EscapeCharacterEnd != char.MinValue)
                    {
                        name = EscapeCharacterStart + name + EscapeCharacterEnd;
                    }
                    _typeTableNameCache.TryAdd(type, name);
                }

                LogReceived?.Invoke($"Resolved table name '{name}' for '{type.Name}'");
                return name;
            }

            /// <summary>
            /// Gets the name of the column in the database for the specified type,
            /// using the configured <see cref="IColumnNameResolver"/>.
            /// </summary>
            /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to get the column name for.</param>
            /// <returns>The column name in the database for <paramref name="propertyInfo"/>.</returns>
            public static string Column(PropertyInfo propertyInfo)
            {
                var key = $"{propertyInfo.DeclaringType}.{propertyInfo.Name}";
                if (!_columnNameCache.TryGetValue(key, out var columnName))
                {
                    columnName = _columnNameResolver.ResolveColumnName(propertyInfo);
                    if (EscapeCharacterStart != char.MinValue || EscapeCharacterEnd != char.MinValue)
                    {
                        columnName = EscapeCharacterStart + columnName + EscapeCharacterEnd;
                    }
                    _columnNameCache.TryAdd(key, columnName);
                }

                LogReceived?.Invoke($"Resolved column name '{columnName}' for '{propertyInfo.Name}'");
                return columnName;
            }

            private class KeyPropertyInfo
            {
                public KeyPropertyInfo(PropertyInfo propertyInfo, bool isIdentity)
                {
                    PropertyInfo = propertyInfo;
                    IsIdentity = isIdentity;
                }

                public PropertyInfo PropertyInfo { get; }

                public bool IsIdentity { get; }
            }

            private class ForeignKeyInfo
            {
                public ForeignKeyInfo(PropertyInfo propertyInfo, ForeignKeyRelation relation)
                {
                    PropertyInfo = propertyInfo;
                    Relation = relation;
                }

                public PropertyInfo PropertyInfo { get; set; }

                public ForeignKeyRelation Relation { get; set; }
            }
        }
    }
}
