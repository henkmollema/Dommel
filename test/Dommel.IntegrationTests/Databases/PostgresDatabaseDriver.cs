using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Dommel.IntegrationTests;

public class PostgresDatabaseDriver : DatabaseDriver
{
    public override DbConnection GetConnection(string databaseName)
    {
        var connectionString = $"Server=localhost;Port=5432;Database={databaseName};Uid=postgres;Pwd=postgres;";
        if (CI.IsAppVeyor)
        {
            connectionString = $"Server=localhost;Port=5432;Database={databaseName};Uid=postgres;Pwd=Password12!;";
        }
        else if (CI.IsTravis)
        {
            connectionString = $"Server=localhost;Port=5432;Database={databaseName};Uid=postgres;Pwd=;";
        }

        return new NpgsqlConnection(connectionString);
    }

    public override string TempDbDatabaseName => "postgres";

    protected override async Task CreateDatabase()
    {
        using var con = GetConnection(TempDbDatabaseName);
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

    protected override async Task<bool> CreateTables()
    {
        using var con = GetConnection(DefaultDatabaseName);
        var sql = @"
SELECT * FROM information_schema.tables WHERE table_name = 'Products' LIMIT 1;
CREATE TABLE IF NOT EXISTS ""Categories"" (""CategoryId"" SERIAL PRIMARY KEY, ""Name"" VARCHAR(255));
CREATE TABLE IF NOT EXISTS ""Products"" (""ProductId"" SERIAL PRIMARY KEY, ""CategoryId"" INT, ""FullName"" VARCHAR(255), ""Slug"" VARCHAR(255));
CREATE TABLE IF NOT EXISTS ""ProductOptions"" (""Id"" SERIAL PRIMARY KEY, ""ProductId"" INT);
CREATE TABLE IF NOT EXISTS ""ProductsCategories"" (""ProductId"" INT, ""CategoryId"" INT, PRIMARY KEY (""ProductId"", ""CategoryId""));
CREATE TABLE IF NOT EXISTS ""Orders"" (""Id"" SERIAL PRIMARY KEY, ""Created"" TIMESTAMP NOT NULL);
CREATE TABLE IF NOT EXISTS ""OrderLines"" (""Id"" SERIAL PRIMARY KEY, ""OrderId"" INT, ""Line"" VARCHAR(255));
CREATE TABLE IF NOT EXISTS ""Foos"" (""Id"" SERIAL PRIMARY KEY, ""Name"" VARCHAR(255));
CREATE TABLE IF NOT EXISTS ""Bars"" (""Id"" SERIAL PRIMARY KEY, ""Name"" VARCHAR(255));
CREATE TABLE IF NOT EXISTS ""Bazs"" (""BazId"" UUID primary key, ""Name"" VARCHAR(255));
CREATE TABLE IF NOT EXISTS ""Plufs"" (""PlufId"" INT PRIMARY KEY, ""Name"" VARCHAR(255));";
        var created = await con.ExecuteScalarAsync(sql);

        // No result means the tables were just created
        return created == null;
    }
}