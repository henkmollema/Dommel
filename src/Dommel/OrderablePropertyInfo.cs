using System.Reflection;

namespace Dommel
{
    /// <summary>
    /// Represents a column that will be ordered
    /// </summary>
    public class OrderablePropertyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderablePropertyInfo"/>
        /// class specifiing one column with a direction
        /// </summary>
        /// <param name="property">The column to order by. </param>
        /// <param name="direction">The direction of the sort</param>
        public OrderablePropertyInfo(PropertyInfo property, SortDirectionEnum direction)
        {
            Property = property;
            Direction = direction;
        }

        /// <summary>
        /// Gets the property
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the sort direction
        /// </summary>
        public SortDirectionEnum Direction { get; }
    }
}
