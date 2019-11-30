using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class SelectTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsyncEqual(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.CategoryId == 1);
            Assert.Equal(10, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsyncContains(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.Contains("Anton"));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsyncContainsVariable(DatabaseDriver database)
        {
            var productName = "Anton";
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.Contains(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsyncStartsWith(DatabaseDriver database)
        {
            var productName = "Cha";
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.StartsWith(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsyncEndsWith(DatabaseDriver database)
        {
            var productName = "2";
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.EndsWith(productName));
            Assert.Equal(5, products.Count());
        }
    }
}
