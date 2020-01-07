using System;
using System.Collections.Concurrent;
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
            public QueryCacheKey(QueryCacheType cacheType, ISqlBuilder sqlBuilder, MemberInfo memberInfo)
            {
                SqlBuilderType = sqlBuilder.GetType();
                CacheType = cacheType;
                MemberInfo = memberInfo;
            }

            public QueryCacheType CacheType { get; }

            public Type SqlBuilderType { get; }

            public MemberInfo MemberInfo { get; }

            public bool Equals(QueryCacheKey other) => CacheType == other.CacheType && SqlBuilderType == other.SqlBuilderType && MemberInfo == other.MemberInfo;
        }
    }
}
