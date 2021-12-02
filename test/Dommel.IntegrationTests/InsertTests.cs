using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class InsertTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Insert(DatabaseDriver database)
    {
        // Arrange
        using var con = database.GetConnection();
        var productToInsert = new Product { Name = "Foo Product" };
        productToInsert.SetSlug("foo-product");

        // Act
        var id = Convert.ToInt32(con.Insert(productToInsert));

        // Assert
        var product = con.Get<Product>(id);
        Assert.NotNull(product);
        Assert.Equal(id, product!.ProductId);
        Assert.Equal("Foo Product", product.Name);
        Assert.Equal("foo-product", product.Slug);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task InsertAsync(DatabaseDriver database)
    {
        // Arrange
        using var con = database.GetConnection();
        var productToInsert = new Product { Name = "Foo Product" };
        productToInsert.SetSlug("foo-product");

        // Act
        var id = Convert.ToInt32(await con.InsertAsync(productToInsert));

        // Assert
        var product = await con.GetAsync<Product>(id);
        Assert.NotNull(product);
        Assert.Equal(id, product!.ProductId);
        Assert.Equal("Foo Product", product.Name);
        Assert.Equal("foo-product", product.Slug);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void InsertAll(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ps = new List<Foo>
                {
                    new Foo { Name = "blah" },
                    new Foo { Name = "blah" },
                    new Foo { Name = "blah" },
                };

        con.InsertAll(ps);

        var blahs = con.Select<Foo>(p => p.Name == "blah");
        Assert.Equal(3, blahs.Count());
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task InsertAllAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ps = new List<Bar>
                {
                    new Bar { Name = "blah" },
                    new Bar { Name = "blah" },
                    new Bar { Name = "blah" },
                };

        await con.InsertAllAsync(ps);

        var blahs = await con.SelectAsync<Bar>(p => p.Name == "blah");
        Assert.Equal(3, blahs.Count());
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void InsertAllEmtyList(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ps = new List<Product>();
        con.InsertAll(ps);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task InsertAllAsyncEmtyList(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ps = new List<Product>();
        await con.InsertAllAsync(ps);
    }
}
