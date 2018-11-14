using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Xunit;

[assembly:CollectionBehavior(DisableTestParallelization = true)]

namespace Dommel.IntegrationTests
{
    [Collection("IntegrationTests")]
    public abstract class DommelTestBase : IAsyncLifetime
    {
        protected static readonly bool IsAppVeyor = bool.TryParse(Environment.GetEnvironmentVariable("AppVeyor"), out var appVeyor) ? appVeyor : false;

        protected virtual string GetConnectionString(string databaseName = "DommelTests") =>
            IsAppVeyor
                ? $"Server=(local)\\SQL2016;Database={databaseName};User ID=sa;Password=Password12!"
                : $"Server=(LocalDb)\\mssqllocaldb;Database={databaseName};Integrated Security=True";

        public virtual async Task InitializeAsync()
        {
            using (var con = new SqlConnection(GetConnectionString("tempdb")))
            {
                await con.ExecuteAsync("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'DommelTests') BEGIN CREATE DATABASE DommelTests; END;");
            }

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();
                var sql = "IF OBJECT_ID(N'dbo.Products', N'U') IS NULL BEGIN CREATE TABLE dbo.Products (Id int IDENTITY(1,1) PRIMARY KEY, Name varchar(255) not null); SELECT 1; END;";
                var created = await connection.ExecuteScalarAsync(sql);
                if (created != null)
                {
                    // Table was just created, insert dummy data
                    var products = new List<Product>
                    {
                        new Product { Name = "Chai" },
                        new Product { Name = "Chang" },
                        new Product { Name = "Aniseed Syrup" },
                        new Product { Name = "Chef Anton's Cajun Seasoning" },
                        new Product { Name = "Chef Anton's Gumbo Mix" },

                        new Product { Name = "Chai 2" },
                        new Product { Name = "Chang 2" },
                        new Product { Name = "Aniseed Syrup 2" },
                        new Product { Name = "Chef Anton's Cajun Seasoning 2" },
                        new Product { Name = "Chef Anton's Gumbo Mix 2" },

                        new Product { Name = "Chai 3" },
                        new Product { Name = "Chang 3" },
                        new Product { Name = "Aniseed Syrup 3" },
                    };

                    await connection.InsertAsyncAll(products);
                }
            }
        }

        public virtual async Task DisposeAsync()
        {
            using (var con = new SqlConnection(GetConnectionString()))
            {
                await con.ExecuteAsync("IF OBJECT_ID(N'dbo.Products', N'U') IS NOT NULL BEGIN DROP TABLE dbo.Products; END;");
            }
        }
    }
}
