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
            product!.Name = "Test";
            product.SetSlug("test");
            con.Update(product);

            var newProduct = con.Get<Product>(1);
            Assert.Equal("Test", newProduct!.Name);
            Assert.Equal("test", newProduct.Slug);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task UpdateAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = await con.GetAsync<Product>(1);
            Assert.NotNull(product);
            product!.Name = "Test";
            product.SetSlug("test");
            await con.UpdateAsync(product);

            var newProduct = await con.GetAsync<Product>(1);
            Assert.Equal("Test", newProduct!.Name);
            Assert.Equal("test", newProduct.Slug);
        }
    }
}
