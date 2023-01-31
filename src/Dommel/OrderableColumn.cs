using System.Linq.Expressions;
using System;

namespace Dommel
{

    /// <summary>
    /// Represents a column that will be ordered
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class OrderableColumn<TEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderableColumn{TEntity}"/>
        /// class specifing one column with a direction
        /// </summary>
        /// <param name="selector">The column to order by. E.g. <code>x => x.Name</code></param>
        /// <param name="direction">The direction of the sort</param>
        public OrderableColumn(Expression<Func<TEntity, object?>> selector, SortDirectionEnum direction)
        {
            Selector = selector;
            Direction = direction;
        }

        /// <summary>
        /// Gets the column
        /// </summary>
        public Expression<Func<TEntity, object?>> Selector { get; }

        /// <summary>
        /// Gets the sort direction
        /// </summary>
        public SortDirectionEnum Direction { get; }
    }
}
