using Xunit;

namespace Dommel.IntegrationTests
{
    public class DatabaseTestData : TheoryData<Database>
    {
        public DatabaseTestData()
        {
            // Defines the database providers to use for each test method.
            // These providers are used to initialize the databases in the
            // DatabaseFixture as well.
            Add(new SqlServerDatabase());
            Add(new MySqlDatabase());
            Add(new PostgresDatabase());
        }
    }
}
