using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;

namespace Dommel.IntegrationTests;

public class MySqlDatabaseDriver : DatabaseDriver
{
    public override DbConnection GetConnection(string databaseName)
    {
        var connectionString = $"Server=localhost;Database={databaseName};Uid=dommeltest;Pwd=test;";
        if (CI.IsAppVeyor)
        {
            connectionString = $"Server=localhost;Database={databaseName};Uid=root;Pwd=Password12!;";
        }
        else if (CI.IsTravis)
        {
            connectionString = $"Server=localhost;Database={databaseName};Uid=root;Pwd=;";
        }

        return new MySqlConnection(connectionString);
    }

    public override string TempDbDatabaseName => "mysql";

    protected override async Task CreateDatabase()
    {
        using var con = GetConnection(TempDbDatabaseName);
        await con.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS {DefaultDatabaseName}");
    }

    protected override async Task<bool> CreateTables()
    {
        using var con = GetConnection(DefaultDatabaseName);
        var sql = @"
SELECT * FROM information_schema.tables where table_name = 'Products' LIMIT 1;
CREATE TABLE IF NOT EXISTS Categories (CategoryId INT AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255));
CREATE TABLE IF NOT EXISTS ProductsCategories (ProductId INT, CategoryId INT, PRIMARY KEY (ProductId, CategoryId));
CREATE TABLE IF NOT EXISTS Products (ProductId INT AUTO_INCREMENT PRIMARY KEY, CategoryId int, Name VARCHAR(255), Slug VARCHAR(255));
CREATE TABLE IF NOT EXISTS ProductOptions (Id INT AUTO_INCREMENT PRIMARY KEY, ProductId INT);
CREATE TABLE IF NOT EXISTS Orders (Id INT AUTO_INCREMENT PRIMARY KEY, Created DATETIME NOT NULL);
CREATE TABLE IF NOT EXISTS OrderLines (Id INT AUTO_INCREMENT PRIMARY KEY, OrderId int, Line VARCHAR(255));
CREATE TABLE IF NOT EXISTS Foos (Id INT AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255));
CREATE TABLE IF NOT EXISTS Bars (Id INT AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255));
CREATE TABLE IF NOT EXISTS Bazs (BazId CHAR(36) PRIMARY KEY, Name VARCHAR(255));";
        var created = await con.ExecuteScalarAsync(sql);

        // No result means the tables were just created
        return created == null;
    }
}
