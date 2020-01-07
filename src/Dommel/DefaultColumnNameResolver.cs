using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Implements the <see cref="IKeyPropertyResolver"/>.
        /// </summary>
        public class DefaultColumnNameResolver : IColumnNameResolver
        {
            /// <summary>
            /// Resolves the column name for the property.
            /// Looks for the [Column] attribute. Otherwise it's just the name of the property.
            /// </summary>
            public virtual string ResolveColumnName(PropertyInfo propertyInfo)
            {
                var columnAttr = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                if (columnAttr != null)
                {
                    return columnAttr.Name;
                }

                return propertyInfo.Name;
            }
        }
    }
}
