using System;

using Xunit;

namespace Dommel.Tests;

public class SqlExpressionTests
{
    [Fact]
    public void Join()
    {
        var sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
        var name = "foo";
        sqlExpression.Join<Category>((x, y) => x.CategoryId == y.Id);
        sqlExpression.Where(x => x.Name == name);
        sqlExpression.AndWhere<Category>(x => x.Id > 5);
        sqlExpression.Select();
        var sql = sqlExpression.ToSql(out var param);
    }

    [Fact]
    public void ToString_ReturnsSql()
    {
        var sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
        var sql = sqlExpression.Where(p => p.Name == "Chai").ToSql();
        Assert.Equal(" where ([Products].[Name] = @p1)", sql);
        Assert.Equal(sql, sqlExpression.ToString());
    }

    [Fact]
    public void ToStringVisitation_ReturnsSql()
    {
        var sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
        var sql = sqlExpression.Where(p => p.CategoryId.ToString() == "1").ToSql();
        Assert.Equal(" where (CAST([Products].[CategoryId] AS CHAR) = @p1)", sql);
        Assert.Equal(sql, sqlExpression.ToString());
    }

    [Fact]
    public void ToString_ThrowsWhenCalledWithArgument()
    {
        var sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
        var ex = Assert.Throws<ArgumentException>(() => sqlExpression.Where(p => p.CategoryId.ToString("n2") == "1"));
        Assert.Contains("ToString-expression should not contain any argument.", ex.Message);
    }
}
