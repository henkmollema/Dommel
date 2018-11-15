using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class GetTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get(Database database)
        {
            using (var con = database.GetConnection())
            {
                var product = con.Get<Product>(1);
                Assert.NotNull(product);
                Assert.NotEmpty(product.Name);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var product = await con.GetAsync<Product>(1);
                Assert.NotNull(product);
                Assert.NotEmpty(product.Name);
            }
        }
    }
}
