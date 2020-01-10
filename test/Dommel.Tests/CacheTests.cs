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
        internal void SetsCache(QueryCacheType queryCacheType)
        {
            var cacheKey = new QueryCacheKey(queryCacheType, new DummySqlBuilder(), typeof(Foo));
            DommelMapper.QueryCache[cacheKey] = "blah";
            Assert.Equal("blah", DommelMapper.QueryCache[cacheKey]);
        }

        [Fact]
        public void IsEqual()
        {
            Assert.Equal(
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo)),
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo)));
        }

        [Fact]
        public void IsNotEqualCacheType()
        {
            Assert.NotEqual(
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo)),
                new QueryCacheKey(QueryCacheType.GetAll, new DummySqlBuilder(), typeof(Foo)));
        }

        [Fact]
        public void IsNotEqualBuilderType()
        {
            Assert.NotEqual(
                new QueryCacheKey(QueryCacheType.Get, new SqlServerSqlBuilder(), typeof(Foo)),
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo)));
        }

        [Fact]
        public void IsNotEqualEntityType()
        {
            Assert.NotEqual(
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Foo)),
                new QueryCacheKey(QueryCacheType.Get, new DummySqlBuilder(), typeof(Bar)));
        }

        private class Foo { }
        private class Bar { }
    }
}
