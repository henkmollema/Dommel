using System.Threading.Tasks;
using Dapper;
using Dommel.IntegrationTests;

namespace Dommel.Json.IntegrationTests
{
    public class JsonSqlServerDatabaseDriver : SqlServerDatabaseDriver
    {
        protected override async Task<bool> CreateTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                var sql = @"
CREATE TABLE Leads (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DateCreated DATETIME,
    Email VARCHAR(255),
    Data NVARCHAR(MAX),
    Metadata NVARCHAR(MAX)
);";
                await con.ExecuteScalarAsync(sql);
            }

            return await base.CreateTables();
        }

        protected override async Task DropTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                await con.ExecuteScalarAsync("DROP TABLE Leads");
            }
            await base.DropTables();
        }
    }
}
