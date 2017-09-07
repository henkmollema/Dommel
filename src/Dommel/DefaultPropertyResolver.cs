using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dommel
{
    /// <summary>
    /// Default implemenation of the <see cref="DommelMapper.IPropertyResolver"/> interface.
    /// </summary>
    public class DefaultPropertyResolver : PropertyResolverBase
    {
        /// <inheritdoc/>
        public override IEnumerable<PropertyInfo> ResolveProperties(Type type)
        {
            return FilterComplexTypes(type.GetProperties());
        }
    }
}
