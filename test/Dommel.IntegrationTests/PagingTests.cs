using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class PagingTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Fetches_FirstPage(Database database)
        {
            using (var con = database.GetConnection())
            {
                var paged = con.GetPaged<Product>(1, 5).ToArray();
                Assert.Equal(5, paged.Length);
                Assert.Collection(paged,
                    p => Assert.Equal("Chai", p.Name),
                    p => Assert.Equal("Chang", p.Name),
                    p => Assert.Equal("Aniseed Syrup", p.Name),
                    p => Assert.Equal("Chef Anton's Cajun Seasoning", p.Name),
                    p => Assert.Equal("Chef Anton's Gumbo Mix", p.Name));
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Fetches_SecondPage(Database database)
        {
            using (var con = database.GetConnection())
            {
                var paged = con.GetPaged<Product>(2, 5).ToArray();
                Assert.Equal(5, paged.Length);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task Fetches_ThirdPartialPage(Database database)
        {
            using (var con = database.GetConnection())
            {
                var paged = (await con.GetPagedAsync<Product>(3, 5)).ToArray();
                Assert.True(paged.Length >= 3, "Should contain at least 3 items");
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectPaged_FetchesFirstPage(Database database)
        {
            using (var con = database.GetConnection())
            {
                var paged = (await con.SelectPagedAsync<Product>(p => p.Name == "Chai", 1, 5)).ToArray();
                Assert.Single(paged);
            }
        }
    }
}
