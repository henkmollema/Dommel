using Xunit;

namespace Dommel.Tests;

public class SqlServerCeSqlBuilderTests
{
    private readonly SqlServerCeSqlBuilder _builder = new SqlServerCeSqlBuilder();

    [Fact]
    public void BuildInsert()
    {
        var sql = _builder.BuildInsert(typeof(Product), "Foos", new[] { "Name", "Bar" }, new[] { "@Name", "@Bar" });
        Assert.Equal("insert into Foos (Name, Bar) values (@Name, @Bar); select @@IDENTITY", sql);
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

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void LimitClause(int count)
    {
        var sql = _builder.LimitClause(count);
        Assert.Equal($"order by 1 offset 0 rows fetch next {count} rows only", sql);
    }
}
