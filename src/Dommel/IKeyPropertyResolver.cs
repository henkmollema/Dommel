using System;
using System.Reflection;

namespace Dommel;

/// <summary>
/// Defines methods for resolving the key property of entities.
/// Custom implementations can be registered with <see cref="DommelMapper.SetKeyPropertyResolver(IKeyPropertyResolver)"/>.
/// </summary>
public interface IKeyPropertyResolver
{
    /// <summary>
    /// Resolves the key properties for the specified type.
    /// </summary>
    /// <param name="type">The type to resolve the key properties for.</param>
    /// <returns>A collection of <see cref="PropertyInfo"/> instances of the key properties of <paramref name="type"/>.</returns>
    ColumnPropertyInfo[] ResolveKeyProperties(Type type);
}
