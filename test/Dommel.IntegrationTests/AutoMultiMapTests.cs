using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class AutoMultiMapTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void OneToOne(Database database)
        {
            using (var con = database.GetConnection())
            {
                var product = con.Get<Product, Category, Product>(1);
                Assert.NotNull(product);
                Assert.NotNull(product.Category);
                Assert.Equal("Food", product.Category.Name);
                Assert.Equal(product.CategoryId, product.Category.Id);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void OneToOneNotExisting(Database database)
        {
            using (var con = database.GetConnection())
            {
                var product = con.Get<Product, Category, Product>(11);
                Assert.NotNull(product);
                Assert.Null(product.Category);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void OneToMany(Database database)
        {
            using (var con = database.GetConnection())
            {
                var order = con.Get<Order, OrderLine, Order>(1);
                Assert.NotNull(order);
                Assert.NotNull(order.OrderLines);
                Assert.Equal(3, order.OrderLines.Count);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void OneToManyNonExsting(Database database)
        {
            using (var con = database.GetConnection())
            {
                var order = con.Get<Order, OrderLine, Order>(2);
                Assert.NotNull(order);
                Assert.Null(order.OrderLines);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task OneToOneAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var product = await con.GetAsync<Product, Category, Product>(1);
                Assert.NotNull(product);
                Assert.NotNull(product.Category);
                Assert.Equal("Food", product.Category.Name);
                Assert.Equal(product.CategoryId, product.Category.Id);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task OneToOneNotExistingAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var product = await con.GetAsync<Product, Category, Product>(11);
                Assert.NotNull(product);
                Assert.Null(product.Category);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task OneToManyAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var order = await con.GetAsync<Order, OrderLine, Order>(1);
                Assert.NotNull(order);
                Assert.NotNull(order.OrderLines);
                Assert.Equal(3, order.OrderLines.Count);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task OneToManyNonExstingAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var order = await con.GetAsync<Order, OrderLine, Order>(2);
                Assert.NotNull(order);
                Assert.Null(order.OrderLines);
            }
        }
    }
}
