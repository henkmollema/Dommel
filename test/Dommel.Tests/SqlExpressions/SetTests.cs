using System;
using Xunit;

namespace Dommel.Tests;

public class SetTests
{
    private readonly SqlExpression<Product> _sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());

    [Fact]
    public void Set_UpdatesColumn()
    {
        var sql = _sqlExpression.Set(p => p.Name, "Chai").ToSql();
        Assert.Equal(" set [Products].[FullName] = @p1", sql);
    }

    [Fact]
    public void Set_UpdatesMultipleColumns()
    {
        var sql = _sqlExpression
            .Set(p => p.Name, "Chai")
            .Set(p => p.CategoryId, 1)
            .ToSql();
        Assert.Equal(" set [Products].[FullName] = @p1, [Products].[CategoryId] = @p2", sql);
    }

    [Fact]
    public void Set_WithWhere_ReturnsSql()
    {
        var sql = _sqlExpression
            .Set(p => p.Name, "Chai")
            .Where(p => p.Id == 1)
            .ToSql();
        Assert.Equal(" set [Products].[FullName] = @p1 where [Products].[Id] = @p2", sql);
    }

    [Fact]
    public void Set_NullValue_ReturnsSql()
    {
        var sql = _sqlExpression.Set(p => p.Name, null).ToSql();
        Assert.Equal(" set [Products].[FullName] = @p1", sql);
    }
}
