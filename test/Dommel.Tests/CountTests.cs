using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests;

public class CountTests
{
    private static readonly ISqlBuilder SqlBuilder = new SqlServerSqlBuilder();

    [Fact]
    public void GeneratesCountAllSql()
    {
        var sql = BuildCountAllSql(SqlBuilder, typeof(Foo));
        Assert.Equal("select count(*) from [Foos]", sql);
    }

    [Fact]
    public void GeneratesCountSql()
    {
        var sql = BuildCountSql<Foo>(SqlBuilder, x => x.Bar == "Baz", out var parameters);
        Assert.Equal("select count(*) from [Foos] where [Foos].[Bar] = @p1", sql);
        Assert.Single(parameters.ParameterNames);
    }

    private class Foo
    {
        public string? Bar { get; set; }
    }
}
