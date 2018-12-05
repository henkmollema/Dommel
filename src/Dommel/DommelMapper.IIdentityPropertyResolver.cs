using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Defines methods for resolving the identity property of entities.
        /// Identity refers to the behaviour that property being assigned an auto-generating number in the data source which could also be achieved using Sequence (SQL Server),
        /// not necessarily an Identity column itself.
        /// Custom implementations can be registerd with <see cref="DommelMapper.SetIdentityPropertyResolver(IIdentityPropertyResolver)"/>.
        /// </summary>
        public interface IIdentityPropertyResolver
        {
            /// <summary>
            /// Resolves the identity properties for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the identity properties for.</param>
            /// <returns>A collection of <see cref="PropertyInfo"/> instances of the identity properties of <paramref name="type"/>.</returns>
            IEnumerable<PropertyInfo> ResolveIdentityProperties(Type type);
        }
    }

    /// <summary>
    /// Extensions for <see cref="DommelMapper.IIdentityPropertyResolver"/>.
    /// </summary>
    public static class IdentityPropertyResolverExtensions
    {
        /// <summary>
        /// Resolves the single identity property for the specified type.
        /// </summary>
        /// <param name="identityPropertyResolver">The <see cref="DommelMapper.IIdentityPropertyResolver"/>.</param>
        /// <param name="type">The type to resolve the identity property for.</param>
        /// <returns>A <see cref="PropertyInfo"/> instance of the identity property of <paramref name="type"/>.</returns>
        public static PropertyInfo ResolveIdentityProperty(this DommelMapper.IIdentityPropertyResolver identityPropertyResolver, Type type)
            => identityPropertyResolver.ResolveIdentityProperties(type).FirstOrDefault();
    }
}