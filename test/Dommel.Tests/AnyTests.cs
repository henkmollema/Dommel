using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests;

public class AnyTests
{
    private static readonly ISqlBuilder SqlBuilder = new SqlServerSqlBuilder();

    [Fact]
    public void GeneratesAnyAllSql()
    {
        var sql = BuildAnyAllSql(SqlBuilder, typeof(Foo));
        Assert.Equal($"select 1 from [Foos] {SqlBuilder.LimitClause(1)}", sql);
    }

    [Fact]
    public void GeneratesAnySql()
    {
        var sql = BuildAnySql<Foo>(SqlBuilder, x => x.Bar == "Baz", out var parameters);
        Assert.Equal($"select 1 from [Foos] where ([Foos].[Bar] = @p1) {SqlBuilder.LimitClause(1)}", sql);
        Assert.Single(parameters.ParameterNames);
    }

    private class Foo
    {
        public string? Bar { get; set; }
    }
}
