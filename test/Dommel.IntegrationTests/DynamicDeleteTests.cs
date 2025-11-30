using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class DynamicDeleteTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Delete_ByProductId_DeletesEntity(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.DeleteAllAsync<Product>();

        var product1 = new Product { Name = "Product 1", CategoryId = 1 };
        var product2 = new Product { Name = "Product 2", CategoryId = 1 };
        var product3 = new Product { Name = "Product 3", CategoryId = 2 };
        await con.InsertAllAsync(new[] { product1, product2, product3 });

        var initialCount = (await con.GetAllAsync<Product>()).Count();
        Assert.Equal(3, initialCount);

        var affectedRows = await con.DeleteAsync<Product>(sql => sql.Where(p => p.ProductId == product1.ProductId));

        Assert.Equal(1, affectedRows);
        var remainingProducts = (await con.GetAllAsync<Product>()).Count();
        Assert.Equal(initialCount - 1, remainingProducts);
        Assert.Null(await con.GetAsync<Product>(product1.ProductId));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Delete_ByCategoryId_DeletesMultipleEntities(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.DeleteAllAsync<Product>();

        var product1 = new Product { Name = "Product 1", CategoryId = 1 };
        var product2 = new Product { Name = "Product 2", CategoryId = 1 };
        var product3 = new Product { Name = "Product 3", CategoryId = 2 };
        await con.InsertAllAsync(new[] { product1, product2, product3 });

        var initialCount = (await con.GetAllAsync<Product>()).Count();
        Assert.Equal(3, initialCount);

        var affectedRows = await con.DeleteAsync<Product>(sql => sql.Where(p => p.CategoryId == 1));

        Assert.Equal(2, affectedRows);
        var remainingProducts = (await con.GetAllAsync<Product>()).Count();
        Assert.Equal(initialCount - 2, remainingProducts);
        Assert.NotNull(await con.GetAsync<Product>(product3.ProductId));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Delete_WithMultipleWhereClauses_DeletesCorrectEntities(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.DeleteAllAsync<Product>();

        var product1 = new Product { Name = "Product 1", CategoryId = 1 };
        var product2 = new Product { Name = "Product 2", CategoryId = 1 };
        var product3 = new Product { Name = "Product 3", CategoryId = 2 };
        var product4 = new Product { Name = "Product 4", CategoryId = 2 };
        await con.InsertAllAsync(new[] { product1, product2, product3, product4 });

        var initialCount = (await con.GetAllAsync<Product>()).Count();
        Assert.Equal(4, initialCount);

        var affectedRows = await con.DeleteAsync<Product>(sql => sql
            .Where(p => p.CategoryId == 1)
            .OrWhere(p => p.ProductId == product4.ProductId));

        Assert.Equal(3, affectedRows);
        var remainingProducts = (await con.GetAllAsync<Product>()).Count();
        Assert.Equal(initialCount - 3, remainingProducts);
        Assert.Null(await con.GetAsync<Product>(product1.ProductId));
        Assert.Null(await con.GetAsync<Product>(product2.ProductId));
        Assert.NotNull(await con.GetAsync<Product>(product3.ProductId));
        Assert.Null(await con.GetAsync<Product>(product4.ProductId));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Delete_AllEntities(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.DeleteAllAsync<Product>();

        var product1 = new Product { Name = "Product 1", CategoryId = 1 };
        var product2 = new Product { Name = "Product 2", CategoryId = 1 };
        await con.InsertAllAsync(new[] { product1, product2 });

        var initialCount = (await con.GetAllAsync<Product>()).Count();
        Assert.Equal(2, initialCount);

        var affectedRows = await con.DeleteAsync<Product>(sql => sql.Where(p => true)); // Delete all

        Assert.Equal(2, affectedRows);
        var remainingProducts = (await con.GetAllAsync<Product>()).Count();
        Assert.Equal(0, remainingProducts);
    }
}
