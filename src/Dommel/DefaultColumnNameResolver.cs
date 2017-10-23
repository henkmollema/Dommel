using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dommel
{
    /// <summary>
    /// Implements the <see cref="DommelMapper.IKeyPropertyResolver"/>.
    /// </summary>
    public class DefaultColumnNameResolver : IColumnNameResolver
    {
        /// <summary>
        /// Resolves the column name for the property. This is just the name of the property.
        /// </summary>
        public virtual string ResolveColumnName(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name;
        }
    }
}
