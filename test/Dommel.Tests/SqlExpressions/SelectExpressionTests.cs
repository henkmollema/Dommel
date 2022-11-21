using System;
using Xunit;

namespace Dommel.Tests;

public class SelectExpressionTests
{
    private readonly SqlExpression<Product> _sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());

    [Fact]
    public void Select_AllProperties()
    {
        var sql = _sqlExpression
            .Select()
            .ToSql();
        Assert.Equal("select * from [Products]", sql);
    }

    [Fact]
    public void Select_ThrowsForNullSelector() => Assert.Throws<ArgumentNullException>("selector", () => _sqlExpression.Select(null!));

    [Fact]
    public void Select_ThrowsForEmptyProjection()
    {
        var ex = Assert.Throws<ArgumentException>("selector", () => _sqlExpression.Select(x => new object()));
        Assert.Equal(new ArgumentException("Projection over type 'Product' yielded no properties.", "selector").Message, ex.Message);
    }

    [Fact]
    public void Select_SingleProperty()
    {
        var sql = _sqlExpression
            .Select(p => new { p.Id })
            .ToSql();
        Assert.Equal("select [Products].[Id] from [Products]", sql);
    }

    [Fact]
    public void Select_MultipleProperties()
    {
        var sql = _sqlExpression
            .Select(p => new { p.Id, p.Name })
            .ToSql();
        Assert.Equal("select [Products].[Id], [Products].[FullName] from [Products]", sql);
    }
}