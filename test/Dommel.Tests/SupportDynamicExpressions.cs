using Dapper;
using Moq;
using Moq.Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class SupportDynamicExpressions
    {
        private readonly DommelMapper.SqlExpression<Foo> sqlExpression = new DommelMapper.SqlExpression<Foo>(new SqlServerSqlBuilder());
        private readonly Mock<IDbConnection> mock = new Mock<IDbConnection>();

        public SupportDynamicExpressions()
        {
            mock.SetupDapper(x => x.QueryFirstOrDefault<Foo>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .Returns(new Foo());
        }

        [Fact]
        public void CommonAndExpression()
        {
            var dommelExpression = sqlExpression.Where(p => p.Id == 1 && p.Bar.Contains("test"));
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where [Id] = @p1 and [Bar] like @p2", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal("%test%", dynamicParameters.Get<string>("p2"));
        }

        [Fact]
        public void AndExpression()
        {
            Expression<Func<Foo, bool>> expression = p => p.Id == 1;
            expression = And(expression, p => p.Bar.Contains("test"));

            var dommelExpression = sqlExpression.Where(expression);
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where [Id] = @p1 and [Bar] like @p2", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal("%test%", dynamicParameters.Get<string>("p2"));
        }

        [Fact]
        public void CommonOrExpression()
        {
            var dommelExpression = sqlExpression.Where(p => p.Id == 1 || p.Bar.Contains("testOr"));
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where [Id] = @p1 or [Bar] like @p2", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal("%testOr%", dynamicParameters.Get<string>("p2"));
        }

        [Fact]
        public void OrExpression()
        {
            Expression<Func<Foo, bool>> expression = p => p.Id == 1;
            expression = Or(expression, p => p.Id == 2);

            var dommelExpression = sqlExpression.Where(expression);
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where [Id] = @p1 or [Id] = @p2", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal(2, dynamicParameters.Get<int>("p2"));
        }

        [Fact]
        public void InExpression()
        {
            var ids = new[] {1, 2};
            var idList = new ArrayList {"1", "2"};
            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var guidList = new List<Guid>() {guid1, guid2};
            Expression<Func<Foo, bool>> expression = p =>
                ids.Contains(p.Id) || idList.Contains(p.StringId) || guidList.Contains(p.Guid) ||
                p.Bar.Contains("testIn");

            var dommelExpression = sqlExpression.Where(expression);
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where [Id] in (@p1,@p2) or [StringId] in (@p3,@p4) or [Guid] in (@p5,@p6) or [Bar] like @p7", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal(2, dynamicParameters.Get<int>("p2"));
            Assert.Equal("1", dynamicParameters.Get<string>("p3"));
            Assert.Equal("2", dynamicParameters.Get<string>("p4"));
            Assert.Equal(guid1, dynamicParameters.Get<Guid>("p5"));
            Assert.Equal(guid2, dynamicParameters.Get<Guid>("p6"));
            Assert.Equal("%testIn%", dynamicParameters.Get<string>("p7"));

        }

        [Table("tblFoo")]
        public class Foo
        {
            public int Id { get; set; }

            public string StringId { get; set; }

            public Guid Guid { get; set; }

            public string Bar { get; set; }
        }

        public Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                return right;
            }

            var invokeExpression = Expression.Invoke(right, left.Parameters.Cast<Expression>());
            return (Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left.Body, invokeExpression), left.Parameters));
        }

        public Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                return right;
            }

            var invokeExpression = Expression.Invoke(right, left.Parameters.Cast<Expression>());
            return (Expression.Lambda<Func<T, bool>>(Expression.OrElse(left.Body, invokeExpression), left.Parameters));
        }
    }
}
