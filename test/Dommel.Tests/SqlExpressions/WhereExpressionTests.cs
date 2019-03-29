using System;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class WhereExpressionTests
    {
        private readonly SqlExpression<Product> _sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());

        [Fact]
        public void Where_ThrowsForNullExpression()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>("expression", () => _sqlExpression.Where(null));
        }

        [Fact]
        public void Where_ThrowsWhenCalledTwice()
        {
            _sqlExpression.Where(_ => true);
            var ex = Assert.Throws<InvalidOperationException>(() => _sqlExpression.Where(_ => true));
            Assert.Equal("Where statement already started. Use 'AndWhere' or 'OrWhere' to add additional statements.", ex.Message);
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
    }
}
