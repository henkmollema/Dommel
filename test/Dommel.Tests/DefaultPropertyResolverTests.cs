using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Dommel.Tests
{
    public class DefaultPropertyResolverTests
    {
        [Fact]
        public void ResolvesSimpleProperties()
        {
            // Arrange
            var resolver = new DommelMapper.DefaultPropertyResolver();
            var type = typeof(Foo);

            // Act
            var props = resolver.ResolveProperties(type).ToArray();

            // Assert
            Assert.Equal(type.GetProperties().Skip(1).ToArray(), props);
        }

        [Fact]
        public void Resolves_WithCustom()
        {
            // Arrange
            var resolver = new CustomResolver();
            var type = typeof(Foo);

            // Act
            var props = resolver.ResolveProperties(type).ToArray();

            // Assert
            Assert.Equal(type.GetProperties().Skip(2).ToArray(), props);
        }

        [Fact]
        public void IgnoresIgnoreAttribute()
        {
            // Arrange
            var resolver = new DommelMapper.DefaultPropertyResolver();
            var type = typeof(Bar);

            // Act
            var props = resolver.ResolveProperties(type).ToArray();

            // Assert
            var prop = Assert.Single(props);
            Assert.Equal(type.GetProperty("Id"), prop);
        }

        private class CustomResolver : DommelMapper.DefaultPropertyResolver
        {
            // Create a new hashset without the object type.
            protected override HashSet<Type> PrimitiveTypes => new HashSet<Type>(base.PrimitiveTypes.Skip(1));
        }

        private class Foo
        {
            public class Bar { }

            public Bar Baz { get; set; }

            public object Object { get; set; }

            public string String { get; set; }

            public Guid? Guid { get; set; }

            public decimal? Decimal { get; set; }

            public double? Double { get; set; }

            public float? Float { get; set; }

            public DateTime DateTime { get; set; }

            public DateTimeOffset? DateTimeOffset { get; set; }

            public TimeSpan? Timespan { get; set; }

            public byte[] Bytes { get; set; }
        }

        private class Bar
        {
            public int Id { get; set; }

            [Ignore]
            public DateTime Timestamp { get; set; }
        }
    }
}
