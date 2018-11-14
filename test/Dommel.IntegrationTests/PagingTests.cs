using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class PagingTestsSqlServer : PagingTests, IClassFixture<SqlServerDatabase>
    {
        public PagingTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class PagingTestsMySql : PagingTests, IClassFixture<MySqlDatabase>
    {
        public PagingTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class PagingTests : DommelTestBase
    {
        public PagingTests(Database database) : base(database)
        {
        }

        [Fact]
        public void Fetches_FirstPage()
        {
            using (var con = GetConnection())
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

        [Fact]
        public void Fetches_SecondPage()
        {
            using (var con = GetConnection())
            {
                var paged = con.GetPaged<Product>(2, 5).ToArray();
                Assert.Equal(5, paged.Length);
            }
        }

        [Fact]
        public async Task Fetches_ThirdPartialPage()
        {
            using (var con = GetConnection())
            {
                var paged = (await con.GetPagedAsync<Product>(3, 5)).ToArray();
                Assert.Equal(3, paged.Length);
            }
        }

        [Fact]
        public async Task SelectPaged_FetchesFirstPage()
        {
            using (var con = GetConnection())
            {
                var paged = (await con.SelectPagedAsync<Product>(p => p.Name == "Chai", 1, 5)).ToArray();
                Assert.Single(paged);
            }
        }
    }
}
