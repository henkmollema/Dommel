using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        /// Helper class for retrieving type metadata to build sql queries using configured resolvers.
        /// </summary>
        public static class Resolvers
        {
            private static readonly IDictionary<Type, string> _typeTableNameCache = new Dictionary<Type, string>();
            private static readonly IDictionary<string, string> _columnNameCache = new Dictionary<string, string>();
            private static readonly IDictionary<Type, PropertyInfo> _typeKeyPropertyCache = new Dictionary<Type, PropertyInfo>();
            private static readonly IDictionary<Type, PropertyInfo[]> _typePropertiesCache = new Dictionary<Type, PropertyInfo[]>();
            private static readonly IDictionary<string, ForeignKeyInfo> _typeForeignKeyPropertyCache = new Dictionary<string, ForeignKeyInfo>();

            /// <summary>
            /// Gets the key property for the specified type, using the configured <see cref="IKeyPropertyResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="Type"/> to get the key property for.</param>
            /// <returns>The key property for <paramref name="type"/>.</returns>
            public static PropertyInfo KeyProperty(Type type)
            {
                PropertyInfo keyProperty;
                if (!_typeKeyPropertyCache.TryGetValue(type, out keyProperty))
                {
                    keyProperty = _keyPropertyResolver.ResolveKeyProperty(type);
                    _typeKeyPropertyCache[type] = keyProperty;
                }

                return keyProperty;
            }

            /// <summary>
            /// Gets the foreign key property for the specified source type and including type
            /// using the configure d<see cref="IForeignKeyPropertyResolver"/>.
            /// </summary>
            /// <param name="sourceType">The source type which should contain the foreign key property.</param>
            /// <param name="includingType">The type of the foreign key relation.</param>
            /// <param name="foreignKeyRelation">The foreign key relationship type.</param>
            /// <returns>The foreign key property for <paramref name="sourceType"/> and <paramref name="includingType"/>.</returns>
            public static PropertyInfo ForeignKeyProperty(Type sourceType, Type includingType, out ForeignKeyRelation foreignKeyRelation)
            {
                var key = $"{sourceType.FullName};{includingType.FullName}";

                ForeignKeyInfo foreignKeyInfo;
                if (!_typeForeignKeyPropertyCache.TryGetValue(key, out foreignKeyInfo))
                {
                    // Resole the property and relation.
                    var foreignKeyProperty = _foreignKeyPropertyResolver.ResolveForeignKeyProperty(sourceType, includingType, out foreignKeyRelation);

                    // Cache the info.
                    foreignKeyInfo = new ForeignKeyInfo(foreignKeyProperty, foreignKeyRelation);
                    _typeForeignKeyPropertyCache[key] = foreignKeyInfo;
                }

                foreignKeyRelation = foreignKeyInfo.Relation;
                return foreignKeyInfo.PropertyInfo;
            }

            private class ForeignKeyInfo
            {
                public ForeignKeyInfo(PropertyInfo propertyInfo, ForeignKeyRelation relation)
                {
                    PropertyInfo = propertyInfo;
                    Relation = relation;
                }

                public PropertyInfo PropertyInfo { get; }

                public ForeignKeyRelation Relation { get; }
            }

            /// <summary>
            /// Gets the properties to be mapped for the specified type, using the configured
            /// <see cref="DommelMapper.IPropertyResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="System.Type"/> to get the properties from.</param>
            /// <returns>>The collection of to be mapped properties of <paramref name="type"/>.</returns>
            public static IEnumerable<PropertyInfo> Properties(Type type)
            {
                PropertyInfo[] properties;
                if (!_typePropertiesCache.TryGetValue(type, out properties))
                {
                    properties = _propertyResolver.ResolveProperties(type).ToArray();
                    _typePropertiesCache[type] = properties;
                }

                return properties;
            }

            /// <summary>
            /// Gets the name of the table in the database for the specified type,
            /// using the configured <see cref="DommelMapper.ITableNameResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="System.Type"/> to get the table name for.</param>
            /// <returns>The table name in the database for <paramref name="type"/>.</returns>
            public static string Table(Type type)
            {
                string name;
                if (!_typeTableNameCache.TryGetValue(type, out name))
                {
                    name = _tableNameResolver.ResolveTableName(type);
                    _typeTableNameCache[type] = name;
                }
                return name;
            }

            /// <summary>
            /// Gets the name of the column in the database for the specified type,
            /// using the configured <see cref="T:DommelMapper.IColumnNameResolver"/>.
            /// </summary>
            /// <param name="propertyInfo">The <see cref="System.Reflection.PropertyInfo"/> to get the column name for.</param>
            /// <returns>The column name in the database for <paramref name="propertyInfo"/>.</returns>
            public static string Column(PropertyInfo propertyInfo)
            {
                var key = $"{propertyInfo.DeclaringType}.{propertyInfo.Name}";

                string columnName;
                if (!_columnNameCache.TryGetValue(key, out columnName))
                {
                    columnName = _columnNameResolver.ResolveColumnName(propertyInfo);
                    _columnNameCache[key] = columnName;
                }

                return columnName;
            }

            /// <summary>
            /// Provides access to default resolver implementations.
            /// </summary>
            public static class Default
            {
                /// <summary>
                /// The default column name resolver.
                /// </summary>
                public static readonly IColumnNameResolver ColumnNameResolver = new DefaultColumnNameResolver();

                /// <summary>
                /// The default property resolver.
                /// </summary>
                public static readonly IPropertyResolver PropertyResolver = new DefaultPropertyResolver();

                /// <summary>
                /// The default key property resolver.
                /// </summary>
                public static readonly IKeyPropertyResolver KeyPropertyResolver = new DefaultKeyPropertyResolver();

                /// <summary>
                /// The default table name resolver.
                /// </summary>
                public static readonly ITableNameResolver TableNameResolver = new DefaultTableNameResolver();
            }
        }
    }
}
