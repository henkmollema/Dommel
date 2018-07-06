using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Dommel.Tests
{
    public class DefaultKeyPropertyResolverTests
    {
        private static DommelMapper.IKeyPropertyResolver Resolver = new DommelMapper.DefaultKeyPropertyResolver();

        [Fact]
        public void MapsIdProperty()
        {
            var prop = Resolver.ResolveKeyProperties(typeof(Foo))[0];
            Assert.Equal(typeof(Foo).GetProperty("Id"), prop);
        }

        [Fact]
        public void MapsWithAttribute()
        {
            var prop = Resolver.ResolveKeyProperties(typeof(Bar))[0];
            Assert.Equal(typeof(Bar).GetProperty("BarId"), prop);
        }

        [Fact]
        public void NoKeyProperties_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Resolver.ResolveKeyProperties(typeof(Nope))[0]);
            Assert.Equal($"Could not find the key properties for type '{typeof(Nope).FullName}'.", ex.Message);
        }

        [Fact]
        public void MapsMultipleKeyProperties()
        {
            var keyProperties = Resolver.ResolveKeyProperties(typeof(FooBar));
            Assert.Equal(typeof(FooBar).GetProperty("Id"), keyProperties[0]);
            Assert.Equal(typeof(FooBar).GetProperty("BarId"), keyProperties[1]);
        }

        private class Foo
        {
            public object Id { get; set; }
        }

        private class Bar
        {
            [Key]
            public object BarId { get; set; }
        }

        private class FooBar
        {
            public object Id { get; set; }

            [Key]
            public object BarId { get; set; }
        }

        private class Nope
        {
            public object Foo { get; set; }
        }
    }
}
