using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Dommel.Tests
{
    public class DefaultKeyPropertyResolverTests
    {
        private static DommelMapper.DefaultKeyPropertyResolver Resolver = new DommelMapper.DefaultKeyPropertyResolver();

        [Fact]
        public void MapsIdProperty()
        {
            var prop = Resolver.ResolveKeyProperty(typeof(Foo));
            Assert.Equal(typeof(Foo).GetProperty("Id"), prop);
        }

        [Fact]
        public void MapsWithAttribute()
        {
            var prop = Resolver.ResolveKeyProperty(typeof(Bar));
            Assert.Equal(typeof(Bar).GetProperty("BarId"), prop);
        }

        [Fact]
        public void NoKeyProperties_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Resolver.ResolveKeyProperty(typeof(Nope)));
            Assert.Equal($"Could not find the key property for type '{typeof(Nope).FullName}'.", ex.Message);
        }

        [Fact]
        public void MultipleKeyProperties_ThrowsException()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => Resolver.ResolveKeyProperty(typeof(FooBar)));
            Assert.Equal($"Multiple key properties were found for type '{typeof(FooBar).FullName}'.", ex.Message);
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
