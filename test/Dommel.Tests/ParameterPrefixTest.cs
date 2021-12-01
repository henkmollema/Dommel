using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class ParameterPrefixTest
    {
        private static readonly ISqlBuilder SqlBuilder = new DummySqlBuilder();

        [Fact]
        public void Get()
        {
            var sql = BuildGetById(SqlBuilder, typeof(Foo), new[] { (object)1 }, out var parameters);
            Assert.Equal("select * from Foos where Foos.Id = #Id", sql);
            Assert.Single(parameters.ParameterNames);
        }

        [Fact]
        public void Select()
        {
            // Arrange
            var builder = new DummySqlBuilder();
            var sqlExpression = new SqlExpression<Foo>(builder);

            // Act
            var sql = sqlExpression.Where(p => p.Id == 1).ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (Foos.Id = #p1)", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
        }

        [Fact]
        public void TestInsert()
        {
            var sql = BuildInsertQuery(SqlBuilder, typeof(Foo));
            Assert.Equal("insert into Foos (Foos.Bar) values (#Bar); select last_insert_rowid() id", sql);
        }

        [Fact]
        public void TestUpdate()
        {
            var sql = BuildUpdateQuery(SqlBuilder, typeof(Foo));
            Assert.Equal("update Foos set Foos.Bar = #Bar where Foos.Id = #Id", sql);
        }

        [Fact]
        public void TestDelete()
        {
            var sql = BuildDeleteQuery(SqlBuilder, typeof(Foo));
            Assert.Equal("delete from Foos where Foos.Id = #Id", sql);
        }

        public class Foo
        {
            public int Id { get; set; }

            public string? Bar { get; set; }
        }
    }
}
