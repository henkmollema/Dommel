using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Dommel.Tests
{
    public class OrderByTests
    {
        
        [Fact]
        public void Asc()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .OrderBy(f => f.Value1)
                .ToSql();
            Assert.Equal(" order by [Value1] asc", sql);
        }
        
        [Fact]
        public void AscProp()
        {
            var value1Prop = typeof(Foo).GetProperties().FirstOrDefault(p => p.Name == nameof(Foo.Value1));
            
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .OrderBy(value1Prop)
                .ToSql();
            Assert.Equal(" order by [Value1] asc", sql);
        }
        
        [Fact]
        public void AscMultiple()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .OrderBy(f => f.Value1)
                .OrderBy(f => f.Value2)
                .ToSql();
            Assert.Equal(" order by [Value1] asc, [Value2] asc", sql);
        }
        
        [Fact]
        public void Desc()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .OrderByDescending(f => f.Value1)
                .ToSql();
            Assert.Equal(" order by [Value1] desc", sql);
        }
        
        [Fact]
        public void DescProp()
        {
            var value1Prop = typeof(Foo).GetProperties().FirstOrDefault(p => p.Name == nameof(Foo.Value1));
            
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .OrderByDescending(value1Prop)
                .ToSql();
            Assert.Equal(" order by [Value1] desc", sql);
        }
        
        [Fact]
        public void DescMultiple()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .OrderByDescending(f => f.Value1)
                .OrderByDescending(f => f.Value2)
                .ToSql();
            Assert.Equal(" order by [Value1] desc, [Value2] desc", sql);
        }
        
        [Fact]
        public void DescAsc()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .OrderByDescending(f => f.Value1)
                .OrderBy(f => f.Value2)
                .ToSql();
            Assert.Equal(" order by [Value1] desc, [Value2] asc", sql);
        }
        [Fact]
        public void AscDesc()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .OrderBy(f => f.Value1)
                .OrderByDescending(f => f.Value2)
                .ToSql();
            Assert.Equal(" order by [Value1] asc, [Value2] desc", sql);
        }
        
        [Fact]
        public void NullAscThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SqlExpression<Foo>(new SqlServerSqlBuilder())
                    .OrderBy((Expression<Func<Foo, object?>>) null)
            );
        }
        
        [Fact]
        public void NullPropAscThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SqlExpression<Foo>(new SqlServerSqlBuilder())
                    .OrderBy((PropertyInfo) null)
            );
        }
        
        [Fact]
        public void NullDescThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SqlExpression<Foo>(new SqlServerSqlBuilder())
                    .OrderByDescending((Expression<Func<Foo, object?>>) null)
            );
        }
        
        [Fact]
        public void NullPropDescThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => new SqlExpression<Foo>(new SqlServerSqlBuilder())
                    .OrderByDescending((PropertyInfo) null)
            );
        }
        
        public class Foo
        {
            public int Value1 { get; set; }
            public int Value2 { get; set; }
            public int Value3 { get; set; }
            
            public bool Check1 { get; set; }
            public bool Check2 { get; set; }
        }
    }
}