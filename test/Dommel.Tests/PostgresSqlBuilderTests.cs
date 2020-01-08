using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Tests
{
    public class PostgresSqlBuilderTests
    {
        private readonly PostgresSqlBuilder _builder = new PostgresSqlBuilder();

        [Fact]
        public void BuildInsert()
        {
            var sql = _builder.BuildInsert(typeof(Product), "Foos", new[] { "Name", "Bar" }, new[] { "@Name", "@Bar" });
            Assert.Equal("insert into Foos (Name, Bar) values (@Name, @Bar) returning (\"Id\")", sql);
        }

        [Fact]
        public void BuildInsert_Returning2()
        {
            var sql = _builder.BuildInsert(typeof(Foo), "Foos", new[] { "Name", "Bar" }, new[] { "@Name", "@Bar" });
            Assert.Equal("insert into Foos (Name, Bar) values (@Name, @Bar) returning (\"Id1\", \"Id2\")", sql);
        }

        private class Foo
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            public Guid Id1 { get; set; }

            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            public Guid Id2 { get; set; }
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 15)]
        [InlineData(3, 30)]
        public void BuildPaging(int pageNumber, int start)
        {
            var sql = _builder.BuildPaging("asc", pageNumber, 15);
            Assert.Equal($" asc offset {start} limit 15", sql);
        }

        [Fact]
        public void PrefixParameter() => Assert.Equal("@Foo", _builder.PrefixParameter("Foo"));

        [Fact]
        public void QuoteIdentifier() => Assert.Equal("\"Foo\"", _builder.QuoteIdentifier("Foo"));

        [Fact]
        public void BuildInsert_ThrowsWhenTypeIsNull()
        {
            var builder = new PostgresSqlBuilder();
            Assert.Throws<ArgumentNullException>("type", () => builder.BuildInsert(null!, null!, null!, null!));
        }
    }
}
