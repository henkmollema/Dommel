using System;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Defines methods for resolving the key property of entities.
        /// Custom implementations can be registerd with <see cref="SetKeyPropertyResolver(IKeyPropertyResolver)"/>.
        /// </summary>
        public interface IKeyPropertyResolver
        {
            /// <summary>
            /// Resolves the key properties for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the key properties for.</param>
            /// <returns>A collection of <see cref="PropertyInfo"/> instances of the key properties of <paramref name="type"/>.</returns>
            PropertyInfo[] ResolveKeyProperties(Type type);

            /// <summary>
            /// Resolves the key properties for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the key properties for.</param>
            /// <param name="isIdentity">Indicates whether the key properties are identity properties.</param>
            /// <returns>A collection of <see cref="PropertyInfo"/> instances of the key properties of <paramref name="type"/>.</returns>
            PropertyInfo[] ResolveKeyProperties(Type type, out bool isIdentity);
        }
    }
}
