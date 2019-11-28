using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Dommel.IntegrationTests
{
    /// <summary>
    /// Provides a driver to interact with a specific database system.
    /// </summary>
    public abstract class DatabaseDriver
    {
        public abstract string TempDbDatabaseName { get; }

        public virtual string DefaultDatabaseName => "dommeltests";

        public abstract DbConnection GetConnection(string databaseName);

        public DbConnection GetConnection() => GetConnection(DefaultDatabaseName);

        public virtual async Task InitializeAsync()
        {
            await CreateDatabase();
            var created = await CreateTables();

            // Is the table created? If so, insert dummy data
            if (created)
            {
                using var connection = GetConnection();
                await connection.OpenAsync();

                var categoryId = Convert.ToInt32(await connection.InsertAsync(new Category { Name = "Food" }));

                var products = new List<Product>
                    {
                        new Product { CategoryId = categoryId, Name = "Chai" },
                        new Product { CategoryId = categoryId, Name = "Chang" },
                        new Product { CategoryId = categoryId, Name = "Aniseed Syrup" },
                        new Product { CategoryId = categoryId, Name = "Chef Anton's Cajun Seasoning" },
                        new Product { CategoryId = categoryId, Name = "Chef Anton's Gumbo Mix" },

                        new Product { CategoryId = categoryId, Name = "Chai 2" },
                        new Product { CategoryId = categoryId, Name = "Chang 2" },
                        new Product { CategoryId = categoryId, Name = "Aniseed Syrup 2" },
                        new Product { CategoryId = categoryId, Name = "Chef Anton's Cajun Seasoning 2" },
                        new Product { CategoryId = categoryId, Name = "Chef Anton's Gumbo Mix 2" },

                        new Product { Name = "Foo" }, // 11
                        new Product { Name = "Bar" }, // 12
                        new Product { Name = "Baz" }, // 13
                    };

                await connection.InsertAllAsync(products);

                // Order 1
                var orderId = Convert.ToInt32(await connection.InsertAsync(new Order()));
                var orderLines = new List<OrderLine>
                    {
                        new OrderLine { OrderId = orderId, Line = "Line 1"},
                        new OrderLine { OrderId = orderId, Line = "Line 2"},
                        new OrderLine { OrderId = orderId, Line = "Line 3"},
                    };
                await connection.InsertAllAsync(orderLines);

                // Order 2
                _ = await connection.InsertAsync(new Order());

                // Foo's and Bar's for delete queries
                await connection.InsertAllAsync(Enumerable.Range(0, 5).Select(_ => new Foo()));
                await connection.InsertAllAsync(Enumerable.Range(0, 5).Select(_ => new Bar()));
            }
        }

        protected abstract Task CreateDatabase();

        protected abstract Task<bool> CreateTables();

        protected virtual async Task DropTables()
        {
            using var con = GetConnection(DefaultDatabaseName);
            await con.ExecuteAsync(@"
DROP TABLE Categories;
DROP TABLE Products;
DROP TABLE Orders;
DROP TABLE OrderLines;
DROP TABLE Foos;
DROP TABLE Bars;
DROP TABLE Bazs;");
        }

        public virtual async Task DisposeAsync() => await DropTables();
    }
}
