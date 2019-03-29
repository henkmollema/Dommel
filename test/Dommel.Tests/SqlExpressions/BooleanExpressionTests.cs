using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class BooleanExpressionTests
    {
        [Fact]
        public void Single()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Baz)
                .ToSql();
            Assert.Equal(" where [Baz] = '1'", sql);
        }

        [Fact]
        public void SingleNot()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => !f.Baz)
                .ToSql();
            Assert.Equal(" where not ([Baz] = '1')", sql);
        }

        [Fact]
        public void SingleExplicitTrue()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Baz == true)
                .ToSql();
            Assert.Equal(" where [Baz] = @p1", sql);
        }

        [Fact]
        public void Or()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Baz || f.Qux)
                .ToSql();
            Assert.Equal(" where [Baz] = '1' or [Qux] = '1'", sql);
        }

        [Fact]
        public void OrWithNot()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Baz || !f.Qux)
                .ToSql();
            Assert.Equal(" where [Baz] = '1' or not ([Qux] = '1')", sql);
        }

        [Fact]
        public void And()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Baz && f.Qux)
                .ToSql();
            Assert.Equal(" where [Baz] = '1' and [Qux] = '1'", sql);
        }

        [Fact]
        public void AndWithNot()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => !f.Baz && f.Qux)
                .ToSql();
            Assert.Equal(" where not ([Baz] = '1') and [Qux] = '1'", sql);
        }

        [Fact]
        public void Combined()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Bar == "test" || f.Baz)
                .ToSql();
            Assert.Equal(" where [Bar] = @p1 or [Baz] = '1'", sql);
        }

        public class Foo
        {
            public string Bar { get; set; }

            public bool Baz { get; set; }

            public bool Qux { get; set; }
        }
    }
}
