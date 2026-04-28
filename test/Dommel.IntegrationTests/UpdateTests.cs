using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class UpdateTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Update(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = con.Get<Product>(1);
        Assert.NotNull(product);
        var originalName = product!.Name;
        try
        {
            product.Name = "Test";
            product.SetSlug("test");
            con.Update(product);

            var newProduct = con.Get<Product>(1);
            Assert.Equal("Test", newProduct!.Name);
            Assert.Equal("test", newProduct.Slug);
        }
        finally
        {
            product.Name = originalName;
            con.Update(product);
        }
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task UpdateAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = await con.GetAsync<Product>(1);
        Assert.NotNull(product);
        var originalName = product!.Name;
        try
        {
            product.Name = "Test";
            product.SetSlug("test");
            await con.UpdateAsync(product);

            var newProduct = await con.GetAsync<Product>(1);
            Assert.Equal("Test", newProduct!.Name);
            Assert.Equal("test", newProduct.Slug);
        }
        finally
        {
            product.Name = originalName;
            await con.UpdateAsync(product);
        }
    }
}
