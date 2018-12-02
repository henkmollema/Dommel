﻿using System.ComponentModel.DataAnnotations.Schema;
using Xunit;
using static Dommel.DommelMapper;

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
            Assert.Equal("where [Bar] like @p1", sql.Trim());
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
            Assert.Equal("where [Bar] like @p1", sql.Trim());
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
            Assert.Equal("where [Bar] like @p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test", dynamicParameters.Get<string>("p1"));
        }

        [Table("tblFoo")]
        public class Foo
        {
            public int Id { get; set; }

            public string Bar { get; set; }
        }
    }
}
