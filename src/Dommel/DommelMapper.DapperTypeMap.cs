using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Dommel
{
    /// <summary>
    /// Simple CRUD operations for Dapper.
    /// </summary>
    public static partial class DommelMapper
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo> ColumnNameCache = new ConcurrentDictionary<string, PropertyInfo>();

        static DommelMapper()
        {
            // Type mapper for [Column] attribute
            SqlMapper.TypeMapProvider = type => CreateMap(type);

            SqlMapper.ITypeMap CreateMap(Type t) => new CustomPropertyTypeMap(t,
                (type, columnName) =>
                {
                    var cacheKey = type + columnName;
                    if (!ColumnNameCache.TryGetValue(cacheKey, out var propertyInfo))
                    {
                        propertyInfo = type.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<ColumnAttribute>()?.Name == columnName || p.Name == columnName);
                        ColumnNameCache.TryAdd(cacheKey, propertyInfo);
                    }

                    return propertyInfo;
                });
        }
    }
}
