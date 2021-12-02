using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class SelectAutoMultiMapTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void SelectMultiMap(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = con.Select<Product, Category, ProductOption, Product>(x => x.Name != null && x.CategoryId == 1);
        Assert.All(products, x => Assert.NotNull(x.Category));
        var first = products.First();
        Assert.NotEmpty(first.Options);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void FirstOrDefaultMultiMap(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = con.FirstOrDefault<Product, Category, ProductOption, Product>(x => x.Name != null && x.CategoryId == 1);
        Assert.NotNull(product);
        Assert.NotNull(product!.Category);
        Assert.NotEmpty(product.Options);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task SelectAsyncMultiMap(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.SelectAsync<Product, Category, ProductOption, Product>(x => x.Name != null && x.CategoryId == 1);
        Assert.All(products, x => Assert.NotNull(x.Category));
        var first = products.First();
        Assert.NotEmpty(first.Options);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task FirstOrDefaultAsyncMultiMap(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = await con.FirstOrDefaultAsync<Product, Category, ProductOption, Product>(x => x.Name != null && x.CategoryId == 1);
        Assert.NotNull(product);
        Assert.NotNull(product!.Category);
        Assert.NotEmpty(product.Options);
    }
}
