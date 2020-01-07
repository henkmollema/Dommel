using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Xunit;

namespace Dommel.Tests
{
    public class DefaultKeyPropertyResolverTests
    {
        private static readonly IKeyPropertyResolver Resolver = new DefaultKeyPropertyResolver();

        [Fact]
        public void MapsIdProperty()
        {
            var prop = Resolver.ResolveKeyProperties(typeof(Foo)).Single();
            Assert.Equal(typeof(Foo).GetProperty("Id"), prop.Property);
            Assert.Equal(DatabaseGeneratedOption.Identity, prop.GeneratedOption);
            Assert.True(prop.IsGenerated);
        }

        [Fact]
        public void MapsIdPropertyInheritance()
        {
            var prop = Resolver.ResolveKeyProperties(typeof(FooInheritance)).Single().Property;
            Assert.Equal(typeof(FooInheritance).GetProperty("Id"), prop);
        }


        [Fact]
        public void MapsIdPropertyGenericInheritance()
        {
            var prop = Resolver.ResolveKeyProperties(typeof(FooGenericInheritance)).Single().Property;
            Assert.Equal(typeof(FooGenericInheritance).GetProperty("Id"), prop);
        }



        [Fact]
        public void MapsWithAttribute()
        {
            var prop = Resolver.ResolveKeyProperties(typeof(Bar)).Single().Property;
            Assert.Equal(typeof(Bar).GetProperty("BarId"), prop);
        }

        [Fact]
        public void NoKeyProperties_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Resolver.ResolveKeyProperties(typeof(Nope)).Single().Property);
            Assert.Equal($"Could not find the key properties for type '{typeof(Nope).FullName}'.", ex.Message);
        }

        [Fact]
        public void MapsMultipleKeyProperties()
        {
            var keyProperties = Resolver.ResolveKeyProperties(typeof(FooBar));
            Assert.Equal(2, keyProperties.Length);
            Assert.Equal(typeof(FooBar).GetProperty("Id"), keyProperties[0].Property);
            Assert.Equal(typeof(FooBar).GetProperty("BarId"), keyProperties[1].Property);
        }

        [Fact]
        public void MapsNonGeneratedId()
        {
            var keyProperty = Resolver.ResolveKeyProperties(typeof(WithNonGeneratedIdColumn)).Single();
            Assert.Equal(typeof(WithNonGeneratedIdColumn).GetProperty("Id"), keyProperty.Property);
            Assert.Equal(DatabaseGeneratedOption.None, keyProperty.GeneratedOption);
            Assert.False(keyProperty.IsGenerated);
        }

        [Fact]
        public void MapsNonGeneratedIdWithCustomColumnName()
        {
            var keyProperty = Resolver.ResolveKeyProperties(typeof(WithNonGeneratedCustomIdColumn)).Single();
            Assert.Equal(typeof(WithNonGeneratedCustomIdColumn).GetProperty("MyNonGeneratedKey"), keyProperty.Property);
            Assert.Equal(DatabaseGeneratedOption.None, keyProperty.GeneratedOption);
            Assert.False(keyProperty.IsGenerated);
        }

        private class FooGeneric<T> where T : struct
        {
            public T Id { get; set; }
        }


        private class FooGenericInheritance : FooGeneric<int>
        {
        }

        private class FooInheritance : Foo
        {
        }


        private class Foo
        {
            public object? Id { get; set; }
        }

        private class Bar
        {
            [Key]
            public object? BarId { get; set; }
        }

        private class FooBar
        {
            public object? Id { get; set; }

            [Key]
            public object? BarId { get; set; }
        }

        private class Nope
        {
            public object? Foo { get; set; }
        }

        private class WithNonGeneratedIdColumn
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public Guid Id { get; set; }
        }

        private class WithNonGeneratedCustomIdColumn
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public Guid MyNonGeneratedKey { get; set; }
        }
    }
}
