using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly DatabaseDriver[] _databases;

        public DatabaseFixture()
        {
            // Extract the database drivers from the test data
            _databases = new DatabaseTestData()
                .Select(x => x[0])
                .OfType<DatabaseDriver>()
                .OfType<DatabaseDriver>()
                .ToArray();

            if (_databases.Length == 0)
            {
                throw new InvalidOperationException($"No databases defined in {nameof(DatabaseTestData)} theory data.");
            }
        }

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
}
