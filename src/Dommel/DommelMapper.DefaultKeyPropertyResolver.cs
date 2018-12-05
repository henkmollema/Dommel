using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <inheritdoc />
        /// <summary>
        /// Implements the <see cref="T:Dommel.DommelMapper.IKeyPropertyResolver" /> interface by resolving key properties
        /// with the [Key] attribute or with the name 'Id'.
        /// </summary>
        public class DefaultKeyPropertyResolver : IKeyPropertyResolver
        {
            /// <inheritdoc />
            /// <summary>
            /// Finds the key properties by looking for properties with the [Key] attribute.
            /// </summary>
            public virtual IEnumerable<PropertyInfo> ResolveKeyProperties(Type type)
            {
                var keyProps = Resolvers
                        .Properties(type)
                        .Where(p => string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) || p.GetCustomAttribute<KeyAttribute>() != null)
                        .ToArray();

                if (keyProps.Length == 0)
                {
                    throw new InvalidOperationException($"Could not find the key properties for type '{type.FullName}'.");
                }

                return keyProps;
            }
        }
    }
}
