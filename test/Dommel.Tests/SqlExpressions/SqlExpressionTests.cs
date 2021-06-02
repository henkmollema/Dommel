using Dapper;
using Xunit;

namespace Dommel.Tests
{
    public class SqlExpressionTests
    {
        private readonly SqlExpression<Product> _sqlExpression = new SqlExpression<Product>(new SqlServerSqlBuilder());

        [Fact]
        public void ToString_ReturnsSql()
        {
            var sql = _sqlExpression.Where(p => p.Name == "Chai").ToSql();
            Assert.Equal(" where ([Name] = @p1)", sql);
            Assert.Equal(sql, _sqlExpression.ToString());
        }

        [Fact]
        public void ParseNewExpression()
        {
            var sql = _sqlExpression.Where(p => p.Name == new string("Chai")).ToSql(out DynamicParameters parameters);
            Assert.Equal(" where ([Name] = @p1)", sql);
            Assert.Equal(sql, _sqlExpression.ToString());
            Assert.Equal("Chai", parameters.Get<string>("p1"));
        }
    }
}
