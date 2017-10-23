using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dommel
{
    /// <summary>
    /// Defines methods for resolving the properties of entities.
    /// Custom implementations can be registerd with <see cref="M:SetPropertyResolver()"/>.
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
}
