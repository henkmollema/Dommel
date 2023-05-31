using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class GetAllTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void GetAll(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = con.GetAll<Product>();
        Assert.NotEmpty(products);
        Assert.All(products, p => Assert.NotEmpty(p.Name!));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task GetAllAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var products = await con.GetAllAsync<Product>();
        Assert.NotEmpty(products);
        Assert.All(products, p => Assert.NotEmpty(p.Name!));
    }
}
