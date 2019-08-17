using System.Threading.Tasks;
using Dapper;
using Dommel.IntegrationTests;

namespace Dommel.Json.IntegrationTests
{
    public class JsonMySqlDatabaseDriver : MySqlDatabaseDriver
    {
        protected override async Task<bool> CreateTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                var sql = @"
CREATE TABLE IF NOT EXISTS `Leads` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `DateCreated` DATETIME,
    `Email` VARCHAR(255),
    `Data` LONGTEXT,
    `Metadata` LONGTEXT
);";
                await con.ExecuteScalarAsync(sql);
            }

            return await base.CreateTables();
        }

        protected override async Task DropTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                await con.ExecuteScalarAsync("DROP TABLE `Leads`");
            }
            await base.DropTables();
        }
    }
}
