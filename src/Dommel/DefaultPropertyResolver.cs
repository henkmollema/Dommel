using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Dommel;

/// <summary>
/// Default implemenation of the <see cref="IPropertyResolver"/> interface.
/// </summary>
public class DefaultPropertyResolver : IPropertyResolver
{
    private static readonly HashSet<Type> PrimitiveTypesSet = new()
    {
        typeof(object),
        typeof(string),
        typeof(Guid),
        typeof(decimal),
        typeof(double),
        typeof(float),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(byte[]),
#if NET6_0_OR_GREATER
        typeof(DateOnly),
        typeof(TimeOnly),
#endif
    };

    /// <summary>
    /// Resolves the properties to be mapped for the specified type.
    /// </summary>
    /// <param name="type">The type to resolve the properties to be mapped for.</param>
    /// <returns>A collection of <see cref="PropertyInfo"/>'s of the <paramref name="type"/>.</returns>
    public virtual IEnumerable<ColumnPropertyInfo> ResolveProperties(Type type)
    {
        foreach (var property in FilterComplexTypes(type.GetRuntimeProperties()))
        {
            if (!property.IsDefined(typeof(IgnoreAttribute)) && !property.IsDefined(typeof(NotMappedAttribute)))
            {
                yield return new ColumnPropertyInfo(property);
            }
        }
    }

    /// <summary>
    /// Gets a collection of types that are considered 'primitive' for Dommel but are not for the CLR.
    /// Override this to specify your own set of types.
    /// </summary>
    protected virtual HashSet<Type> PrimitiveTypes => PrimitiveTypesSet;

    /// <summary>
    /// Filters the complex types from the specified collection of properties.
    /// </summary>
    /// <param name="properties">A collection of properties.</param>
    /// <returns>The properties that are considered 'primitive' of <paramref name="properties"/>.</returns>
    protected virtual IEnumerable<PropertyInfo> FilterComplexTypes(IEnumerable<PropertyInfo> properties)
    {
        foreach (var property in properties)
        {
            var type = property.PropertyType;
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type.GetTypeInfo().IsPrimitive || type.GetTypeInfo().IsEnum || PrimitiveTypes.Contains(type))
            {
                yield return property;
            }
        }
    }
}
