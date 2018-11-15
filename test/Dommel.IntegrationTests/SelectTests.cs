using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class SelectTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void SelectEqual(DatabaseDriver database)
        {
            using (var con = database.GetConnection())
            {
                var products = con.Select<Product>(p => p.CategoryId == 1);
                Assert.NotEmpty(products);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsyncEqual(DatabaseDriver database)
        {
            using (var con = database.GetConnection())
            {
                var products = await con.SelectAsync<Product>(p => p.CategoryId == 1);
                Assert.NotEmpty(products);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void SelectContains(DatabaseDriver database)
        {
            using (var con = database.GetConnection())
            {
                var products = con.Select<Product>(p => p.Name.Contains("Chai"));
                Assert.NotEmpty(products);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsyncContains(DatabaseDriver database)
        {
            using (var con = database.GetConnection())
            {
                var products = await con.SelectAsync<Product>(p => p.Name.Contains("Chai"));
                Assert.NotEmpty(products);
            }
        }
    }
}
