using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
            var resolver = new DefaultPropertyResolver();
            var type = typeof(Foo);

            // Act
            var props = resolver.ResolveProperties(type).Select(x => x.Property);

            // Assert
            Assert.Collection(props,
                x => Assert.Equal(x, type.GetProperty("Object")),
                x => Assert.Equal(x, type.GetProperty("String")),
                x => Assert.Equal(x, type.GetProperty("Guid")),
                x => Assert.Equal(x, type.GetProperty("Decimal")),
                x => Assert.Equal(x, type.GetProperty("Double")),
                x => Assert.Equal(x, type.GetProperty("Float")),
                x => Assert.Equal(x, type.GetProperty("DateTime")),
                x => Assert.Equal(x, type.GetProperty("DateTimeOffset")),
                x => Assert.Equal(x, type.GetProperty("Timespan")),
                x => Assert.Equal(x, type.GetProperty("Bytes")),
                x => Assert.Equal(x, type.GetProperty("ReadonlyProp")),
                x => Assert.Equal(x, type.GetProperty("PrivateSetterProp"))
            );
        }

        [Fact]
        public void Resolves_WithCustomResolver()
        {
            // Arrange
            var resolver = new CustomResolver();
            var type = typeof(Foo);

            // Act
            var props = resolver.ResolveProperties(type).Select(x => x.Property);

            // Assert
            Assert.Collection(props,
                x => Assert.Equal(x, type.GetProperty("String")),
                x => Assert.Equal(x, type.GetProperty("Guid")),
                x => Assert.Equal(x, type.GetProperty("Decimal")),
                x => Assert.Equal(x, type.GetProperty("Double")),
                x => Assert.Equal(x, type.GetProperty("Float")),
                x => Assert.Equal(x, type.GetProperty("DateTime")),
                x => Assert.Equal(x, type.GetProperty("DateTimeOffset")),
                x => Assert.Equal(x, type.GetProperty("Timespan")),
                x => Assert.Equal(x, type.GetProperty("Bytes")),
                x => Assert.Equal(x, type.GetProperty("ReadonlyProp")),
                x => Assert.Equal(x, type.GetProperty("PrivateSetterProp"))
            );
        }

        [Fact]
        public void IgnoresIgnoreAttribute()
        {
            // Arrange
            var resolver = new DefaultPropertyResolver();
            var type = typeof(Bar);

            // Act
            var props = resolver.ResolveProperties(type).Select(x => x.Property);

            // Assert
            var prop = Assert.Single(props);
            Assert.Equal(type.GetProperty("Id"), prop);
        }

        [Fact]
        public void IgnoresNotMappedAttribute()
        {
            // Arrange
            var resolver = new DefaultPropertyResolver();
            var type = typeof(Baz);

            // Act
            var props = resolver.ResolveProperties(type).Select(x => x.Property);

            // Assert
            var prop = Assert.Single(props);
            Assert.Equal(type.GetProperty("Id"), prop);
        }

        private class CustomResolver : DefaultPropertyResolver
        {
            // Create a new hashset without the object type.
            protected override HashSet<Type> PrimitiveTypes => new HashSet<Type>(base.PrimitiveTypes.Skip(1));
        }

        private class Foo
        {
            public class Bar { }

            public Bar? Baz { get; set; }

            //
            // 10 primitive property types from here
            public object? Object { get; set; }

            public string? String { get; set; }

            public Guid? Guid { get; set; }

            public decimal? Decimal { get; set; }

            public double? Double { get; set; }

            public float? Float { get; set; }

            public DateTime DateTime { get; set; }

            public DateTimeOffset? DateTimeOffset { get; set; }

            public TimeSpan? Timespan { get; set; }

            public byte[]? Bytes { get; set; }

            //
            // Properties without a (public) setter
            public string? ReadonlyProp { get; }

            public string? PrivateSetterProp { get; private set; }
        }

        private class Bar
        {
            public int Id { get; set; }

            [Ignore]
            public DateTime Timestamp { get; set; }
        }

        private class Baz
        {
            public int Id { get; set; }

            [NotMapped]
            public DateTime Timestamp { get; set; }
        }
    }
}
