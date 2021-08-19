using Xunit;

namespace Dommel.Tests
{
    public class CacheTests
    {
        [Theory]
        [InlineData(QueryCacheType.Get)]
        [InlineData(QueryCacheType.GetByMultipleIds)]
        [InlineData(QueryCacheType.GetAll)]
        [InlineData(QueryCacheType.Project)]
        [InlineData(QueryCacheType.ProjectAll)]
        [InlineData(QueryCacheType.Count)]
        [InlineData(QueryCacheType.Insert)]
        [InlineData(QueryCacheType.Update)]
        [InlineData(QueryCacheType.Delete)]
        [InlineData(QueryCacheType.DeleteAll)]
        [InlineData(QueryCacheType.Any)]
        internal void SetsCache(QueryCacheType queryCacheType)
        {
            var cacheKey = new QueryCacheKey(queryCacheType, new DummySqlBuilder(), typeof(Foo), "table");
            DommelMapper.QueryCache[cacheKey] = "blah";
            Assert.Equal("blah", DommelMapper.QueryCache[cacheKey]);
        }

        [Fact]
        public void IsEqual()
        {
            Assert.Equal(
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo), "table"),
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo), "table"));
        }

        [Fact]
        public void IsNotEqualCacheType()
        {
            Assert.NotEqual(
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo), "table"),
                new QueryCacheKey(QueryCacheType.GetAll, new DummySqlBuilder(), typeof(Foo), "table"));
        }

        [Fact]
        public void IsNotEqualBuilderType()
        {
            Assert.NotEqual(
                new QueryCacheKey(QueryCacheType.Get, new SqlServerSqlBuilder(), typeof(Foo), "table"),
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo), "table"));
        }

        [Fact]
        public void IsNotEqualEntityType()
        {
            Assert.NotEqual(
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo), "table"),
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Bar), "table"));
        }

        private class Foo { }
        private class Bar { }
    }
}
