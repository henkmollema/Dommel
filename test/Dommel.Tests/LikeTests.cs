using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Dommel.Tests
{
    public class LikeTests
    {
        private readonly DommelMapper.SqlExpression<User> sqlExpression = new DommelMapper.SqlExpression<User>();

        [Fact]
        public void LikeOperandContains()
        {
            var expressao = sqlExpression.Where(p => p.Name.Contains("teste"));
            var sql = expressao.ToSql(out var dynamicParameters);

            Assert.Equal("where Name like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%teste%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandStartsWith()
        {
            var expressao = sqlExpression.Where(p => p.Name.StartsWith("teste"));
            var sql = expressao.ToSql(out var dynamicParameters);

            Assert.Equal("where Name like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("teste%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandEndsWith()
        {
            var expressao = sqlExpression.Where(p => p.Name.EndsWith("teste"));
            var sql = expressao.ToSql(out var dynamicParameters);

            Assert.Equal("where Name like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%teste", dynamicParameters.Get<string>("p1"));
        }

        public class User
        {
            public int UserId { get; set; }

            public string Name { get; set; }
        }
    }
}
