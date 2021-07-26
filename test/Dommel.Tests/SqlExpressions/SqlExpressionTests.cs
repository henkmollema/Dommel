using Xunit;

namespace Dommel.Tests
{
    public class SqlExpressionTests
    {
        [Fact]
        public void ToString_ReturnsSql()
        {
            var _sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
            var sql = _sqlExpression.Where(p => p.Name == "Chai").ToSql();
            Assert.Equal(" where ([Name] = @p1)", sql);
            Assert.Equal(sql, _sqlExpression.ToString());
        }

        [Fact]
        public void ToStringVisitation_ReturnsSql()
        {
            var _sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());
            var sql = _sqlExpression.Where(p => p.CategoryId.ToString() == "1").ToSql();
            Assert.Equal(" where (CAST([CategoryId] AS CHAR) = @p1)", sql);
            Assert.Equal(sql, _sqlExpression.ToString());
        }
    }
}
