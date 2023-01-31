using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class FromTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void SelectAllSync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = con.From<Product>(sql => sql.Select());
        Assert.NotEmpty(products);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void SelectProjectSync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = con.From<Product>(sql =>
            sql.Select(p => new { p.ProductId, p.Name }));
        Assert.NotEmpty(products);
        Assert.All(products, p => Assert.Equal(0, p.CategoryId));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task SelectAll(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.FromAsync<Product>(sql => sql.Select());
        Assert.NotEmpty(products);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task SelectProject(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.FromAsync<Product>(sql =>
            sql.Select(p => new { p.ProductId, p.Name }));
        Assert.NotEmpty(products);
        Assert.All(products, p => Assert.Equal(0, p.CategoryId));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task Select_Where(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.FromAsync<Product>(
            sql => sql.Select(p => new { p.Name, p.CategoryId })
            .Where(p => p.Name!.StartsWith("Chai")));

        Assert.NotEmpty(products);
        Assert.All(products, p => Assert.StartsWith("Chai", p.Name));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task OrderBy(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.FromAsync<Product>(
            sql => sql.OrderBy(p => p.Name).Select());

        Assert.NotEmpty(products);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task OrderByPropertyInfo(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.FromAsync<Product>(
            sql => sql.OrderBy(typeof(Product).GetProperty("Name")!).Select());

        Assert.NotEmpty(products);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task KitchenSink(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.FromAsync<Product>(sql =>
            sql.Select(p => new { p.Name, p.CategoryId })
                .Where(p => p.Name!.StartsWith("Chai") && p.CategoryId == 1)
                .OrWhere(p => p.Name != null)
                .AndWhere(p => p.CategoryId != 0)
                .OrderBy(p => p.CategoryId)
                .OrderByDescending(p => p.Name)
                .Page(1, 5));

        Assert.Equal(5, products.Count());
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task OrdersBy(DatabaseDriver database)
    {
        var orders = new List<OrderableColumn<Product>>();
        orders.Add(new(p=>p.CategoryId, SortDirectionEnum.Ascending));
        orders.Add(new(p=>p.Name, SortDirectionEnum.Descending));

        using var con = database.GetConnection();
        var products = await con.FromAsync<Product>(sql =>
            sql.Select(p => new { p.Name, p.CategoryId })
                .Where(p => p.CategoryId == 1)
                .OrderBy(orders)
                .Page(1, 5));

        Assert.Equal(5, products.Count());
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task OrdersByProperty(DatabaseDriver database)
    {
        var product = new Product();
        var orders = new List<OrderablePropertyInfo>();
        orders.Add(new(typeof(Product).GetProperty("CategoryId"), SortDirectionEnum.Ascending));
        orders.Add(new(typeof(Product).GetProperty("Name"), SortDirectionEnum.Descending));

        using var con = database.GetConnection();
        var products = await con.FromAsync<Product>(sql =>
            sql.Select(p => new { p.Name, p.CategoryId })
                .Where(p => p.CategoryId == 1)
                .OrderBy(orders)
                .Page(1, 5));

        Assert.Equal(5, products.Count());
    }
}
