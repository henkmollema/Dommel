using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Dommel.IntegrationTests
{
    public class PostgresDatabase : Database
    {
        public override DbConnection GetConnection(string databaseName)
        {
            var connectionString = IsAppVeyor
                ? $"Server=localhost;Port=5432;Database={databaseName};Uid=postgres;Pwd=Password12!;"
                : $"Server=localhost;Port=5432;Database={databaseName};Uid=postgres;Pwd=root;";

            return new NpgsqlConnection(connectionString);
        }

        public override string TempDbDatabaseName => "postgres";

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public override async Task CreateDatabase()
        {
            DommelMapper.EscapeCharacterStart = DommelMapper.EscapeCharacterEnd = '"';
            using (var con = GetConnection(TempDbDatabaseName))
            {
                try
                {
                    // Always try to create the database as you'll run into
                    // race conditions when tests run in parallel.
                    await con.ExecuteAsync($"CREATE DATABASE {DefaultDatabaseName}");
                }
                catch (PostgresException pex) when (pex.SqlState == "42P04")
                {
                    // Ignore errors that the database already exists
                }
            }
        }

        public override async Task<bool> CreateTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                var sql = @"
SELECT * FROM information_schema.tables where table_name = 'Products' LIMIT 1;
CREATE TABLE IF NOT EXISTS ""Categories"" (""Id"" serial primary key, ""Name"" VARCHAR(255));
CREATE TABLE IF NOT EXISTS ""Products"" (""Id"" serial primary key, ""CategoryId"" int, ""Name"" VARCHAR(255));
CREATE TABLE IF NOT EXISTS ""Orders"" (""Id"" serial primary key, ""Created"" TIMESTAMP NOT NULL);
CREATE TABLE IF NOT EXISTS ""OrderLines"" (""Id"" serial primary key, ""OrderId"" int, ""Line"" VARCHAR(255));";
                var created = await con.ExecuteScalarAsync(sql);

                // No result means the tables were just created
                return created == null;
            }
        }

        public override async Task DropTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                await con.ExecuteAsync(@"
DROP TABLE ""Categories"";
DROP TABLE ""Products"";
DROP TABLE ""Orders"";
DROP TABLE ""OrderLines"";");
                con.Close();
            }

            DommelMapper.EscapeCharacterStart = DommelMapper.EscapeCharacterEnd = default;
        }
    }
}
