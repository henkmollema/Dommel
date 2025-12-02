using System;
using Xunit;

namespace Dommel.Tests;

public class SqlExpressionTests
{
    private readonly SqlExpression<Product> _sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());

    [Fact]
    public void ToString_ReturnsSql()
    {
        var sql = _sqlExpression.Where(p => p.Name == "Chai").ToSql();
        Assert.Equal(" where [Products].[FullName] = @p1", sql);
    }

    [Fact]
    public void ToStringVisitation_ReturnsSql()
    {
        var sql = _sqlExpression.Where(p => p.CategoryId.ToString() == "1").ToSql();
        Assert.Equal(" where CAST([Products].[CategoryId] AS CHAR) = @p1", sql);
    }

    [Fact]
    public void ToString_ThrowsWhenCalledWithArgument()
    {
        var ex = Assert.Throws<ArgumentException>(() => _sqlExpression.Where(p => p.CategoryId.ToString("n2") == "1"));
        Assert.Contains("ToString-expression should not contain any argument.", ex.Message);
    }

    [Fact]
    public void ToSql_ThrowsWhenSelectAndSetArePresent()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _sqlExpression
            .Select()
            .Set(p => p.Name, "Chai")
            .ToSql());
        Assert.Equal("A SQL expression cannot contain both a Select and Set statement.", ex.Message);
    }
}
