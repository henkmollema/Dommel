using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class SelectTestsSqlServer : SelectTests, IClassFixture<SqlServerDatabase>
    {
        public SelectTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class SelectTestsMySql : SelectTests, IClassFixture<MySqlDatabase>
    {
        public SelectTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class SelectTests : DommelTestBase
    {
        public SelectTests(Database database) : base(database)
        {
        }

        [Fact]
        public void SelectEqual()
        {
            using (var con = GetConnection())
            {
                var products = con.Select<Product>(p => p.CategoryId == 1);
                Assert.NotEmpty(products);
            }
        }

        [Fact]
        public async Task SelectAsyncEqual()
        {
            using (var con = GetConnection())
            {
                var products = await con.SelectAsync<Product>(p => p.CategoryId == 1);
                Assert.NotEmpty(products);
            }
        }

        [Fact]
        public void SelectContains()
        {
            using (var con = GetConnection())
            {
                var products = con.Select<Product>(p => p.Name.Contains("Chai"));
                Assert.NotEmpty(products);
            }
        }

        [Fact]
        public async Task SelectAsyncContains()
        {
            using (var con = GetConnection())
            {
                var products = await con.SelectAsync<Product>(p => p.Name.Contains("Chai"));
                Assert.NotEmpty(products);
            }
        }
    }
}
