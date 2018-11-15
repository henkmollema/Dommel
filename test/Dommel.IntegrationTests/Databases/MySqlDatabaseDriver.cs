using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Dommel.IntegrationTests
{
    public class MySqlDatabaseDriver : DatabaseDriver
    {
        public override DbConnection GetConnection(string databaseName)
        {
            var connectionString = IsAppVeyor
                ? $"Server=localhost;Database={databaseName};Uid=root;Pwd=Password12!;"
                : $"Server=localhost;Database={databaseName};Uid=dommeltest;Pwd=test;";

            return new MySqlConnection(connectionString);
        }

        public override string TempDbDatabaseName => "mysql";

        protected override async Task CreateDatabase()
        {
            using (var con = GetConnection(TempDbDatabaseName))
            {
                await con.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS {DefaultDatabaseName}");
            }
        }

        protected override async Task<bool> CreateTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                var sql = @"
SELECT * FROM information_schema.tables where table_name = 'Products' LIMIT 1;
CREATE TABLE IF NOT EXISTS Categories (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255));
CREATE TABLE IF NOT EXISTS Products (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, CategoryId int, Name VARCHAR(255));
CREATE TABLE IF NOT EXISTS Orders (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, Created DATETIME NOT NULL);
CREATE TABLE IF NOT EXISTS OrderLines (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, OrderId int, Line VARCHAR(255));
CREATE TABLE IF NOT EXISTS Foos (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255));
CREATE TABLE IF NOT EXISTS Bars (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255));";
                var created = await con.ExecuteScalarAsync(sql);

                // No result means the tables were just created
                return created == null;
            }
        }
    }
}
