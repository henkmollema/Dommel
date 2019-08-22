using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        internal static IPropertyResolver PropertyResolver = new DefaultPropertyResolver();
        internal static IKeyPropertyResolver KeyPropertyResolver = new DefaultKeyPropertyResolver();
        internal static IForeignKeyPropertyResolver ForeignKeyPropertyResolver = new DefaultForeignKeyPropertyResolver();
        internal static ITableNameResolver TableNameResolver = new DefaultTableNameResolver();
        internal static IColumnNameResolver ColumnNameResolver = new DefaultColumnNameResolver();

        /// <summary>
        /// Sets the <see cref="IPropertyResolver"/> implementation for resolving key of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IPropertyResolver"/>.</param>
        public static void SetPropertyResolver(IPropertyResolver resolver) => PropertyResolver = resolver;

        /// <summary>
        /// Sets the <see cref="IKeyPropertyResolver"/> implementation for resolving key properties of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IKeyPropertyResolver"/>.</param>
        public static void SetKeyPropertyResolver(IKeyPropertyResolver resolver) => KeyPropertyResolver = resolver;

        /// <summary>
        /// Sets the <see cref="IForeignKeyPropertyResolver"/> implementation for resolving foreign key properties.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IForeignKeyPropertyResolver"/>.</param>
        public static void SetForeignKeyPropertyResolver(IForeignKeyPropertyResolver resolver) => ForeignKeyPropertyResolver = resolver;

        /// <summary>
        /// Sets the <see cref="ITableNameResolver"/> implementation for resolving table names for entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="ITableNameResolver"/>.</param>
        public static void SetTableNameResolver(ITableNameResolver resolver) => TableNameResolver = resolver;

        /// <summary>
        /// Sets the <see cref="IColumnNameResolver"/> implementation for resolving column names.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IColumnNameResolver"/>.</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver) => ColumnNameResolver = resolver;

        /// <summary>
        /// Helper class for retrieving type metadata to build SQL queries using the configured resolvers.
        /// </summary>
        public static class Resolvers
        {
            private static readonly ConcurrentDictionary<string, string> _typeTableNameCache = new ConcurrentDictionary<string, string>();
            private static readonly ConcurrentDictionary<string, string> _columnNameCache = new ConcurrentDictionary<string, string>();
            private static readonly ConcurrentDictionary<Type, KeyPropertyInfo[]> _typeKeyPropertiesCache = new ConcurrentDictionary<Type, KeyPropertyInfo[]>();
            private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _typePropertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();
            private static readonly ConcurrentDictionary<string, ForeignKeyInfo> _typeForeignKeyPropertyCache = new ConcurrentDictionary<string, ForeignKeyInfo>();

            /// <summary>
            /// Gets the key properties for the specified type, using the configured <see cref="IKeyPropertyResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="Type"/> to get the key properties for.</param>
            /// <returns>The key properties for <paramref name="type"/>.</returns>
            public static KeyPropertyInfo[] KeyProperties(Type type)
            {
                if (!_typeKeyPropertiesCache.TryGetValue(type, out var keyProperties))
                {
                    keyProperties = KeyPropertyResolver.ResolveKeyProperties(type);
                    _typeKeyPropertiesCache.TryAdd(type, keyProperties);
                }

                LogReceived?.Invoke($"Resolved property '{string.Join(", ", keyProperties.Select(p => p.Property.Name))}' as key property for '{type}'");
                return keyProperties;
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
                var key = $"{sourceType};{includingType}";
                if (!_typeForeignKeyPropertyCache.TryGetValue(key, out var foreignKeyInfo))
                {
                    // Resolve the property and relation.
                    var foreignKeyProperty = ForeignKeyPropertyResolver.ResolveForeignKeyProperty(sourceType, includingType, out foreignKeyRelation);

                    // Cache the info.
                    foreignKeyInfo = new ForeignKeyInfo(foreignKeyProperty, foreignKeyRelation);
                    _typeForeignKeyPropertyCache.TryAdd(key, foreignKeyInfo);
                }

                foreignKeyRelation = foreignKeyInfo.Relation;

                LogReceived?.Invoke($"Resolved property '{foreignKeyInfo.PropertyInfo}' ({foreignKeyInfo.Relation}) as foreign key between '{sourceType}' and '{includingType}'");
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
                    properties = PropertyResolver.ResolveProperties(type).ToArray();
                    _typePropertiesCache.TryAdd(type, properties);
                }

                return properties;
            }
            /// <summary>
            /// Gets the name of the table in the database for the specified type,
            /// using the configured <see cref="ITableNameResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="Type"/> to get the table name for.</param>
            /// <param name="connection">The database connection instance.</param>
            /// <returns>The table name in the database for <paramref name="type"/>.</returns>
            public static string Table(Type type, IDbConnection connection) =>
                Table(type, GetSqlBuilder(connection));

            /// <summary>
            /// Gets the name of the table in the database for the specified type,
            /// using the configured <see cref="ITableNameResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="Type"/> to get the table name for.</param>
            /// <param name="sqlBuilder">The SQL builder instance.</param>
            /// <returns>The table name in the database for <paramref name="type"/>.</returns>
            public static string Table(Type type, ISqlBuilder sqlBuilder)
            {
                var key = $"{sqlBuilder.GetType()}.{type}";
                if (!_typeTableNameCache.TryGetValue(key, out var name))
                {
                    name = sqlBuilder.QuoteIdentifier(TableNameResolver.ResolveTableName(type));
                    _typeTableNameCache.TryAdd(key, name);
                }

                LogReceived?.Invoke($"Resolved table name '{name}' for '{type}'");
                return name;
            }

            /// <summary>
            /// Gets the name of the column in the database for the specified type,
            /// using the configured <see cref="IColumnNameResolver"/>.
            /// </summary>
            /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to get the column name for.</param>
            /// <param name="connection">The database connection instance.</param>
            /// <returns>The column name in the database for <paramref name="propertyInfo"/>.</returns>
            public static string Column(PropertyInfo propertyInfo, IDbConnection connection)
                => Column(propertyInfo, GetSqlBuilder(connection));

            /// <summary>
            /// Gets the name of the column in the database for the specified type,
            /// using the configured <see cref="IColumnNameResolver"/>.
            /// </summary>
            /// <param name="propertyInfo">The <see cref="PropertyInfo"/> to get the column name for.</param>
            /// <param name="sqlBuilder">The SQL builder instance.</param>
            /// <returns>The column name in the database for <paramref name="propertyInfo"/>.</returns>
            public static string Column(PropertyInfo propertyInfo, ISqlBuilder sqlBuilder)
            {
                var key = $"{sqlBuilder.GetType()}.{propertyInfo.DeclaringType}.{propertyInfo.Name}";
                if (!_columnNameCache.TryGetValue(key, out var columnName))
                {
                    columnName = sqlBuilder.QuoteIdentifier(ColumnNameResolver.ResolveColumnName(propertyInfo));
                    _columnNameCache.TryAdd(key, columnName);
                }

                LogReceived?.Invoke($"Resolved column name '{columnName}' for '{propertyInfo}'");
                return columnName;
            }

            private struct ForeignKeyInfo
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
