using System;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;

namespace Dommel
{
    public static partial class DommelMapper
    {
        internal enum QueryCacheType
        {
            Get,
            GetByMultipleIds,
            GetAll,
            Project,
            ProjectAll,
            Count,
            Insert,
            Update,
            Delete,
            DeleteAll,
        }

        internal static ConcurrentDictionary<QueryCacheKey, string> QueryCache { get; } = new ConcurrentDictionary<QueryCacheKey, string>();

        internal struct QueryCacheKey : IEquatable<QueryCacheKey>
        {
            public QueryCacheKey(QueryCacheType cacheType, IDbConnection connection, MemberInfo memberInfo)
            {
                ConnectionType = connection.GetType();
                CacheType = cacheType;
                MemberInfo = memberInfo;
            }

#if NETSTANDARD1_3
            public QueryCacheKey(QueryCacheType cacheType, IDbConnection connection, Type type)
            {
                ConnectionType = connection.GetType();
                CacheType = cacheType;
                MemberInfo = type.GetTypeInfo();
            }
#endif

            public QueryCacheType CacheType { get; }

            public Type ConnectionType { get; }

            public MemberInfo MemberInfo { get; }

            public bool Equals(QueryCacheKey other) => CacheType == other.CacheType && ConnectionType == other.ConnectionType && MemberInfo == other.MemberInfo;
        }
    }
}
