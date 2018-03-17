using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Default implemenation of the <see cref="IPropertyResolver"/> interface.
        /// </summary>
        public class DefaultPropertyResolver : IPropertyResolver
        {
            private static readonly HashSet<Type> _primitiveTypes = new HashSet<Type>
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
                typeof(byte[])
            };

            /// <summary>
            /// Resolves the properties to be mapped for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the properties to be mapped for.</param>
            /// <returns>A collection of <see cref="PropertyInfo"/>'s of the <paramref name="type"/>.</returns>
            public IEnumerable<PropertyInfo> ResolveProperties(Type type) => FilterComplexTypes(type.GetProperties());

            /// <summary>
            /// Gets a collection of types that are considered 'primitive' for Dommel but are not for the CLR.
            /// Override this to specify your own set of types.
            /// </summary>
            protected virtual HashSet<Type> PrimitiveTypes => _primitiveTypes;

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
    }
}
