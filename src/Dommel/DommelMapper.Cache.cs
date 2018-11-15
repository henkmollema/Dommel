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
            Count,
            Insert,
            Update,
            Delete,
            DeleteAll,
        }

#pragma warning disable IDE1006 // Naming Styles
        internal static ConcurrentDictionary<QueryCacheKey, string> QueryCache = new ConcurrentDictionary<QueryCacheKey, string>();
        internal static ConcurrentDictionary<QueryCacheKey, string> ResolverCache = new ConcurrentDictionary<QueryCacheKey, string>();
#pragma warning restore IDE1006 // Naming Styles

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
