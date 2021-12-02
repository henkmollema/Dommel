using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class CountTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Count(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        Assert.True(con.Count<Product>() > 0);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task CountAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        Assert.True(await con.CountAsync<Product>() > 0);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void CountWithPredicate(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        Assert.True(con.Count<Product>(x => x.Name != null) > 0);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task CountAsyncWithPredicate(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        Assert.True(await con.CountAsync<Product>(x => x.Name != null) > 0);
    }
}
