using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Defines methods for resolving column names for entities.
        /// Custom implementations can be registerd with <see cref="SetColumnNameResolver(IColumnNameResolver)"/>.
        /// </summary>
        public interface IColumnNameResolver
        {
            /// <summary>
            /// Resolves the column name for the specified property.
            /// </summary>
            /// <param name="propertyInfo">The property of the entity.</param>
            /// <returns>The column name for the property.</returns>
            string ResolveColumnName(PropertyInfo propertyInfo);
        }
    }
}
