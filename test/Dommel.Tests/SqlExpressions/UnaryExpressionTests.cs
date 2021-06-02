using Xunit;

namespace Dommel.Tests
{
    public class UnaryExpressionTests
    {
        [Fact]
        public void Negate()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => -f.Value1 > 0 )
                .ToSql();
            Assert.Equal(" where (-[Value1] > @p1)", sql);
        }
        
        [Fact]
        public void Not()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => !f.Check1 )
                .ToSql();
            Assert.Equal(" where (not ([Check1] = '1'))", sql);
        }

        private class Foo
        {
            public int Value1 { get; set; }
            public bool Check1 { get; set; }
        }
    }
}