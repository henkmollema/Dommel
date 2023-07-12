using System;
using System.Reflection;

namespace Dommel;

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
    Any,
}

internal readonly struct QueryCacheKey : IEquatable<QueryCacheKey>
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

    public readonly bool Equals(QueryCacheKey other) => CacheType == other.CacheType && SqlBuilderType == other.SqlBuilderType && MemberInfo == other.MemberInfo;

    public override bool Equals(object? obj) => obj is QueryCacheKey key && Equals(key);

    public override int GetHashCode() => CacheType.GetHashCode() + SqlBuilderType.GetHashCode() + MemberInfo.GetHashCode();
}
