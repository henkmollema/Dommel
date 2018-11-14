using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class GetAllTestsSqlServer : GetAllTests, IClassFixture<SqlServerDatabase>
    {
        public GetAllTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class GetAllTestsMySql : GetAllTests, IClassFixture<MySqlDatabase>
    {
        public GetAllTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class GetAllTests : DommelTestBase
    {
        public GetAllTests(Database database) : base(database)
        {
        }

        [Fact]
        public void GetAll()
        {
            using (var con = GetConnection())
            {
                var products = con.GetAll<Product>();
                Assert.NotEmpty(products);
                Assert.All(products, p => Assert.NotEmpty(p.Name));
            }
        }

        [Fact]
        public async Task GetAllAsync()
        {
            using (var con = GetConnection())
            {
                var products = await con.GetAllAsync<Product>();
                Assert.NotEmpty(products);
                Assert.All(products, p => Assert.NotEmpty(p.Name));
            }
        }
    }
}
