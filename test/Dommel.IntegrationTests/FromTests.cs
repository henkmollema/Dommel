using System.Linq;
using System.Threading.Tasks;
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
    public async Task GroupBy(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var sqlBuilder = DommelMapper.GetSqlBuilder(con);
        var columnName = sqlBuilder.QuoteIdentifier("CategoryId");
        var xs = await con.FromAsync<Product, (int categoryId, int count)>(
            sql => sql.Select($"{columnName}, count(*)").GroupBy(x => x.CategoryId));
        Assert.True(xs.Any());
        Assert.All(xs, x =>
        {
            Assert.True(x.categoryId >= 0);
            Assert.True(x.count >= 0);
        });
    }
}
