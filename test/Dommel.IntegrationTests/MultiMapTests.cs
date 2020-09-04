using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class MultiMapTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = con.Get<Product, Category, Product>(1, (p, c) =>
            {
                p.Category = c;
                return p;
            });

            Assert.NotNull(product);
            Assert.NotEmpty(product!.Name);
            Assert.NotNull(product.Category);
            Assert.NotNull(product.Category?.Name);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = await con.GetAsync<Product, Category, Product>(1, (p, c) =>
            {
                p.Category = c;
                return p;
            });

            Assert.NotNull(product);
            Assert.NotEmpty(product!.Name);
            Assert.NotNull(product.Category);
            Assert.NotNull(product.Category?.Name);
        }
    }
}
