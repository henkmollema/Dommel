using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Implements the <see cref="IKeyPropertyResolver"/> interface by resolving key properties
        /// with the [Key] attribute or with the name 'Id'.
        /// </summary>
        public class DefaultKeyPropertyResolver : IKeyPropertyResolver
        {
            /// <summary>
            /// Finds the key properties by looking for properties with the [Key] attribute.
            /// </summary>
            public PropertyInfo[] ResolveKeyProperties(Type type) => ResolveKeyProperties(type, out _);

            /// <summary>
            /// Finds the key properties by looking for properties with the [Key] attribute.
            /// </summary>
            public PropertyInfo[] ResolveKeyProperties(Type type, out bool isIdentity)
            {
                var keyProps = Resolvers
                        .Properties(type)
                        .Where(p => string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) || p.GetCustomAttribute<KeyAttribute>() != null)
                        .ToArray();

                if (keyProps.Length == 0)
                {
                    throw new InvalidOperationException($"Could not find the key properties for type '{type.FullName}'.");
                }

                isIdentity = true;
                return keyProps;
            }
        }
    }
}
