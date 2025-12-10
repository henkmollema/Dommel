using System;
using System.Linq;
using Xunit;

namespace Dommel.Tests;

public class InExpressionTests
{
    private readonly SqlExpression<Product> _sqlExpression = new(new SqlServerSqlBuilder());

    [Fact]
    public void Where_Contains_ReturnsInStatement_Ints()
    {
        int[] ids = [1, 2, 3];
        var sql = _sqlExpression
            .Where(x => ids.Contains(x.Id))
            .ToSql();
        Assert.Equal("where [Products].[Id] in (@p1,@p2,@p3)", sql.Trim());
    }

    [Fact]
    public void Where_Contains_ReturnsInStatement_IntsNullable()
    {
        int?[] ids = [1, 2, 3];
        var sql = _sqlExpression
            .Where(x => ids.Contains(x.Id))
            .ToSql();
        Assert.Equal("where [Products].[Id] in (@p1,@p2,@p3)", sql.Trim());
    }

    [Fact]
    public void Where_Contains_ReturnsInStatement_Strings()
    {
        string[] names = ["one", "two", "three"];
        var sql = _sqlExpression
            .Where(x => names.Contains(x.Name))
            .ToSql();
        Assert.Equal("where [Products].[FullName] in (@p1,@p2,@p3)", sql.Trim());
    }

    [Fact]
    public void Where_Contains_ReturnsInStatement_StringsNullable()
    {
        string?[] names = ["one", "two", "three"];
        var sql = _sqlExpression
            .Where(x => names.Contains(x.Name))
            .ToSql();
        Assert.Equal("where [Products].[FullName] in (@p1,@p2,@p3)", sql.Trim());
    }

    [Fact]
    public void Where_Contains_ReturnsInStatement_NullableInts()
    {
        int[]? ids = [1, 2, 3];
        var sql = _sqlExpression
            .Where(x => ids.Contains(x.Id))
            .ToSql();
        Assert.Equal("where [Products].[Id] in (@p1,@p2,@p3)", sql.Trim());
    }

    [Fact]
    public void Where_Contains_ReturnsInStatement_NullableIntsNullable()
    {
        int?[]? ids = [1, 2, 3];
        var sql = _sqlExpression
            .Where(x => ids.Contains(x.Id))
            .ToSql();
        Assert.Equal("where [Products].[Id] in (@p1,@p2,@p3)", sql.Trim());
    }

    [Fact]
    public void Where_Contains_ReturnsInStatement_NullableStrings()
    {
        string[]? names = ["one", "two", "three"];
        var sql = _sqlExpression
            .Where(x => names.Contains(x.Name))
            .ToSql();
        Assert.Equal("where [Products].[FullName] in (@p1,@p2,@p3)", sql.Trim());
    }

    [Fact]
    public void Where_Contains_ReturnsInStatement_NullableStringsNullable()
    {
        string?[]? names = ["one", "two", "three"];
        var sql = _sqlExpression
            .Where(x => names.Contains(x.Name))
            .ToSql();
        Assert.Equal("where [Products].[FullName] in (@p1,@p2,@p3)", sql.Trim());
    }
}
