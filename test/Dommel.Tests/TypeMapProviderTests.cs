using Dapper;
using Xunit;

namespace Dommel.Tests
{
    public class TypeMapProviderTests
    {
        [Fact]
        public void AddsTypeMapProvider()
        {
            // Arrange
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            _ = DommelMapper.QueryCache; // Reference anything from DommelMapper to make sure the static ctor runs

            // Act
            var typeMap = SqlMapper.TypeMapProvider(typeof(Poco));
            
            // Assert
            Assert.Equal(typeof(Poco).GetProperty(nameof(Poco.Foo)), typeMap.GetMember("Foo")?.Property);
            Assert.Equal(typeof(Poco).GetProperty(nameof(Poco.BarBaz)), typeMap.GetMember("Bar_Baz")?.Property);
        }

        private class Poco
        {
            public int Id { get; set; }
            
            public string? Foo { get; set; }
            
            public string? BarBaz { get; set; }
        }
    }
}