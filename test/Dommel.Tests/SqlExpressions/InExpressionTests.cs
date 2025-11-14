using System;
using System.Linq;
using Xunit;

namespace Dommel.Tests;

public class InExpressionTests
{
    private readonly SqlExpression<Product> _sqlExpression = new(new SqlServerSqlBuilder());

    [Fact]
    public void Where_Contains_ReturnsInStatement()
    {
        var ids = new[] { 1, 2, 3 };
        var sql = _sqlExpression
            .Where(x => ids.Contains(x.Id))
            .ToSql();
        Assert.Equal("where [Products].[Id] in (@p1,@p2,@p3)", sql.Trim());
    }
}
