using System;
using Xunit;

namespace Dommel.Tests;

public class WhereExpressionTests
{
    private readonly SqlExpression<Product> _sqlExpression = new(new SqlServerSqlBuilder());

    [Fact]
    public void Where_AllowsNullExpression()
    {
        // Act & Assert
        Assert.NotNull(_sqlExpression.Where(null));
    }

    [Fact]
    public void AndWhere_ThrowsWhenStatementStarted()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _sqlExpression.AndWhere(_ => true));
        Assert.Equal("Start the where statement with the 'Where' method.", ex.Message);
    }

    [Fact]
    public void OrWhere_ThrowsWhenStatementStarted()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _sqlExpression.OrWhere(_ => true));
        Assert.Equal("Start the where statement with the 'Where' method.", ex.Message);
    }

    [Fact]
    public void Where_GeneratesSql()
    {
        var sql = _sqlExpression.Where(p => p.Name == "Chai").ToSql();
        Assert.Equal(" where [Products].[FullName] = @p1", sql);
    }

    [Fact]
    public void Where_AppendsWhenCalledTwice()
    {
        var sql = _sqlExpression
            .Where(p => p.Name == "Chai")
            .Where(p => p.CategoryId == 1 || p.CategoryId == 2 || p.CategoryId == 3)
            .ToSql();
        Assert.Equal(" where ([Products].[FullName] = @p1) and ([Products].[CategoryId] = @p2 or [Products].[CategoryId] = @p3 or [Products].[CategoryId] = @p4)", sql);
    }

    [Fact]
    public void Where_AndWhere_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(p => p.Name == "Chai")
            .AndWhere(p => p.CategoryId == 1 || p.CategoryId == 2)
            .ToSql();
        Assert.Equal(" where ([Products].[FullName] = @p1) and ([Products].[CategoryId] = @p2 or [Products].[CategoryId] = @p3)", sql);
    }

    [Fact]
    public void Where_AndWhere_GeneratesSql2()
    {
        var sql = _sqlExpression
            .Where(p => p.CategoryId == 1 || p.CategoryId == 2)
            .AndWhere(p => p.Name == "Chai")
            .ToSql();
        Assert.Equal(" where ([Products].[CategoryId] = @p1 or [Products].[CategoryId] = @p2) and ([Products].[FullName] = @p3)", sql);
    }

    [Fact]
    public void Where_AndWhereWithOr_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(p => p.Name == "Chai" || p.Name == "Latte")
            .AndWhere(p => p.CategoryId == 1 || p.CategoryId == 2)
            .ToSql();
        Assert.Equal(" where ([Products].[FullName] = @p1 or [Products].[FullName] = @p2) and ([Products].[CategoryId] = @p3 or [Products].[CategoryId] = @p4)", sql);
    }

    [Fact]
    public void Where_OrWhere_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(p => p.Name == "Chai")
            .OrWhere(p => p.CategoryId == 1)
            .ToSql();
        Assert.Equal(" where ([Products].[FullName] = @p1) or ([Products].[CategoryId] = @p2)", sql);
    }

    [Fact]
    public void Where_OrWhereWithAnd_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(p => p.Name == "Chai")
            .OrWhere(p => p.CategoryId == 1 && p.Name == "Latte")
            .ToSql();
        Assert.Equal(" where ([Products].[FullName] = @p1) or ([Products].[CategoryId] = @p2 and [Products].[FullName] = @p3)", sql);
    }

    [Fact]
    public void Where_OrStatement_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(p => p.Name == "Chai" || p.CategoryId == 1)
            .ToSql();
        Assert.Equal(" where [Products].[FullName] = @p1 or [Products].[CategoryId] = @p2", sql);
    }

    [Fact]
    public void Where_OrStatementThreeExpressions_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(p => p.Name == "Chai" || p.Name == "Latte" || p.CategoryId == 1)
            .ToSql();
        Assert.Equal(" where [Products].[FullName] = @p1 or [Products].[FullName] = @p2 or [Products].[CategoryId] = @p3", sql);
    }

    [Fact]
    public void Where_WithParentheses_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(x => x.Id > 0 && (x.Name == "Foo" || x.Name == "Bar"))
            .ToSql();
        Assert.Equal(" where [Products].[Id] > @p1 and ([Products].[FullName] = @p2 or [Products].[FullName] = @p3)", sql);
    }

    [Fact]
    public void Where_WithParentheses_GeneratesSql2()
    {
        var sql = _sqlExpression
            .Where(x => x.Id > 0 && (x.Name == "Foo" || x.Name == "Bar") && (x.CategoryId < 5 || x.CategoryId > 10))
            .ToSql();
        Assert.Equal(" where [Products].[Id] > @p1 and ([Products].[FullName] = @p2 or [Products].[FullName] = @p3) " +
            "and ([Products].[CategoryId] < @p4 or [Products].[CategoryId] > @p5)", sql);
    }

    [Fact]
    public void Where_WithParenthesesAndLike_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(x => x.Id > 0 && (x.Name!.Contains("Foo") || x.Name!.StartsWith("Bar")))
            .ToSql();
        Assert.Equal(" where [Products].[Id] > @p1 and ([Products].[FullName] like @p2 or [Products].[FullName] like @p3)", sql);
    }

    [Fact]
    public void Where_WithoutParentheses_GeneratesSql()
    {
        var sql = _sqlExpression
            .Where(x => x.Id > 0 && x.Name == "Foo" || x.Name == "Bar")
            .ToSql();
        Assert.Equal(" where ([Products].[Id] > @p1 and [Products].[FullName] = @p2) or [Products].[FullName] = @p3", sql);
    }

    [Fact]
    public void Where_WithoutParentheses_GeneratesSql2()
    {
        var sql = _sqlExpression
            .Where(x => x.Id > 0 || x.Name == "Foo" && x.Name == "Bar")
            .ToSql();
        Assert.Equal(" where [Products].[Id] > @p1 or ([Products].[FullName] = @p2 and [Products].[FullName] = @p3)", sql);
    }
}