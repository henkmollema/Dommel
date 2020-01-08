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
        public void GetPaged_FetchesFirstPage(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var paged = con.GetPaged<Product>(1, 5);
            Assert.Equal(5, paged.Count());
            Assert.Collection(paged,
                p => Assert.Equal("Chai", p.Name),
                p => Assert.Equal("Chang", p.Name),
                p => Assert.Equal("Aniseed Syrup", p.Name),
                p => Assert.Equal("Chef Anton's Cajun Seasoning", p.Name),
                p => Assert.Equal("Chef Anton's Gumbo Mix", p.Name));
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetPagedAsync_FetchesFirstPage(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var paged = await con.GetPagedAsync<Product>(1, 5);
            Assert.Equal(5, paged.Count());
            Assert.Collection(paged,
                p => Assert.Equal("Chai", p.Name),
                p => Assert.Equal("Chang", p.Name),
                p => Assert.Equal("Aniseed Syrup", p.Name),
                p => Assert.Equal("Chef Anton's Cajun Seasoning", p.Name),
                p => Assert.Equal("Chef Anton's Gumbo Mix", p.Name));
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void GetPaged_FetchesSecondPage(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var paged = con.GetPaged<Product>(2, 5);
            Assert.Equal(5, paged.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetPagedAsync_FetchesSecondPage(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var paged = await con.GetPagedAsync<Product>(2, 5);
            Assert.Equal(5, paged.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void GetPaged_FetchesThirdPartialPage(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var paged = con.GetPaged<Product>(3, 5);
            Assert.True(paged.Count() >= 3, "Should contain at least 3 items");
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetPagedAsync_FetchesThirdPartialPage(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var paged = await con.GetPagedAsync<Product>(3, 5);
            Assert.True(paged.Count() >= 3, "Should contain at least 3 items");
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void SelectPaged_FetchesFirstPage(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var paged = con.SelectPaged<Product>(p => p.Name == "Chai", 1, 5);
            Assert.Single(paged);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectPagedAsync_FetchesFirstPage(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var paged = await con.SelectPagedAsync<Product>(p => p.Name == "Chai", 1, 5);
            Assert.Single(paged);
        }
    }
}
