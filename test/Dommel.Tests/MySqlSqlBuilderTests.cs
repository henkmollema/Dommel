using Xunit;

namespace Dommel.Tests;

public class MySqlSqlBuilderTests
{
    private readonly MySqlSqlBuilder _builder = new MySqlSqlBuilder();

    [Fact]
    public void BuildInsert()
    {
        var sql = _builder.BuildInsert(typeof(Product), "Foos", new[] { "Name", "Bar" }, new[] { "@Name", "@Bar" });
        Assert.Equal("insert into Foos (Name, Bar) values (@Name, @Bar); select LAST_INSERT_ID() id", sql);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 15)]
    [InlineData(3, 30)]
    public void BuildPaging(int pageNumber, int start)
    {
        var sql = _builder.BuildPaging("asc", pageNumber, 15);
        Assert.Equal($" asc limit {start}, 15", sql);
    }

    [Fact]
    public void PrefixParameter() => Assert.Equal("@Foo", _builder.PrefixParameter("Foo"));

    [Fact]
    public void QuoteIdentifier() => Assert.Equal("`Foo`", _builder.QuoteIdentifier("Foo"));

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void LimitClause(int count)
    {
        var sql = _builder.LimitClause(count);
        Assert.Equal($"limit {count}", sql);
    }
}
