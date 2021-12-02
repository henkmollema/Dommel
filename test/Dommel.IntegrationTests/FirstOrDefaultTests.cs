using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class FirstOrDefaultTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void FirstOrDefault_Equals(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = con.FirstOrDefault<Product>(p => p.CategoryId == 1);
        Assert.NotNull(product);
    }
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task FirstOrDefaultAsync_Equals(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = await con.FirstOrDefaultAsync<Product>(p => p.CategoryId == 1);
        Assert.NotNull(product);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void FirstOrDefault_ContainsConstant(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = con.FirstOrDefault<Product>(p => p.Name!.Contains("Anton"));
        Assert.NotNull(product);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task FirstOrDefaultAsync_ContainsConstant(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var product = await con.FirstOrDefaultAsync<Product>(p => p.Name!.Contains("Anton"));
        Assert.NotNull(product);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void FirstOrDefault_ContainsVariable(DatabaseDriver database)
    {
        var productName = "Anton";
        using var con = database.GetConnection();
        var product = con.FirstOrDefault<Product>(p => p.Name!.Contains(productName));
        Assert.NotNull(product);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task FirstOrDefaultAsync_ContainsVariable(DatabaseDriver database)
    {
        var productName = "Anton";
        using var con = database.GetConnection();
        var product = await con.FirstOrDefaultAsync<Product>(p => p.Name!.Contains(productName));
        Assert.NotNull(product);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void FirstOrDefault_StartsWith(DatabaseDriver database)
    {
        var productName = "Cha";
        using var con = database.GetConnection();
        var product = con.FirstOrDefault<Product>(p => p.Name!.StartsWith(productName));
        Assert.NotNull(product);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task FirstOrDefaultAsync_StartsWith(DatabaseDriver database)
    {
        var productName = "Cha";
        using var con = database.GetConnection();
        var product = await con.FirstOrDefaultAsync<Product>(p => p.Name!.StartsWith(productName));
        Assert.NotNull(product);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void FirstOrDefault_EndsWith(DatabaseDriver database)
    {
        var productName = "2";
        using var con = database.GetConnection();
        var product = con.FirstOrDefault<Product>(p => p.Name!.EndsWith(productName));
        Assert.NotNull(product);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task FirstOrDefaultAsync_EndsWith(DatabaseDriver database)
    {
        var productName = "2";
        using var con = database.GetConnection();
        var product = await con.FirstOrDefaultAsync<Product>(p => p.Name!.EndsWith(productName));
        Assert.NotNull(product);
    }
}
