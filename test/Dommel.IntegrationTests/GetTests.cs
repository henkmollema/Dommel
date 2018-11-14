using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class GetTestsSqlServer : GetTests, IClassFixture<SqlServerDatabase>
    {
        public GetTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class GetTestsMySql : GetTests, IClassFixture<MySqlDatabase>
    {
        public GetTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class GetTests : DommelTestBase
    {
        public GetTests(Database database) : base(database)
        {
        }

        [Fact]
        public void Get()
        {
            using (var con = GetConnection())
            {
                var product = con.Get<Product>(1);
                Assert.NotNull(product);
                Assert.NotEmpty(product.Name);
            }
        }

        [Fact]
        public async Task GetAsync()
        {
            using (var con = GetConnection())
            {
                var product = await con.GetAsync<Product>(1);
                Assert.NotNull(product);
                Assert.NotEmpty(product.Name);
            }
        }
    }
}
