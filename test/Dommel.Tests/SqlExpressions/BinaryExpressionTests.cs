using Xunit;

namespace Dommel.Tests
{
    public class BinaryExpressionTests
    {
        [Fact]
        public void TwoConstants()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => "test" == "3")
                .ToSql();
            Assert.Equal(" where (False)", sql);
        }
        
        [Fact]
        public void GreaterThan()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 > f.Value2)
                .ToSql();
            Assert.Equal(" where ([Value1] > [Value2])", sql);
        }
        
        [Fact]
        public void GreaterThanWithLeftConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => 500 > f.Value2)
                .ToSql();
            Assert.Equal(" where (@p1 > [Value2])", sql);
        }
        
        [Fact]
        public void GreaterThanWithRightConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 > 157)
                .ToSql();
            Assert.Equal(" where ([Value1] > @p1)", sql);
        }
        
        [Fact]
        public void GreaterThanOrEqual()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 >= f.Value2)
                .ToSql();
            Assert.Equal(" where ([Value1] >= [Value2])", sql);
        }
        
        [Fact]
        public void GreaterThanOrEqualWithLeftConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => 200 >= f.Value2)
                .ToSql();
            Assert.Equal(" where (@p1 >= [Value2])", sql);
        }
        
        [Fact]
        public void GreaterThanOrEqualWithRightConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 >= 20)
                .ToSql();
            Assert.Equal(" where ([Value1] >= @p1)", sql);
        }

        [Fact]
        public void LessThan()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 < f.Value2)
                .ToSql();
            Assert.Equal(" where ([Value1] < [Value2])", sql);
        }
        
        [Fact]
        public void LessThanWithLeftConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => 20 < f.Value2)
                .ToSql();
            Assert.Equal(" where (@p1 < [Value2])", sql);
        }
        
        [Fact]
        public void LessThanWithRightConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 < 10)
                .ToSql();
            Assert.Equal(" where ([Value1] < @p1)", sql);
        }
        
        [Fact]
        public void LessThanOrEqual()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 <= f.Value2)
                .ToSql();
            Assert.Equal(" where ([Value1] <= [Value2])", sql);
        }
        
        [Fact]
        public void LessThanOrEqualWithLeftConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => 15 <= f.Value2)
                .ToSql();
            Assert.Equal(" where (@p1 <= [Value2])", sql);
        }
        
        [Fact]
        public void LessThanOrEqualWithRightConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 <= 12)
                .ToSql();
            Assert.Equal(" where ([Value1] <= @p1)", sql);
        }
        
        [Fact]
        public void Add()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 + f.Value2 > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] + [Value2] > [Value3])", sql);
        }
        
        [Fact]
        public void AddWithConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 + 10 > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] + @p1 > [Value3])", sql);
        }
        
        [Fact]
        public void Subtract()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (f.Value1 - f.Value2) > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] - [Value2] > [Value3])", sql);
        }
        
        [Fact]
        public void SubtractWithConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (f.Value1 - 20) > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] - @p1 > [Value3])", sql);
        }
        
        [Fact]
        public void Multiply()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (f.Value1 * f.Value2) > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] * [Value2] > [Value3])", sql);
        }
        
        [Fact]
        public void MultiplyWithConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (30 * f.Value2) > f.Value3)
                .ToSql();
            Assert.Equal(" where (@p1 * [Value2] > [Value3])", sql);
        }
        
        [Fact]
        public void Modulo()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (f.Value1 % f.Value2)  > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] MOD [Value2] > [Value3])", sql);
        }
        
        [Fact]
        public void ModuloWithConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (f.Value1 % 16)  > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] MOD @p1 > [Value3])", sql);
        }
        
        
        [Fact]
        public void Divide()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (f.Value1 / f.Value2)  > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] / [Value2] > [Value3])", sql);
        }
        
        [Fact]
        public void DivideWithConstant()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (f.Value1 / 16)  > f.Value3)
                .ToSql();
            Assert.Equal(" where ([Value1] / @p1 > [Value3])", sql);
        }
        
        [Fact]
        public void AndAlso()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Check1 && f.Check2)
                .ToSql();
            Assert.Equal(" where ([Check1] = '1' and [Check2] = '1')", sql);
        }
        
        [Fact]
        public void OrElse()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Check1 || f.Check2)
                .ToSql();
            Assert.Equal(" where ([Check1] = '1' or [Check2] = '1')", sql);
        }
        
        [Fact]
        public void MultipleArithmeticAndLogic()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => f.Value1 * f.Value2 / (f.Value3 + f.Value2) > f.Value1)
                .ToSql();
            Assert.Equal(" where ([Value1] * [Value2] / ([Value3] + [Value2]) > [Value1])", sql);
        }
        
        
        [Fact]
        public void MultipleArithmeticAndLogicWithParentheses()
        {
            var sql = new SqlExpression<Foo>(new SqlServerSqlBuilder())
                .Where(f => (f.Value1 + f.Value2) * f.Value3 > 0)
                .ToSql();
            Assert.Equal(" where (([Value1] + [Value2]) * [Value3] > @p1)", sql);
        }

        private class Foo
        {
            public int Value1 { get; set; }
            public int Value2 { get; set; }
            public int Value3 { get; set; }
            
            public bool Check1 { get; set; }
            public bool Check2 { get; set; }
        }
    }
}