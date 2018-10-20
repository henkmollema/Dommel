using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using Xunit;

namespace Dommel.Tests
{
    public class LikeTests
    {
        private readonly DommelMapper.SqlExpression<Foo> sqlExpression = new DommelMapper.SqlExpression<Foo>();

        public LikeTests()
        {
            var mock = new Mock<IDbConnection>();
            DommelMapper.GetSqlBuilder(mock.Object);
        }

        [Fact]
        public void LikeOperandContains()
        {
            var expression = sqlExpression.Where(p => p.Bar.Contains("test"));
            var sql = expression.ToSql(out var dynamicParameters);

            Assert.Equal("where Bar like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandStartsWith()
        {
            var expressao = sqlExpression.Where(p => p.Bar.StartsWith("teste"));
            var sql = expressao.ToSql(out var dynamicParameters);

            Assert.Equal("where Bar like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("teste%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandEndsWith()
        {
            var expressao = sqlExpression.Where(p => p.Bar.EndsWith("teste"));
            var sql = expressao.ToSql(out var dynamicParameters);

            Assert.Equal("where Bar like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%teste", dynamicParameters.Get<string>("p1"));
        }

        [Table("tblFoo")]
        public class Foo
        {
            public string Bar { get; set; }
        }
    }
}
