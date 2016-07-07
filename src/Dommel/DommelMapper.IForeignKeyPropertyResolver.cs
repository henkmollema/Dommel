using System;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        /// Sets the <see cref="IForeignKeyPropertyResolver"/> implementation for resolving foreign key properties.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IForeignKeyPropertyResolver"/>.</param>
        public static void SetForeignKeyPropertyResolver(IForeignKeyPropertyResolver resolver)
        {
            _foreignKeyPropertyResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving foreign key properties.
        /// Custom implementations can be registered with <see cref="SetForeignKeyPropertyResolver(IForeignKeyPropertyResolver)"/>.
        /// </summary>
        public interface IForeignKeyPropertyResolver
        {
            /// <summary>
            /// Resolves the foreign key property for the specified source type and including type.
            /// </summary>
            /// <param name="sourceType">The source type which should contain the foreign key property.</param>
            /// <param name="includingType">The type of the foreign key relation.</param>
            /// <param name="foreignKeyRelation">The foreign key relationship type.</param>
            /// <returns>The foreign key property for <paramref name="sourceType"/> and <paramref name="includingType"/>.</returns>
            PropertyInfo ResolveForeignKeyProperty(Type sourceType, Type includingType, out ForeignKeyRelation foreignKeyRelation);
        }
    }
}
