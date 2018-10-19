using System;
using System.Linq;
using System.Reflection;
using static Dommel.DommelMapper;

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

    /// <summary>
    /// Extensions for <see cref="IKeyPropertyResolver"/>.
    /// </summary>
    public static class KeyPropertyResolverExtensions
    {
        /// <summary>
        /// Resolves the single key property for the specified type.
        /// </summary>
        /// <param name="keyPropertyResolver">The <see cref="IKeyPropertyResolver"/>.</param>
        /// <param name="type">The type to resolve the key property for.</param>
        /// <returns>A <see cref="PropertyInfo"/> instance of the key property of <paramref name="type"/>.</returns>
        public static PropertyInfo ResolveKeyProperty(this IKeyPropertyResolver keyPropertyResolver, Type type)
            => keyPropertyResolver.ResolveKeyProperties(type).FirstOrDefault();

        /// <summary>
        /// Resolves the single key property for the specified type.
        /// </summary>
        /// <param name="keyPropertyResolver">The <see cref="IKeyPropertyResolver"/>.</param>
        /// <param name="type">The type to resolve the key property for.</param>
        /// <param name="isIdentity">Indicates whether the key properties are identity properties.</param>
        /// <returns>A <see cref="PropertyInfo"/> instance of the key property of <paramref name="type"/>.</returns>
        public static PropertyInfo ResolveKeyProperty(this IKeyPropertyResolver keyPropertyResolver, Type type, out bool isIdentity) =>
            keyPropertyResolver.ResolveKeyProperties(type, out isIdentity).FirstOrDefault();
    }
}
