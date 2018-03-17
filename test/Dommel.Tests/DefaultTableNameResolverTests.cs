using System;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Tests
{
    public class DefaultTableNameResolverTests
    {
        private static readonly DommelMapper.DefaultTableNameResolver Resolver = new DommelMapper.DefaultTableNameResolver();

        [Theory]
        [InlineData(typeof(Product), "Products")]
        [InlineData(typeof(Products), "Products")]
        [InlineData(typeof(Category), "Categories")]
        public void PluralizesName(Type type, string tableName)
        {
            var name = Resolver.ResolveTableName(type);
            Assert.Equal(tableName, name);
        }

        [Fact]
        public void MapsTableAttribute()
        {
            var name = Resolver.ResolveTableName(typeof(Foo));
            Assert.Equal("tblFoo", name);
        }

        [Fact]
        public void MapsTableAttributeWithSchema()
        {
            var name = Resolver.ResolveTableName(typeof(FooWithSchema));
            Assert.Equal("dbo.tblFoo", name);
        }

        private class Product { }

        private class Products { }

        private class Category { }

        [Table("tblFoo")]
        private class Foo { }

        [Table("tblFoo", Schema = "dbo")]
        private class FooWithSchema { }
    }
}
