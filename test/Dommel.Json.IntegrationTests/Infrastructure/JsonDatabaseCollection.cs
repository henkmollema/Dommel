using Xunit;

namespace Dommel.Json.IntegrationTests
{
    // Apply the text fixture to all tests in the "Database" collection
    [CollectionDefinition("JSON Database")]
    public class JsonDatabaseCollection : ICollectionFixture<JsonDatabaseFixture>
    {
    }
}
