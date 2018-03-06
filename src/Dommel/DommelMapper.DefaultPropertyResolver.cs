using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Default implemenation of the <see cref="IPropertyResolver"/> interface.
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
}
