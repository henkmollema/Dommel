using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class AnyTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Any(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        Assert.True(con.Any<Product>());
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task AnyAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        Assert.True(await con.AnyAsync<Product>());
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void AnyWithPredicate(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        Assert.True(con.Any<Product>(x => x.Name != null));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task AnyAsyncWithPredicate(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        Assert.True(await con.AnyAsync<Product>(x => x.Name != null));
    }
}
