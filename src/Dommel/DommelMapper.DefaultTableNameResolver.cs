using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        /// Implements the <see cref="ITableNameResolver"/> interface by resolving table names
        /// by attempting to read information from the <see cref="TableAttribute"/> or
        /// by making the type name plural and removing the 'I' prefix for interfaces.
        /// </summary>
        public class DefaultTableNameResolver : ITableNameResolver
        {
            /// <inheritdoc/>
            public virtual string ResolveTableName(Type type)
            {
                var tableAttr = type.GetTypeInfo().GetCustomAttribute<TableAttribute>(inherit: true);
                if (tableAttr != null)
                {
                    if (string.IsNullOrEmpty(tableAttr.Schema))
                    {
                        return $"{tableAttr.Schema}.{tableAttr.Name}";
                    }

                    return tableAttr.Name;
                }

                var name = type.Name + "s";
                if (type.GetTypeInfo().IsInterface && name.StartsWith("I"))
                {
                    // Strip I from interfaces
                    name = name.Substring(1);
                }

                return name;
            }
        }
    }
}
