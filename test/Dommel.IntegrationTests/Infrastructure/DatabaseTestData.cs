using Xunit;

namespace Dommel.IntegrationTests
{
    public class DatabaseTestData : TheoryData<DatabaseDriver>
    {
        public DatabaseTestData()
        {
            // Defines the database providers to use for each test method.
            // These providers are used to initialize the databases in the
            // DatabaseFixture as well.
            Add(new SqlServerDatabaseDriver());
            Add(new MySqlDatabaseDriver());
            Add(new PostgresDatabaseDriver());
        }
    }
}
