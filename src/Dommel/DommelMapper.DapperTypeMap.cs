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
        private static readonly ConcurrentDictionary<string, PropertyInfo> _columnNameCache = new ConcurrentDictionary<string, PropertyInfo>();

        static DommelMapper()
        {
            SqlMapper.TypeMapProvider = type => CreateMap(type);

            SqlMapper.ITypeMap CreateMap(Type t) => new CustomPropertyTypeMap(t,
                (type, columnName) =>
                {
                    var cacheKey = type + columnName;
                    if (!_columnNameCache.TryGetValue(cacheKey, out var propertyInfo))
                    {
                        propertyInfo = type.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<ColumnAttribute>()?.Name == columnName || p.Name == columnName);
                        _columnNameCache.TryAdd(cacheKey, propertyInfo);
                    }

                    return propertyInfo;
                });
        }
    }
}
