using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Tests
{
    public class LikeTests
    {
        [Fact]
        public void LikeOperandContains()
        {
            // Arrange
            var sqlExpression = new SqlExpression<Foo>(new SqlServerSqlBuilder());

            // Act
            var expression = sqlExpression.Where(p => p.Bar.Contains("test"));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (lower([tblFoo].[Bar]) like lower(@p1))", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandContainsVariable()
        {
            // Arrange
            var sqlExpression = new SqlExpression<Foo>(new SqlServerSqlBuilder());
            var substring = "test";

            // Act
            var expression = sqlExpression.Where(p => p.Bar.Contains(substring));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (lower([tblFoo].[Bar]) like lower(@p1))", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandStartsWith()
        {
            // Arrange
            var sqlExpression = new SqlExpression<Foo>(new SqlServerSqlBuilder());

            // Act
            var expression = sqlExpression.Where(p => p.Bar.StartsWith("test"));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (lower([tblFoo].[Bar]) like lower(@p1))", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("test%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandEndsWith()
        {
            // Arrange
            var sqlExpression = new SqlExpression<Foo>(new SqlServerSqlBuilder());

            // Act
            var expression = sqlExpression.Where(p => p.Bar.EndsWith("test"));
            var sql = expression.ToSql(out var dynamicParameters);

            // Assert
            Assert.Equal("where (lower([tblFoo].[Bar]) like lower(@p1))", sql.Trim());
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
