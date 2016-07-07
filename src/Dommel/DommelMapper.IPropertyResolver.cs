using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        private static IPropertyResolver _propertyResolver = new DefaultPropertyResolver();

        /// <summary>
        /// Defines methods for resolving the properties of entities.
        /// Custom implementations can be registerd with <see cref="SetPropertyResolver(IPropertyResolver)"/>.
        /// </summary>
        public interface IPropertyResolver
        {
            /// <summary>
            /// Resolves the properties to be mapped for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the properties to be mapped for.</param>
            /// <returns>A collection of <see cref="PropertyInfo"/>'s of the <paramref name="type"/>.</returns>
            IEnumerable<PropertyInfo> ResolveProperties(Type type);
        }

        /// <summary>
        /// Sets the <see cref="IPropertyResolver"/> implementation for resolving key of entities.
        /// </summary>
        /// <param name="propertyResolver">An instance of <see cref="IPropertyResolver"/>.</param>
        public static void SetPropertyResolver(IPropertyResolver propertyResolver)
        {
            _propertyResolver = propertyResolver;
        }
    }
}
