using System;
using System.Data;
using Dapper;
using Dommel.IntegrationTests;
using Npgsql;
using Xunit;

namespace Dommel.Json.IntegrationTests;

public class JsonDatabaseFixture : DatabaseFixtureBase
{
    public JsonDatabaseFixture()
    {
        DommelJsonMapper.AddJson(new DommelJsonOptions
        {
            EntityAssemblies = new[]
            {
                    typeof(JsonDatabaseFixture).Assembly,
                    typeof(DatabaseFixture).Assembly
                },
            JsonTypeHandler = () => new NpgJsonObjectTypeHandler(),
        });
    }

    private class NpgJsonObjectTypeHandler : SqlMapper.ITypeHandler
    {
        private readonly JsonObjectTypeHandler _defaultTypeHandler = new JsonObjectTypeHandler();

        public void SetValue(IDbDataParameter parameter, object value)
        {
            // Use the default handler
            _defaultTypeHandler.SetValue(parameter, value);

            // Set the special NpgsqlDbType to use the JSON data type
            if (parameter is NpgsqlParameter npgsqlParameter)
            {
                npgsqlParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json;
            }
        }

        public object? Parse(Type destinationType, object value) =>
            _defaultTypeHandler.Parse(destinationType, value);
    }

    protected override TheoryData<DatabaseDriver> Drivers => new JsonDatabaseTestData();
}
