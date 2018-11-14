using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class UpdateTestsSqlServer : UpdateTests, IClassFixture<SqlServerDatabase>
    {
        public UpdateTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class UpdateTestsMySql : UpdateTests, IClassFixture<MySqlDatabase>
    {
        public UpdateTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class UpdateTests : DommelTestBase
    {
        public UpdateTests(Database database) : base(database)
        {
        }

        [Fact]
        public void Update()
        {
            using (var con = GetConnection())
            {
                var product = con.Get<Product>(1);
                Assert.NotNull(product);

                product.Name = "Test";
                con.Update(product);

                var newProduct = con.Get<Product>(1);
                Assert.Equal("Test", newProduct.Name);
            }
        }

        [Fact]
        public async Task UpdateAsync()
        {
            using (var con = GetConnection())
            {
                var product = await con.GetAsync<Product>(1);
                Assert.NotNull(product);

                product.Name = "Test";
                con.Update(product);

                var newProduct = await con.GetAsync<Product>(1);
                Assert.Equal("Test", newProduct.Name);
            }
        }
    }
}
