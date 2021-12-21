using Dommel.IntegrationTests;
using Xunit;

namespace Dommel.Json.IntegrationTests;

public class JsonDatabaseTestData : TheoryData<DatabaseDriver>
{
    public JsonDatabaseTestData()
    {
        // Defines the database providers to use for each test method.
        // These providers are used to initialize the databases in the
        // DatabaseFixture as well.
        if (!CI.IsTravis)
        {
            Add(new JsonSqlServerDatabaseDriver());
        }

        Add(new JsonMySqlDatabaseDriver());
        Add(new JsonPostgresDatabaseDriver());
    }
}
