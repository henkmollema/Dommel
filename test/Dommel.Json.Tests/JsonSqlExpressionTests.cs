using Xunit;

namespace Dommel.Json.Tests
{
    public class JsonSqlExpressionTests
    {
        [Fact]
        public void GeneratesMySqlJsonValue()
        {
            // Arrange
            var sql = new JsonSqlExpression<Lead>(new MySqlSqlBuilder(), new DommelJsonOptions());

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
            var sql = new JsonSqlExpression<Lead>(new SqlServerSqlBuilder(), new DommelJsonOptions());

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
            var sql = new JsonSqlExpression<Lead>(new SqliteSqlBuilder(), new DommelJsonOptions());

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
            var sql = new JsonSqlExpression<Lead>(new SqlServerCeSqlBuilder(), new DommelJsonOptions());

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
            var sql = new JsonSqlExpression<Lead>(new PostgresSqlBuilder(), new DommelJsonOptions());

            // Act
            var str = sql.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

            // Assert
            Assert.Equal(" where (\"Data\"->>'LastName' = @p1)", str);
            Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
        }
    }
}
