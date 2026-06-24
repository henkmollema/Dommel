using System.Collections.Generic;
using System.Linq;
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

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void UpdateMultiple(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        con.InsertAll(new List<Product>
        {
            new Product { Name = "update-multiple", CategoryId = 10 },
            new Product { Name = "update-multiple", CategoryId = 10 },
            new Product { Name = "update-multiple", CategoryId = 10 },
        });

        try
        {
            var affected = con.UpdateMultiple<Product>(
                update => update.SetProperty(p => p.Name, "updated-sync").SetProperty(p => p.CategoryId, p => p.CategoryId * 2),
                p => p.Name == "update-multiple");

            Assert.Equal(3, affected);

            var updated = con.Select<Product>(p => p.Name == "updated-sync").ToList();
            Assert.Equal(3, updated.Count);
            Assert.All(updated, p => Assert.Equal(20, p.CategoryId));
        }
        finally
        {
            con.DeleteMultiple<Product>(p => p.Name == "update-multiple" || p.Name == "updated-sync");
        }
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task UpdateMultipleAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.InsertAllAsync(new List<Product>
        {
            new Product { Name = "update-multiple-async", CategoryId = 10 },
            new Product { Name = "update-multiple-async", CategoryId = 10 },
            new Product { Name = "update-multiple-async", CategoryId = 10 },
        });

        try
        {
            var affected = await con.UpdateMultipleAsync<Product>(
                update => update.SetProperty(p => p.Name, "updated-async").SetProperty(p => p.CategoryId, p => p.CategoryId * 2),
                p => p.Name == "update-multiple-async");

            Assert.Equal(3, affected);

            var updated = (await con.SelectAsync<Product>(p => p.Name == "updated-async")).ToList();
            Assert.Equal(3, updated.Count);
            Assert.All(updated, p => Assert.Equal(20, p.CategoryId));
        }
        finally
        {
            await con.DeleteMultipleAsync<Product>(p => p.Name == "update-multiple-async" || p.Name == "updated-async");
        }
    }
}
