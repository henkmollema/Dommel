using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class SelectTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_Equals_Const(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.CategoryId == 1);
            Assert.Equal(10, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_Equals_Const(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.CategoryId == 1);
            Assert.Equal(10, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_Equals_LocalVar(DatabaseDriver database)
        {
            var filter = new FooBar { ProductName = "Chef Anton's Cajun Seasoning" };
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name == filter.ProductName);
            Assert.Single(products);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_Equals_LocalVar(DatabaseDriver database)
        {
            var filter = new FooBar { ProductName = "Chef Anton's Cajun Seasoning" };
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name == filter.ProductName);
            Assert.Single(products);
        }

        private class FooBar
        {
            public string? ProductName { get; set; }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_Equals_MemberAccess(DatabaseDriver database)
        {
            var filter = new FooBar { ProductName = "Chef Anton's Cajun Seasoning" };
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name == filter.ProductName);
            Assert.Single(products);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_Equals_MemberAccess(DatabaseDriver database)
        {
            var filter = new FooBar { ProductName = "Chef Anton's Cajun Seasoning" };
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name == filter.ProductName);
            Assert.Single(products);
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_ContainsConstant(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name!.Contains("Anton"));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_ContainsConstant_CaseInsensitive(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name!.Contains("anton"));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_ContainsConstant(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.Contains("Anton"));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_ContainsConstant_CaseInsensitive(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.Contains("anton"));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_ContainsVariable(DatabaseDriver database)
        {
            var productName = "Anton";
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name!.Contains(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_ContainsVariable_CaseInsensitive(DatabaseDriver database)
        {
            var productName = "anton";
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name!.Contains(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_ContainsVariable(DatabaseDriver database)
        {
            var productName = "Anton";
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.Contains(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_ContainsVariable_CaseInsensitive(DatabaseDriver database)
        {
            var productName = "anton";
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.Contains(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_StartsWith(DatabaseDriver database)
        {
            var productName = "Cha";
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name!.StartsWith(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_StartsWith_CaseInsensitive(DatabaseDriver database)
        {
            var productName = "cha";
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name!.StartsWith(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_StartsWith(DatabaseDriver database)
        {
            var productName = "Cha";
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.StartsWith(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_StartsWith_CaseInsensitive(DatabaseDriver database)
        {
            var productName = "cha";
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.StartsWith(productName));
            Assert.Equal(4, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Select_EndsWith(DatabaseDriver database)
        {
            var productName = "2";
            using var con = database.GetConnection();
            var products = con.Select<Product>(p => p.Name!.EndsWith(productName));
            Assert.Equal(5, products.Count());
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SelectAsync_EndsWith(DatabaseDriver database)
        {
            var productName = "2";
            using var con = database.GetConnection();
            var products = await con.SelectAsync<Product>(p => p.Name!.EndsWith(productName));
            Assert.Equal(5, products.Count());
        }
    }
}