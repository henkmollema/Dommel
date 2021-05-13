using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Dommel.Tests
{
    public class DynamicExpressionTests
    {
        private readonly SqlExpression<Foo> _sqlExpression = new SqlExpression<Foo>(new SqlServerSqlBuilder());

        [Fact]
        public void CommonAndExpression()
        {
            var dommelExpression = _sqlExpression.Where(p => p.Id == 1 && p.Bar.Contains("test"));
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where ([Id] = @p1 and lower([Bar]) like lower(@p2))", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal("%test%", dynamicParameters.Get<string>("p2"));
        }

        [Fact]
        public void AndExpression()
        {
            Expression<Func<Foo, bool>> expression = p => p.Id == 1;
            expression = And(expression, p => p.Bar.Contains("test"));

            var dommelExpression = _sqlExpression.Where(expression);
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where ([Id] = @p1 and lower([Bar]) like lower(@p2))", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal("%test%", dynamicParameters.Get<string>("p2"));
        }

        [Fact]
        public void CommonOrExpression()
        {
            var dommelExpression = _sqlExpression.Where(p => p.Id == 1 || p.Bar.Contains("testOr"));
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where ([Id] = @p1 or lower([Bar]) like lower(@p2))", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal("%testOr%", dynamicParameters.Get<string>("p2"));
        }

        [Fact]
        public void OrExpression()
        {
            Expression<Func<Foo, bool>> expression = p => p.Id == 1;
            expression = Or(expression, p => p.Id == 2);

            var dommelExpression = _sqlExpression.Where(expression);
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal("where ([Id] = @p1 or [Id] = @p2)", sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal(2, dynamicParameters.Get<int>("p2"));
        }

        [Fact]
        public void InExpression()
        {
            var ids = new[] {1, 2};
            var idList = new ArrayList {"1", "2"};
            var guid1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var guid2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var guidList = new List<Guid> {guid1, guid2};
            var decimalList = new List<decimal> {1.0m, 2.0m};
            Expression<Func<Foo, bool>> expression = p =>
                ids.Contains(p.Id) || idList.Contains(p.StringId) ||
                decimalList.Contains(p.DecimalId) || guidList.Contains(p.Guid) ||
                p.Bar.Contains("testIn");

            var dommelExpression = _sqlExpression.Where(expression);
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal(
                "where ([Id] in (@p1,@p2) or [StringId] in (@p3,@p4) or [DecimalId] in (@p5,@p6) or [Guid] in (@p7,@p8) or lower([Bar]) like lower(@p9))",
                sql.Trim());
            Assert.Equal(1, dynamicParameters.Get<int>("p1"));
            Assert.Equal(2, dynamicParameters.Get<int>("p2"));
            Assert.Equal("1", dynamicParameters.Get<string>("p3"));
            Assert.Equal("2", dynamicParameters.Get<string>("p4"));
            Assert.Equal(1.0m, dynamicParameters.Get<decimal>("p5"));
            Assert.Equal(2.0m, dynamicParameters.Get<decimal>("p6"));
            Assert.Equal(guid1, dynamicParameters.Get<Guid>("p7"));
            Assert.Equal(guid2, dynamicParameters.Get<Guid>("p8"));
            Assert.Equal("%testIn%", dynamicParameters.Get<string>("p9"));
        }
        
        [Fact]
        public void InWithEmptyArray()
        {
            var ids = new int[] {};
            Expression<Func<Foo, bool>> expression = p =>
                ids.Contains(p.Id);

            var dommelExpression = _sqlExpression.Where(expression);
            var sql = dommelExpression.ToSql(out var dynamicParameters);

            Assert.Equal(
                "where ([Id] in (null))",
                sql.Trim());
           
        }

        [Table("tblFoo")]
        public class Foo
        {
            public int Id { get; set; }

            public string? StringId { get; set; }

            public decimal DecimalId { get; set; }

            public Guid Guid { get; set; }

            public string Bar { get; set; } = "";
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