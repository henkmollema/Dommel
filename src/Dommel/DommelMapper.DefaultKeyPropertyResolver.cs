using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        /// Implements the <see cref="IKeyPropertyResolver"/> interface by resolving key properties
        /// with the <see cref="KeyAttribute"/> or with the name 'Id'.
        /// </summary>
        public class DefaultKeyPropertyResolver : IKeyPropertyResolver
        {
            /// <inheritdoc/>
            public virtual PropertyInfo ResolveKeyProperty(Type type)
            {
                PropertyInfo property = null;
                foreach (var p in Resolvers.Properties(type))
                {
                    if (p.GetCustomAttribute<KeyAttribute>(inherit: true) != null ||
                        p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        if (property != null)
                        {
                            throw new Exception($"Multiple key properties were found for type '{type.FullName}'.");
                        }

                        property = p;
                    }
                }

                if (property == null)
                {
                    throw new Exception($"Could not find the key property for type '{type.FullName}'.");
                }

                return property;
            }
        }
    }
}
