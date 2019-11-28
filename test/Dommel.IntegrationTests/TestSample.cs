using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class SampleTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Sample(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            _ = con.GetAll<Product>();
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SampleAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            _ = await con.GetAllAsync<Product>();
        }
    }
}
