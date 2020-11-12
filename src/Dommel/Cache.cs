using System;
using System.Reflection;

namespace Dommel
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
        Any,
    }

    internal struct QueryCacheKey : IEquatable<QueryCacheKey>
    {
        public QueryCacheKey(QueryCacheType cacheType, ISqlBuilder sqlBuilder, Type entityType)
        {
            SqlBuilderType = sqlBuilder.GetType();
            CacheType = cacheType;
            EntityType = entityType;
        }

        public QueryCacheType CacheType { get; }

        public Type SqlBuilderType { get; }

        public Type EntityType { get; }

        public bool Equals(QueryCacheKey other) => CacheType == other.CacheType && SqlBuilderType == other.SqlBuilderType && EntityType == other.EntityType;
    }
}
