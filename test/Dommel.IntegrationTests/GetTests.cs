using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class GetTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = con.Get<Product>(1);
            Assert.NotNull(product);
            Assert.NotEmpty(product.Name);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = await con.GetAsync<Product>(1);
            Assert.NotNull(product);
            Assert.NotEmpty(product.Name);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get_ParamsOverload(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = con.Get<Product>(new object[] { 1 });
            Assert.NotNull(product);
            Assert.NotEmpty(product.Name);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync_ParamsOverload(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = await con.GetAsync<Product>(new object[] { 1 });
            Assert.NotNull(product);
            Assert.NotEmpty(product.Name);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get_ThrowsWhenCompositeKey(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var ex = Assert.Throws<InvalidOperationException>(() => con.Get<ProductsCategories>(1));
            Assert.Equal("Entity ProductsCategories contains more than one key property.Use the Get<T> overload which supports passing multiple IDs.", ex.Message);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync_ThrowsWhenCompositeKey(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => con.GetAsync<ProductsCategories>(1));
            Assert.Equal("Entity ProductsCategories contains more than one key property.Use the Get<T> overload which supports passing multiple IDs.", ex.Message);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Get_CompositeKey(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = con.Get<ProductsCategories>(1, 1);
            Assert.NotNull(product);
            Assert.Equal(1, product.ProductId);
            Assert.Equal(1, product.CategoryId);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task GetAsync_CompositeKey(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var product = await con.GetAsync<ProductsCategories>(1, 1);
            Assert.NotNull(product);
            Assert.Equal(1, product.ProductId);
            Assert.Equal(1, product.CategoryId);
        }
    }

    public class ProductsCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }
    }
}
