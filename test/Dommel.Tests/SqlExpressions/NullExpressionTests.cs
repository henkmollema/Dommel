using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class NullExpressionTests
    {
        [Fact]
        public void GeneratesCorrectIsEqualSyntax()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Bar == null)
                .ToSql();
            Assert.Equal(" where ([Bar] is null)", sql);
        }

        [Fact]
        public void GeneratesCorrectIsNotEqualSyntax()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Bar != null)
                .ToSql();
            Assert.Equal(" where ([Bar] is not null)", sql);
        }

        [Fact]
        public void GeneratesInvalidIsEqualSyntaxForInvalidExpression()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => null == f.Bar)
                .ToSql();
            Assert.Equal(" where ( = @p1)", sql);
        }

        [Fact]
        public void GeneratesInvalidIsNotEqualSyntaxForInvalidExpression()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => null != f.Bar)
                .ToSql();
            Assert.Equal(" where ( <> @p1)", sql);
        }

        public class Foo
        {
            public string? Bar { get; set; }
        }
    }
}
