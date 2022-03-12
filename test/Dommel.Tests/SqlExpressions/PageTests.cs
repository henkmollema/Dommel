using Xunit;

namespace Dommel.Tests;

public class PageTests
{
    private readonly SqlExpression<Product> _sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());

    [Fact]
    public void GeneratesSql()
    {
        var sql = _sqlExpression.Page(1, 5).ToSql();
        Assert.Equal(" order by [Products].[Id] asc offset 0 rows fetch next 5 rows only", sql);
    }

    [Fact]
    public void Page_OrderBy()
    {
        var sql = _sqlExpression.Page(1, 5).OrderBy(p => p.CategoryId).ToSql();
        Assert.Equal(" order by [Products].[CategoryId] asc offset 0 rows fetch next 5 rows only", sql);
    }

    [Fact]
    public void OrderBy_Page_OrderBy()
    {
        var sql = _sqlExpression.OrderBy(p => p.Name).Page(1, 5).OrderByDescending(p => p.CategoryId).ToSql();
        Assert.Equal(" order by [Products].[FullName] asc, [Products].[CategoryId] desc offset 0 rows fetch next 5 rows only", sql);
    }
}