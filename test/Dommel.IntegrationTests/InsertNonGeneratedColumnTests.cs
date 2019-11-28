using System;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class InsertNonGeneratedColumnTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task InsertAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            // Arrange
            var generatedId = Guid.NewGuid();

            // Act
            _ = await con.InsertAsync(new Baz { BazId = generatedId });

            // Assert
            var product = await con.GetAsync<Baz>(generatedId);
            Assert.NotNull(product);
            Assert.Equal(generatedId, product.BazId);
            Assert.Equal("Baz", product.Name);
        }
    }
}
