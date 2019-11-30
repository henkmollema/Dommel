using System;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
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
        public void Select_ThrowsForNullSelector()
        {
            Assert.Throws<ArgumentNullException>("selector", () => _sqlExpression.Select(null!));
        }

        [Fact]
        public void Select_SingleProperty()
        {
            var sql = _sqlExpression
                .Select(p => new { p.Id })
                .ToSql();
            Assert.Equal("select [Id] from [Products]", sql);
        }

        [Fact]
        public void Select_MultipleProperties()
        {
            var sql = _sqlExpression
                .Select(p => new { p.Id, p.Name })
                .ToSql();
            Assert.Equal("select [Id], [Name] from [Products]", sql);
        }
    }
}
