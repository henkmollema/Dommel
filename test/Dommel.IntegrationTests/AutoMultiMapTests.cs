using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class AutoMultiMapTestsSqlServer : AutoMultiMapTests, IClassFixture<SqlServerDatabase>
    {
        public AutoMultiMapTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class AutoMultiMapTestsMySql : AutoMultiMapTests, IClassFixture<MySqlDatabase>
    {
        public AutoMultiMapTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class AutoMultiMapTests : DommelTestBase
    {
        public AutoMultiMapTests(Database database) : base(database)
        {
        }

        [Fact]
        public async Task OneToOne()
        {
            using (var con = GetConnection())
            {
                var product = await con.GetAsync<Product, Category, Product>(1);
                Assert.NotNull(product);
                Assert.NotNull(product.Category);
                Assert.Equal("Food", product.Category.Name);
                Assert.Equal(product.CategoryId, product.Category.Id);
            }
        }

        [Fact]
        public async Task OneToMany()
        {
            using (var con = GetConnection())
            {
                var order = await con.GetAsync<Order, OrderLine, Order>(1);
                Assert.NotNull(order);
                Assert.NotNull(order.OrderLines);
                Assert.Equal(3, order.OrderLines.Count);
            }
        }
    }
}
