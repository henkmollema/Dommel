using System;
using Xunit;

namespace Dommel.Tests
{
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
            Assert.Equal(" where ([Products].[Name] = @p1)", sql);
        }

        [Fact]
        public void Where_AppendsWhenCalledTwice()
        {
            var sql = _sqlExpression
                .Where(p => p.Name == "Chai")
                .Where(p => p.CategoryId == 1)
                .ToSql();
            Assert.Equal(" where ([Products].[Name] = @p1) and ([Products].[CategoryId] = @p2)", sql);
        }

        [Fact]
        public void Where_AndWhere_GeneratesSql()
        {
            var sql = _sqlExpression
                .Where(p => p.Name == "Chai")
                .AndWhere(p => p.CategoryId == 1)
                .ToSql();
            Assert.Equal(" where ([Products].[Name] = @p1) and ([Products].[CategoryId] = @p2)", sql);
        }

        [Fact]
        public void Where_OrWhere_GeneratesSql()
        {
            var sql = _sqlExpression
                .Where(p => p.Name == "Chai")
                .OrWhere(p => p.CategoryId == 1)
                .ToSql();
            Assert.Equal(" where ([Products].[Name] = @p1) or ([Products].[CategoryId] = @p2)", sql);
        }
    }
}
