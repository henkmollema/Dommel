using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        /// Sets the <see cref="IColumnNameResolver"/> implementation for resolving column names.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IColumnNameResolver"/>.</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver)
        {
            _columnNameResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving column names for entities.
        /// Custom implementations can be registered with <see cref="SetColumnNameResolver(IColumnNameResolver)"/>.
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
