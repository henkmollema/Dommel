using Xunit;

namespace Dommel.Json.Tests;

public class DommelJsonOptionsTests
{
    [Fact]
    public void SetsEntityAssemblies()
    {
        // Arrange
        var assemblies = new[] { typeof(DommelJsonOptions).Assembly };

        // Act
        var options = new DommelJsonOptions { EntityAssemblies = assemblies };

        // Assert
        Assert.Equal(assemblies, options.EntityAssemblies);
        Assert.Null(options.JsonTypeHandler);
    }
}
