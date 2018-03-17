using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Default implementation of the <see cref="ITableNameResolver"/> interface.
        /// </summary>
        public class DefaultTableNameResolver : ITableNameResolver
        {
            /// <summary>
            /// Resolves the table name.
            /// Looks for the [Table] attribute. Otherwise by making the type
            /// plural (eg. Product -> Products) and removing the 'I' prefix for interfaces.
            /// </summary>
            public virtual string ResolveTableName(Type type)
            {
                var typeInfo = type.GetTypeInfo();
                var tableAttr = typeInfo.GetCustomAttribute<TableAttribute>();
                if (tableAttr != null)
                {
                    if (!string.IsNullOrEmpty(tableAttr.Schema))
                    {
                        return $"{tableAttr.Schema}.{tableAttr.Name}";
                    }

                    return tableAttr.Name;
                }

                // Fall back to plural of table name
                var name = type.Name;
                if (name.EndsWith("y", StringComparison.OrdinalIgnoreCase))
                {
                    // Category -> Categories
                    name = name.Remove(name.Length - 1) + "ies";
                }
                else if (!name.EndsWith("s", StringComparison.OrdinalIgnoreCase))
                {
                    // Product -> Products
                    name += "s";
                }

                if (typeInfo.IsInterface && name.StartsWith("I", StringComparison.OrdinalIgnoreCase))
                {
                    // Remove leading I from interfaces
                    name = name.Substring(1);
                }

                return name;
            }
        }
    }
}
