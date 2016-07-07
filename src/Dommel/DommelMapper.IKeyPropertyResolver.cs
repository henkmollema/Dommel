using System;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        /// Sets the <see cref="IKeyPropertyResolver"/> implementation for resolving key properties of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IKeyPropertyResolver"/>.</param>
        public static void SetKeyPropertyResolver(IKeyPropertyResolver resolver)
        {
            _keyPropertyResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving the key property of entities.
        /// Custom implementations can be registered with <see cref="SetKeyPropertyResolver(IKeyPropertyResolver)"/>.
        /// </summary>
        public interface IKeyPropertyResolver
        {
            /// <summary>
            /// Resolves the key property for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the key property for.</param>
            /// <returns>A <see cref="PropertyInfo"/> instance of the key property of <paramref name="type"/>.</returns>
            PropertyInfo ResolveKeyProperty(Type type);
        }
    }
}
