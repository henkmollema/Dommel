using System.Threading.Tasks;
using Dapper;
using Dommel.IntegrationTests;

namespace Dommel.Json.IntegrationTests
{
    public class JsonPostgresDatabaseDriver : PostgresDatabaseDriver
    {
        protected override async Task<bool> CreateTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                var sql = @"
create table if not exists ""Leads"" (
    ""Id"" serial primary key, 
    ""DateCreated"" timestamp, 
    ""Email"" varchar(255), 
    ""Data"" json,
    ""Metadata"" json
);";
                await con.ExecuteScalarAsync(sql);
            }

            return await base.CreateTables();
        }

        protected override async Task DropTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                await con.ExecuteScalarAsync(@"drop table ""Leads""");
            }
            await base.DropTables();
        }
    }
}
