using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Json.Tests
{
    public class LikeTests
    {
        [Fact]
        public void LikeOperandContains_WithConstant()
        {
            // Arrange
            var sqlExpression = new JsonSqlExpression<Lead>(new MySqlSqlBuilder());

            // Act
            var expression = sqlExpression.Where(p => p.Data.FirstName!.Contains("test"));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (`Data`->'$.FirstName' like @p1)", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperand_WithVariable()
        {
            // Arrange
            var sqlExpression = new JsonSqlExpression<Lead>(new MySqlSqlBuilder());
            var substring = "test";

            // Act
            var expression = sqlExpression.Where(p => p.Data.FirstName!.Contains(substring));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (`Data`->'$.FirstName' like @p1)", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandStartsWith_WithConstant()
        {
            // Arrange
            var sqlExpression = new JsonSqlExpression<Lead>(new MySqlSqlBuilder());

            // Act
            var expression = sqlExpression.Where(p => p.Data.FirstName!.StartsWith("test"));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (`Data`->'$.FirstName' like @p1)", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("test%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandStartsWith_WithVariable()
        {
            // Arrange
            var sqlExpression = new JsonSqlExpression<Lead>(new MySqlSqlBuilder());
            var substring = "test";

            // Act
            var expression = sqlExpression.Where(p => p.Data.FirstName!.StartsWith(substring));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (`Data`->'$.FirstName' like @p1)", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("test%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandEndsWith_WithConstant()
        {
            // Arrange
            var sqlExpression = new JsonSqlExpression<Lead>(new MySqlSqlBuilder());

            // Act
            var expression = sqlExpression.Where(p => p.Data.FirstName!.EndsWith("test"));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (`Data`->'$.FirstName' like @p1)", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandEndsWith_WithVariable()
        {
            // Arrange
            var sqlExpression = new JsonSqlExpression<Lead>(new MySqlSqlBuilder());
            var substring = "test";

            // Act
            var expression = sqlExpression.Where(p => p.Data.FirstName!.EndsWith(substring));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (`Data`->'$.FirstName' like @p1)", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test", dynamicParameters.Get<string>("p1"));
        }

        [Table("tblFoo")]
        public class Foo
        {
            public int Id { get; set; }

            public string Bar { get; set; } = "";
        }
    }
}
