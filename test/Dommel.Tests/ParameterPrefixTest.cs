using System.Data;
using System.Reflection;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class ParameterPrefixTest
    {
        private static readonly IDbConnection DbConnection = new FooDbConnection();

        public ParameterPrefixTest()
        {
            AddSqlBuilder(typeof(FooDbConnection), new DummySqlBuilder());
        }

        [Fact]
        public void Get()
        {
            var sql = BuildGetById(DbConnection, typeof(Foo), new[] { (object)1 }, out var parameters);
            Assert.Equal("select * from Foos where Id = #Id", sql);
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
            Assert.Equal("where (Id = #p1)", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
        }

        [Fact]
        public void TestUpdate()
        {
            var sql = BuildUpdateQuery(DbConnection, typeof(Foo));
            Assert.Equal("update Foos set Bar = #Bar where Id = #Id", sql);
        }

        [Fact]
        public void TestDelete()
        {
            var sql = BuildDeleteQuery(DbConnection, typeof(Foo));
            Assert.Equal("delete from Foos where Id = #Id", sql);
        }

        public interface IDataBaseParameterPrefix : IDbConnection { }

        public sealed class DummySqlBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public string PrefixParameter(string paramName) => $"#{paramName}";

            /// <inheritdoc/>
            public string QuoteIdentifier(string identifier) => identifier;

            /// <inheritdoc/>
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty) => $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select last_insert_rowid() id";

            /// <inheritdoc/>
            public string BuildPaging(string orderBy, int pageNumber, int pageSize)
            {
                var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
                return $" {orderBy} LIMIT {start}, {pageSize}";
            }
        }

        public class Foo
        {
            public int Id { get; set; }

            public string Bar { get; set; }
        }
    }
}
