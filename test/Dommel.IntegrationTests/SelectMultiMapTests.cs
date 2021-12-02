using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class SelectMultiMapTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void SelectMultiMap(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = con.Select<Product, Category, ProductOption, Product>(
            p => p.CategoryId == 1 && p.Name != null,
            (p, c, po) =>
            {
                p.Category = c;
                if (!p.Options.Contains(po))
                {
                    p.Options.Add(po);
                }
                return p;
            });
        Assert.Equal(5, products.Count());
        Assert.All(products, x => Assert.NotNull(x.Category));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void FirstOrDefaultMultiMap(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = con.FirstOrDefault<Product, Category, ProductOption, Product>(
            p => p.CategoryId == 1 && p.Name != null,
            (p, c, po) =>
            {
                p.Category = c;
                if (!p.Options.Contains(po))
                {
                    p.Options.Add(po);
                }
                return p;
            });
        Assert.NotNull(product);
        Assert.NotNull(product!.Category);
        Assert.NotEmpty(product.Options);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task SelectAsyncMultiMap(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.SelectAsync<Product, Category, ProductOption, Product>(
            p => p.CategoryId == 1 && p.Name != null,
            (p, c, po) =>
            {
                p.Category = c;
                if (!p.Options.Contains(po))
                {
                    p.Options.Add(po);
                }
                return p;
            });
        Assert.Equal(5, products.Count());
        Assert.All(products, x => Assert.NotNull(x.Category));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task FirstOrDefaultAsyncMultiMap(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = await con.FirstOrDefaultAsync<Product, Category, ProductOption, Product>(
            p => p.CategoryId == 1 && p.Name != null,
            (p, c, po) =>
            {
                p.Category = c;
                if (!p.Options.Contains(po))
                {
                    p.Options.Add(po);
                }
                return p;
            });
        Assert.NotNull(product);
        Assert.NotNull(product!.Category);
        Assert.NotEmpty(product.Options);
    }
}
