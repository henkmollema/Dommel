using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class SampleTestsSqlServer : SampleTests, IClassFixture<SqlServerDatabase>
    {
        public SampleTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class SampleTestsMySql : SampleTests, IClassFixture<MySqlDatabase>
    {
        public SampleTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class SampleTests : DommelTestBase
    {
        public SampleTests(Database database) : base(database)
        {
        }

        [Fact]
        public void Sample()
        {
            using (var con = GetConnection())
            {
                _ = con.GetAll<Product>();
            }
        }

        [Fact]
        public async Task SampleAsync()
        {
            using (var con = GetConnection())
            {
                _ = await con.GetAllAsync<Product>();
            }
        }
    }
}
