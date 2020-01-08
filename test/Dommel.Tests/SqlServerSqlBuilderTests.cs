﻿using Xunit;

namespace Dommel.Tests
{
    public class SqlServerSqlBuilderTests
    {
        private readonly SqlServerSqlBuilder _builder = new SqlServerSqlBuilder();

        [Fact]
        public void BuildInsert()
        {
            var sql = _builder.BuildInsert(typeof(Product), "Foos", new[] { "Name", "Bar" }, new[] { "@Name", "@Bar" });
            Assert.Equal("set nocount on insert into Foos (Name, Bar) values (@Name, @Bar); select scope_identity()", sql);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 15)]
        [InlineData(3, 30)]
        public void BuildPaging(int pageNumber, int start)
        {
            var sql = _builder.BuildPaging("asc", pageNumber, 15);
            Assert.Equal($" asc offset {start} rows fetch next 15 rows only", sql);
        }

        [Fact]
        public void PrefixParameter() => Assert.Equal("@Foo", _builder.PrefixParameter("Foo"));

        [Fact]
        public void QuoteIdentifier() => Assert.Equal("[Foo]", _builder.QuoteIdentifier("Foo"));
    }
}
