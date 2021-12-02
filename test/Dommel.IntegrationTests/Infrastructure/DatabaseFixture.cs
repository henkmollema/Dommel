using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

public class DatabaseFixture : DatabaseFixtureBase
{
    protected override TheoryData<DatabaseDriver> Drivers => new DatabaseTestData();
}

public abstract class DatabaseFixtureBase : IAsyncLifetime
{
    private readonly DatabaseDriver[] _databases;

    public DatabaseFixtureBase()
    {
        // Extract the database drivers from the test data
        _databases = Drivers
            .Select(x => x[0])
            .OfType<DatabaseDriver>()
            .ToArray();

        if (_databases.Length == 0)
        {
            throw new InvalidOperationException($"No databases defined in {nameof(DatabaseTestData)} theory data.");
        }
    }

    protected abstract TheoryData<DatabaseDriver> Drivers { get; }

    public async Task InitializeAsync()
    {
        foreach (var database in _databases)
        {
            await database.InitializeAsync();
        }
    }

    public async Task DisposeAsync()
    {
        foreach (var database in _databases)
        {
            await database.DisposeAsync();
        }
    }
}
