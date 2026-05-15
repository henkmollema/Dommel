using System;
using System.Linq.Expressions;
using Xunit;

namespace Dommel.Tests;

public class SelectExpressionTests
{
    private readonly SqlExpression<Product> _sqlExpression = new(new SqlServerSqlBuilder());

    [Fact]
    public void Select_AllProperties()
    {
        var sql = _sqlExpression
            .Select()
            .ToSql();
        Assert.Equal("select * from [Products]", sql);
    }

    [Fact]
    public void Select_ThrowsForNullSelector()
    {
        Assert.Throws<ArgumentNullException>("sql", () => _sqlExpression.Select((string)null!));
        Assert.Throws<ArgumentNullException>("selector", () => _sqlExpression.Select((Expression<Func<Product, object>>)null!));
    }

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

    [Fact]
    public void Select_CustomSql()
    {
        var sql = _sqlExpression
            .Select("max(Id)")
            .ToSql();
        Assert.Equal("select max(Id) from [Products]", sql);
    }

    [Fact]
    public void GroupBy_SingleColumn()
    {
        var sql = _sqlExpression
            .Select()
            .GroupBy(p => p.Name)
            .ToSql();
        Assert.Equal("select * from [Products] group by [Products].[FullName]", sql);
    }

    [Fact]
    public void GroupBy_MultipleColumns()
    {
        var sql = _sqlExpression
            .Select()
            .GroupBy(p => p.Name)
            .GroupBy(p => p.CategoryId)
            .ToSql();
        Assert.Equal("select * from [Products] group by [Products].[FullName], [Products].[CategoryId]", sql);
    }

    [Fact]
    public void GroupBy_WithWhere()
    {
        var sql = _sqlExpression
            .Select("count(*)")
            .Where(p => p.CategoryId == 1)
            .GroupBy(p => p.Name)
            .ToSql();
        Assert.Equal("select count(*) from [Products] where [Products].[CategoryId] = @p1 group by [Products].[FullName]", sql);
    }

    [Fact]
    public void GroupBy_WithOrderBy()
    {
        var sql = _sqlExpression
            .Select()
            .GroupBy(p => p.CategoryId)
            .OrderBy(p => p.Name)
            .ToSql();
        Assert.Equal("select * from [Products] group by [Products].[CategoryId] order by [Products].[FullName] asc", sql);
    }

    [Fact]
    public void GroupBy_WithCount()
    {
        var sql = _sqlExpression
            .Select("CategoryId, count(*)")
            .GroupBy(p => p.CategoryId)
            .ToSql();
        Assert.Equal("select CategoryId, count(*) from [Products] group by [Products].[CategoryId]", sql);
    }

    [Fact]
    public void GroupBy_ThrowsForNullSelector()
    {
        Assert.Throws<ArgumentNullException>(() => _sqlExpression.GroupBy((Expression<Func<Product, object?>>)null!));
    }
}