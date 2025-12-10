using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class DynamicUpdateTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Update_ByProductId_UpdatesEntity(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.DeleteAllAsync<Product>();

        var product = new Product { Name = "Product 1", CategoryId = 1 };
        await con.InsertAsync(product);

        var affectedRows = await con.UpdateAsync<Product>(sql => sql
            .Set(p => p.Name, "Updated Name")
            .Where(p => p.ProductId == product.ProductId));

        Assert.Equal(1, affectedRows);
        var updatedProduct = await con.GetAsync<Product>(product.ProductId);
        Assert.Equal("Updated Name", updatedProduct!.Name);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Update_MultipleColumns_UpdatesEntity(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.DeleteAllAsync<Product>();

        var product = new Product { Name = "Product 1", CategoryId = 1 };
        await con.InsertAsync(product);

        var affectedRows = await con.UpdateAsync<Product>(sql => sql
            .Set(p => p.Name, "Updated Name")
            .Set(p => p.CategoryId, 2)
            .Where(p => p.ProductId == product.ProductId));

        Assert.Equal(1, affectedRows);
        var updatedProduct = await con.GetAsync<Product>(product.ProductId);
        Assert.Equal("Updated Name", updatedProduct!.Name);
        Assert.Equal(2, updatedProduct.CategoryId);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Update_MultipleEntities_UpdatesCorrectEntities(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.DeleteAllAsync<Product>();

        var product1 = new Product { Name = "Product 1", CategoryId = 1 };
        var product2 = new Product { Name = "Product 2", CategoryId = 1 };
        var product3 = new Product { Name = "Product 3", CategoryId = 2 };
        await con.InsertAllAsync(new[] { product1, product2, product3 });

        var affectedRows = await con.UpdateAsync<Product>(sql => sql
            .Set(p => p.Name, "Updated Category 1 Product")
            .Where(p => p.CategoryId == 1));

        Assert.Equal(2, affectedRows);
        
        var updatedProduct1 = await con.GetAsync<Product>(product1.ProductId);
        Assert.Equal("Updated Category 1 Product", updatedProduct1!.Name);

        var updatedProduct2 = await con.GetAsync<Product>(product2.ProductId);
        Assert.Equal("Updated Category 1 Product", updatedProduct2!.Name);

        var unchangedProduct3 = await con.GetAsync<Product>(product3.ProductId);
        Assert.Equal("Product 3", unchangedProduct3!.Name);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Update_WithNullValue_UpdatesEntity(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        await con.DeleteAllAsync<Product>();

        var product = new Product { Name = "Product 1", CategoryId = 1 };
        await con.InsertAsync(product);

        var affectedRows = await con.UpdateAsync<Product>(sql => sql
            .Set(p => p.Name, null)
            .Where(p => p.ProductId == product.ProductId));

        Assert.Equal(1, affectedRows);
        var updatedProduct = await con.GetAsync<Product>(product.ProductId);
        Assert.Null(updatedProduct!.Name);
    }
}
