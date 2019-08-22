using Xunit;

namespace Dommel.Json.Tests
{
    public class JsonSqlExpressionTests
    {
        [Fact]
        public void GeneratesMySqlJsonValue()
        {
            // Arrange
            var sql = new JsonSqlExpression<Lead>(new MySqlSqlBuilder());

            // Act
            var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

            // Assert
            Assert.Equal(" where (`Data`->'$.LastName' = @p1)", str);
            Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
        }

        [Fact]
        public void GeneratesSqlServerJsonValue()
        {
            // Arrange
            var sql = new JsonSqlExpression<Lead>(new SqlServerSqlBuilder());

            // Act
            var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

            // Assert
            Assert.Equal(" where (JSON_VALUE([Data], '$.LastName') = @p1)", str);
            Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
        }

        [Fact]
        public void GeneratesSqliteJsonValue()
        {
            // Arrange
            var sql = new JsonSqlExpression<Lead>(new SqliteSqlBuilder());

            // Act
            var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

            // Assert
            Assert.Equal(" where (JSON_EXTRACT(Data, '$.LastName') = @p1)", str);
            Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
        }

        [Fact]
        public void GeneratesSqlServerCeJsonValue()
        {
            // Arrange
            var sql = new JsonSqlExpression<Lead>(new SqlServerCeSqlBuilder());

            // Act
            var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

            // Assert
            Assert.Equal(" where (JSON_VALUE([Data], '$.LastName') = @p1)", str);
            Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
        }

        [Fact]
        public void GeneratesPostgresJsonValue()
        {
            // Arrange
            var sql = new JsonSqlExpression<Lead>(new PostgresSqlBuiler());

            // Act
            var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

            // Assert
            Assert.Equal(" where (\"Data\"->>'LastName' = @p1)", str);
            Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
        }
    }
}
