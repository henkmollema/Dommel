using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Dommel.Json.Tests
{
    public class JsonPropertyResolverTests
    {
        private static readonly Type LeadType = typeof(Lead);

        [Fact]
        public void DefaultBehavior()
        {
            // Arrange
            var resolver = new JsonPropertyResolver(Array.Empty<Type>());

            // Act
            var props = resolver.ResolveProperties(LeadType);

            // Assert
            Assert.Collection(
                props,
                p => Assert.Equal(p, LeadType.GetProperty("Id")),
                p => Assert.Equal(p, LeadType.GetProperty("DateCreated")),
                p => Assert.Equal(p, LeadType.GetProperty("Email"))
            );
        }

        [Fact]
        public void ResolvesComplexType()
        {
            // Arrange
            var resolver = new JsonPropertyResolver(new[] { typeof(LeadData) });

            // Act
            var props = resolver.ResolveProperties(LeadType);

            // Assert
            Assert.Collection(
                props,
                p => Assert.Equal(p, LeadType.GetProperty("Id")),
                p => Assert.Equal(p, LeadType.GetProperty("DateCreated")),
                p => Assert.Equal(p, LeadType.GetProperty("Email")),
                p => Assert.Equal(p, LeadType.GetProperty("Data"))
            );
        }
    }
}
