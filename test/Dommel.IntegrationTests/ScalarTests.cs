using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class ScalarTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void ScalarSync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var sqlBuilder = DommelMapper.GetSqlBuilder(con);
        var columnName = sqlBuilder.QuoteIdentifier("ProductId");
        var maxId = con.Scalar<Product, int>(sql => sql.Select($"max({columnName})"));
        Assert.True(maxId > 0);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ScalarAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var sqlBuilder = DommelMapper.GetSqlBuilder(con);
        var columnName = sqlBuilder.QuoteIdentifier("ProductId");
        var maxId = await con.ScalarAsync<Product, int>(sql => sql.Select($"max({columnName})"));
        Assert.True(maxId > 0);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ScalarAsync_String(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var lastSlug = await con.ScalarAsync<Product, string>(
            sql => sql.Select(x => new { x.Name }).OrderByDescending(x => x.ProductId).Page(1, 1));
        Assert.False(string.IsNullOrEmpty(lastSlug));
    }
}
