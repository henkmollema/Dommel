using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class UpdateTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Update(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = con.Get<Product>(1);
            Assert.NotNull(product);

            product.Name = "Test";
            con.Update(product);

            var newProduct = con.Get<Product>(1);
            Assert.Equal("Test", newProduct.Name);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task UpdateAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = await con.GetAsync<Product>(1);
            Assert.NotNull(product);

            product.Name = "Test";
            await con.UpdateAsync(product);

            var newProduct = await con.GetAsync<Product>(1);
            Assert.Equal("Test", newProduct.Name);
        }
    }
}
