using System;

using Xunit;

namespace Dommel.Tests;

public class SqlExpressionTests
{
    [Fact]
    public void ToString_ReturnsSql()
    {
        var sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
        var sql = sqlExpression.Where(p => p.Name == "Chai").ToSql();
        Assert.Equal(" where [Products].[FullName] = @p1", sql);
        Assert.Equal(sql, sqlExpression.ToString());
    }

    [Fact]
    public void ToStringVisitation_ReturnsSql()
    {
        var sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
        var sql = sqlExpression.Where(p => p.CategoryId.ToString() == "1").ToSql();
        Assert.Equal(" where CAST([Products].[CategoryId] AS CHAR) = @p1", sql);
        Assert.Equal(sql, sqlExpression.ToString());
    }

    [Fact]
    public void Where_TranslatesArithmetic()
    {
        var sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
        var sql = sqlExpression.Where(p => p.CategoryId * 2 == 4).ToSql();
        Assert.Equal(" where [Products].[CategoryId] * @p1 = @p2", sql);
    }

    [Fact]
    public void ToString_ThrowsWhenCalledWithArgument()
    {
        var sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
        var ex = Assert.Throws<ArgumentException>(() => sqlExpression.Where(p => p.CategoryId.ToString("n2") == "1"));
        Assert.Contains("ToString-expression should not contain any argument.", ex.Message);
    }
}