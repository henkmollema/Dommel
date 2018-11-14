using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Dommel.IntegrationTests
{
    public abstract class Database
    {
        protected static readonly bool IsAppVeyor = bool.TryParse(Environment.GetEnvironmentVariable("AppVeyor"), out var appVeyor) ? appVeyor : false;

        public virtual string TempDbDatabaseName => "tempdb";

        public virtual string DefaultDatabaseName => "DommelTests";

        public abstract DbConnection GetConnection(string databaseName);

        public abstract Task CreateDatabase();

        public abstract Task<bool> CreateTables();

        public virtual async Task DropTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                await con.ExecuteAsync(@"
DROP TABLE Categories;
DROP TABLE Products;
DROP TABLE Orders;
DROP TABLE OrderLines;");
            }
        }
    }

    public class SqlServerDatabase : Database
    {
        public override DbConnection GetConnection(string databaseName)
        {
            var connectionString = IsAppVeyor
                ? $"Server=(local)\\SQL2016;Database={databaseName};User ID=sa;Password=Password12!"
                : $"Server=(LocalDb)\\mssqllocaldb;Database={databaseName};Integrated Security=True";

            return new SqlConnection(connectionString);
        }

        public override async Task CreateDatabase()
        {
            using (var con = GetConnection(TempDbDatabaseName))
            {
                await con.ExecuteAsync("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'DommelTests') BEGIN CREATE DATABASE DommelTests; END;");
            }
        }

        public override async Task<bool> CreateTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                var sql = @"IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories (Id int IDENTITY(1,1) PRIMARY KEY, Name VARCHAR(255));
    CREATE TABLE dbo.Products (Id int IDENTITY(1,1) PRIMARY KEY, CategoryId int, Name VARCHAR(255));
    CREATE TABLE dbo.Orders (Id int IDENTITY(1,1) PRIMARY KEY, Created DATETIME NOT NULL);
    CREATE TABLE dbo.OrderLines (Id int IDENTITY(1,1) PRIMARY KEY, OrderId int, Line VARCHAR(255));
    SELECT 1;
END";
                var created = await con.ExecuteScalarAsync(sql);

                // A result means the tables were just created
                return created != null;
            }
        }
    }

    public class MySqlDatabase : Database
    {
        public override DbConnection GetConnection(string databaseName)
        {
            var connectionString = IsAppVeyor
                ? $"Server=localhost;Database={databaseName};Uid=root;Pwd=Password12!;"
                : $"Server=localhost;Database={databaseName};Uid=dommeltest;Pwd=test;";

            return new MySqlConnection(connectionString);
        }

        public override string TempDbDatabaseName => "mysql";

        public override async Task CreateDatabase()
        {
            using (var con = GetConnection(TempDbDatabaseName))
            {
                await con.ExecuteAsync("CREATE DATABASE IF NOT EXISTS DommelTests");
            }
        }

        public override async Task<bool> CreateTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                var sql = @"
SELECT * FROM information_schema.tables where table_name = 'Products' LIMIT 1;
CREATE TABLE IF NOT EXISTS Categories (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255));
CREATE TABLE IF NOT EXISTS Products (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, CategoryId int, Name VARCHAR(255));
CREATE TABLE IF NOT EXISTS Orders (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, Created DATETIME NOT NULL);
CREATE TABLE IF NOT EXISTS OrderLines (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY, OrderId int, Line VARCHAR(255));";
                var created = await con.ExecuteScalarAsync(sql);

                // No result means the tables were just created
                return created == null;
            }
        }
    }
}
