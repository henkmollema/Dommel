using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class AutoMultiMapTests
    {
        [Fact]
        public void Map1()
        {
            var del = CreateMapDelegate<Product, Category, DontMap, DontMap, DontMap, DontMap, DontMap, Product>();
            var product = new Product();
            var category = new Category();
            var x = del.DynamicInvoke(product, category);
            var mappedProduct = Assert.IsType<Product>(x);
            Assert.NotNull(mappedProduct.Category);
        }

        [Fact]
        public void Map2()
        {
            var del = CreateMapDelegate<Product, Category, Category, DontMap, DontMap, DontMap, DontMap, Product>();
            var product = new Product();
            var category = new Category();
            var x = del.DynamicInvoke(product, category, category);
            var mappedProduct = Assert.IsType<Product>(x);
            Assert.NotNull(mappedProduct.Category);
        }

        [Fact]
        public void Map3()
        {
            var del = CreateMapDelegate<Product, Category, Category, Category, DontMap, DontMap, DontMap, Product>();
            var product = new Product();
            var category = new Category();
            var x = del.DynamicInvoke(product, category, category, category);
            var mappedProduct = Assert.IsType<Product>(x);
            Assert.NotNull(mappedProduct.Category);
        }

        [Fact]
        public void Map4()
        {
            var del = CreateMapDelegate<Product, Category, Category, Category, Category, DontMap, DontMap, Product>();
            var product = new Product();
            var category = new Category();
            var x = del.DynamicInvoke(product, category, category, category, category);
            var mappedProduct = Assert.IsType<Product>(x);
            Assert.NotNull(mappedProduct.Category);
        }

        [Fact]
        public void Map5()
        {
            var del = CreateMapDelegate<Product, Category, Category, Category, Category, Category, DontMap, Product>();
            var product = new Product();
            var category = new Category();
            var x = del.DynamicInvoke(product, category, category, category, category, category);
            var mappedProduct = Assert.IsType<Product>(x);
            Assert.NotNull(mappedProduct.Category);
        }

        [Fact]
        public void Map6()
        {
            var del = CreateMapDelegate<Product, Category, Category, Category, Category, Category, Category, Product>();
            var product = new Product();
            var category = new Category();
            var x = del.DynamicInvoke(product, category, category, category, category, category, category);
            var mappedProduct = Assert.IsType<Product>(x);
            Assert.NotNull(mappedProduct.Category);
        }
    }
}
