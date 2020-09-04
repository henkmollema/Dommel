using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class AutoMultiMapTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get_OneToOne(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = con.GetAll<Product>();
            var product = con.Get<Product, Category, Product>(products.First().ProductId);
            Assert.NotNull(product);
            Assert.NotNull(product!.Category);
            Assert.Equal("Food", product.Category?.Name);
            Assert.Equal(product.CategoryId, product.Category?.CategoryId);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync_OneToOne(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = await con.GetAllAsync<Product>();
            var product = await con.GetAsync<Product, Category, Product>(products.First().ProductId);
            Assert.NotNull(product);
            Assert.NotNull(product!.Category);
            Assert.Equal("Food", product.Category?.Name);
            Assert.Equal(product.CategoryId, product.Category?.CategoryId);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get_OneToOneOneToMany(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = con.GetAll<Product>();
            var product = con.Get<Product, Category, ProductOption, Product>(products.First().ProductId);
            Assert.NotNull(product);
            Assert.NotNull(product!.Category);
            Assert.Equal("Food", product.Category?.Name);
            Assert.Equal(product.CategoryId, product.Category?.CategoryId);
            Assert.Single(product.Options);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync_OneToOneOneToMany(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = await con.GetAllAsync<Product>();
            var product = await con.GetAsync<Product, Category, ProductOption, Product>(products.First().ProductId);
            Assert.NotNull(product);
            Assert.NotNull(product!.Category);
            Assert.Equal("Food", product.Category?.Name);
            Assert.Equal(product.CategoryId, product.Category?.CategoryId);
            Assert.Single(product.Options);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get_OneToOneNotExisting(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var fooProduct = con.FirstOrDefault<Product>(x => x.Name == "Foo");
            Assert.NotNull(fooProduct);
            var product = con.Get<Product, Category, Product>(fooProduct!.ProductId);
            Assert.NotNull(product);
            Assert.Null(product!.Category);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync_OneToOneNotExisting(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var fooProduct = await con.FirstOrDefaultAsync<Product>(x => x.Name == "Foo");
            Assert.NotNull(fooProduct);
            var product = await con.GetAsync<Product, Category, Product>(fooProduct!.ProductId);
            Assert.NotNull(product);
            Assert.Null(product!.Category);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get_OneToMany(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var orders = con.GetAll<Order>();
            var order = con.Get<Order, OrderLine, Order>(orders.First().Id);
            Assert.NotNull(order);
            Assert.NotNull(order!.OrderLines);
            Assert.Equal(3, order.OrderLines?.Count);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync_OneToMany(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var orders = await con.GetAllAsync<Order>();
            var order = await con.GetAsync<Order, OrderLine, Order>(orders.First().Id);
            Assert.NotNull(order);
            Assert.NotNull(order!.OrderLines);
            Assert.Equal(3, order.OrderLines?.Count);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get_OneToManyNonExsting(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var order = con.Get<Order, OrderLine, Order>(2);
            Assert.NotNull(order);
            Assert.Null(order!.OrderLines);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync_OneToManyNonExsting(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var order = await con.GetAsync<Order, OrderLine, Order>(2);
            Assert.NotNull(order);
            Assert.Null(order!.OrderLines);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void GetAll_OneToOne(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = con.GetAll<Product, Category, Product>();
            Assert.NotEmpty(products);
            Assert.Equal(products.Select(x => x.Name).Distinct(), products.Select(x => x.Name));
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAllAsync_OneToOne(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = await con.GetAllAsync<Product, Category, Product>();
            Assert.NotEmpty(products);
            Assert.Equal(products.Select(x => x.Name).Distinct(), products.Select(x => x.Name));
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void GetAll_OneToMany(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var orders = con.GetAll<Order, OrderLine, Order>();
            Assert.Collection(orders,
                x =>
                {
                    Assert.Equal(new DateTime(2011, 1, 1), x.Created);
                    Assert.NotEmpty(x.OrderLines);
                },
                x =>
                {
                    Assert.Equal(new DateTime(2012, 2, 2), x.Created);
                    Assert.Null(x.OrderLines);
                });
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAllAsync_OneToMany(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var orders = await con.GetAllAsync<Order, OrderLine, Order>();
            Assert.Collection(orders,
                x =>
                {
                    Assert.Equal(new DateTime(2011, 1, 1), x.Created);
                    Assert.NotEmpty(x.OrderLines);
                },
                x =>
                {
                    Assert.Equal(new DateTime(2012, 2, 2), x.Created);
                    Assert.Null(x.OrderLines);
                });
        }
    }

    public class ProductWithCategories
    {
        public int Id { get; set; }

        public int Category1Id { get; set; }

        [ForeignKey(nameof(Category1Id))]
        public Category? Category1 { get; set; }

        public int Category2Id { get; set; }

        [ForeignKey(nameof(Category2Id))]
        public Category? Category2 { get; set; }

        public int Category3Id { get; set; }

        [ForeignKey(nameof(Category3Id))]
        public Category? Category3 { get; set; }

        public int Category4Id { get; set; }

        [ForeignKey(nameof(Category4Id))]
        public Category? Category4 { get; set; }

        public int Category5Id { get; set; }

        [ForeignKey(nameof(Category5Id))]
        public Category? Category5 { get; set; }

        public int Category6Id { get; set; }

        [ForeignKey(nameof(Category6Id))]
        public Category? Category6 { get; set; }

        public int Category7Id { get; set; }

        [ForeignKey(nameof(Category7Id))]
        public Category? Category7 { get; set; }

        public int Category8Id { get; set; }

        [ForeignKey(nameof(Category8Id))]
        public Category? Category8 { get; set; }
    }
}
