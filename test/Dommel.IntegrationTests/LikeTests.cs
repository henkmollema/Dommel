using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class LikeTests
    {
        private readonly DommelMapper.SqlExpression<Foo> sqlExpression = new DommelMapper.SqlExpression<Foo>();

        [Fact]
        public void LikeOperandContains()
        {
            var expression = sqlExpression.Where(p => p.String.Contains("test"));
            var sql = expressao.ToSql(out var dynamicParameters);

            Assert.Equal("where String like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%teste%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandStartsWith()
        {
            var expressao = sqlExpression.Where(p => p.String.StartsWith("teste"));
            var sql = expressao.ToSql(out var dynamicParameters);

            Assert.Equal("where String like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("teste%", dynamicParameters.Get<string>("p1"));
        }

        [Fact]
        public void LikeOperandEndsWith()
        {
            var expressao = sqlExpression.Where(p => p.String.EndsWith("teste"));
            var sql = expressao.ToSql(out var dynamicParameters);

            Assert.Equal("where String like p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%teste", dynamicParameters.Get<string>("p1"));
        }

        [Table("tblFoo")]
        public class Foo
        {
            public string String { get; set; }
        }
    }
}
