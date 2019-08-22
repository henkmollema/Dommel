using System;
using System.Data;
using Dapper;
using Dommel.IntegrationTests;
using Newtonsoft.Json;
using Npgsql;
using Xunit;

namespace Dommel.Json.IntegrationTests
{
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
                JsonTypeHandler = () => new JsonObjectTypeHandler(),
            });
        }

        private class JsonObjectTypeHandler : SqlMapper.ITypeHandler
        {
            public void SetValue(IDbDataParameter parameter, object value)
            {
                parameter.Value = value is null || value is DBNull
                    ? (object)DBNull.Value
                    : JsonConvert.SerializeObject(value);
                parameter.DbType = DbType.String;

                if (parameter is NpgsqlParameter npgsqlParameter)
                {
                    npgsqlParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Json;
                }
            }

            public object Parse(Type destinationType, object value) =>
                value is string str ? JsonConvert.DeserializeObject(str, destinationType) : null;
        }

        protected override TheoryData<DatabaseDriver> Drivers => new JsonDatabaseTestData();
    }
}
