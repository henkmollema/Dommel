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
            /// Finds the key property by looking for a property with the [Key] attribute or with the name 'Id'.
            /// </summary>
            public virtual PropertyInfo ResolveKeyProperty(Type type)
            {
                bool isIdentity;
                return ResolveKeyProperty(type, out isIdentity);
            }

            /// <summary>
            /// Finds the key property by looking for a property with the [Key] attribute or with the name 'Id'.
            /// </summary>
            public PropertyInfo ResolveKeyProperty(Type type, out bool isIdentity)
            {
                var allProps = Resolvers.Properties(type).ToList();

                // Look for properties with the [Key] attribute.
                var keyProps = allProps.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

                if (keyProps.Count == 0)
                {
                    // Search for properties named as 'Id' as fallback.
                    keyProps = allProps.Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (keyProps.Count == 0)
                {
                    throw new Exception($"Could not find the key property for type '{type.FullName}'.");
                }

                if (keyProps.Count > 1)
                {
                    throw new Exception($"Multiple key properties were found for type '{type.FullName}'.");
                }

                isIdentity = true;
                return keyProps[0];
            }
        }
    }
}
