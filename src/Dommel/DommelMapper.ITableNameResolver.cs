using System;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        /// Sets the <see cref="ITableNameResolver"/> implementation for resolving table names for entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="ITableNameResolver"/>.</param>
        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving table names of entities.
        /// Custom implementations can be registered with <see cref="SetTableNameResolver(ITableNameResolver)"/>.
        /// </summary>
        public interface ITableNameResolver
        {
            /// <summary>
            /// Resolves the table name for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the table name for.</param>
            /// <returns>A string containing the resolved table name for for <paramref name="type"/>.</returns>
            string ResolveTableName(Type type);
        }
    }
}
