using Xunit;

namespace Dommel.Tests
{
    public class NullExpressionTests
    {
        [Fact]
        public void IsNullLeft()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Bar == null)
                .ToSql();
            Assert.Equal(" where ([Bar] is null)", sql);
        }

        [Fact]
        public void IsNotNullLeft()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Bar != null)
                .ToSql();
            Assert.Equal(" where ([Bar] is not null)", sql);
        }

        [Fact]
        public void IsNullRight()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => null == f.Bar)
                .ToSql();
            Assert.Equal(" where ([Bar] is null)", sql);
        }

        [Fact]
        public void IsNotNullRight()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => null != f.Bar)
                .ToSql();
            Assert.Equal(" where ([Bar] is not null)", sql);
        }

        public class Foo
        {
            public string? Bar { get; set; }
        }
    }
}
