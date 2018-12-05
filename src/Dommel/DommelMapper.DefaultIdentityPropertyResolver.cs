using Dommel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <inheritdoc />
        /// <summary>
        /// Implements the <see cref="T:Dommel.DommelMapper.IIdentityPropertyResolver" /> interface by resolving identity properties
        /// with the [Identity] attribute or with the name 'Id'.
        /// </summary>
        public class DefaultIdentityPropertyResolver : IIdentityPropertyResolver
        {
            /// <inheritdoc />
            /// <summary>
            /// Finds the identity properties by looking for properties with the [Identity] attribute.
            /// </summary>
            public virtual IEnumerable<PropertyInfo> ResolveIdentityProperties(Type type)
            {
                var identityProps = Resolvers
                    .Properties(type)
                    .Where(p => p.GetCustomAttribute<IdentityAttribute>() != null)
                    .ToArray();

                return identityProps;
            }
        }
    }
}