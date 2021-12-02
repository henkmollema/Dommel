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

                var categoryId1 = Convert.ToInt32(await connection.InsertAsync(new Category { Name = "Food" }));
                var categoryId2 = Convert.ToInt32(await connection.InsertAsync(new Category { Name = "Food 2" }));

                var products = new List<Product>
                {
                    new Product { CategoryId = categoryId1, Name = "Chai" },
                    new Product { CategoryId = categoryId1, Name = "Chang" },
                    new Product { CategoryId = categoryId1, Name = "Aniseed Syrup" },
                    new Product { CategoryId = categoryId1, Name = "Chef Anton's Cajun Seasoning" },
                    new Product { CategoryId = categoryId1, Name = "Chef Anton's Gumbo Mix" },

                    new Product { CategoryId = categoryId2, Name = "Chai 2" },
                    new Product { CategoryId = categoryId2, Name = "Chang 2" },
                    new Product { CategoryId = categoryId2, Name = "Aniseed Syrup 2" },
                    new Product { CategoryId = categoryId2, Name = "Chef Anton's Cajun Seasoning 2" },
                    new Product { CategoryId = categoryId2, Name = "Chef Anton's Gumbo Mix 2" },

                    new Product { Name = "Foo" }, // 11
                    new Product { Name = "Bar" }, // 12
                    new Product { Name = "Baz" }, // 13
                };

                await connection.InsertAllAsync(products);

                var productId = (await connection.FirstOrDefaultAsync<Product>(x => x.Name == "Chai"))!.ProductId;
                await connection.InsertAsync(new ProductOption { ProductId = productId });

                // Order 1
                var orderId = Convert.ToInt32(await connection.InsertAsync(new Order { Created = new DateTime(2011, 1, 1) }));
                var orderLines = new List<OrderLine>
                    {
                        new OrderLine { OrderId = orderId, Line = "Line 1"},
                        new OrderLine { OrderId = orderId, Line = "Line 2"},
                        new OrderLine { OrderId = orderId, Line = "Line 3"},
                    };
                await connection.InsertAllAsync(orderLines);

                // Order 2
                _ = await connection.InsertAsync(new Order { Created = new DateTime(2012, 2, 2) });

                // Foo's and Bar's for delete queries
                await connection.InsertAllAsync(Enumerable.Range(0, 5).Select(_ => new Foo()));
                await connection.InsertAllAsync(Enumerable.Range(0, 5).Select(_ => new Bar()));

                // Composite key entities
                await connection.InsertAsync(new ProductsCategories { ProductId = 1, CategoryId = 1 });
                await connection.InsertAsync(new ProductsCategories { ProductId = 1, CategoryId = 2 });
                await connection.InsertAsync(new ProductsCategories { ProductId = 3, CategoryId = 1 });
            }
        }

        protected abstract Task CreateDatabase();

        protected abstract Task<bool> CreateTables();

        protected virtual async Task DropTables()
        {
            using var con = GetConnection(DefaultDatabaseName);
            var sqlBuilder = DommelMapper.GetSqlBuilder(con);
            string Quote(string s) => sqlBuilder.QuoteIdentifier(s);

            await con.ExecuteAsync($@"
DROP TABLE {Quote("Categories")};
DROP TABLE {Quote("Products")};
DROP TABLE {Quote("ProductsCategories")};
DROP TABLE {Quote("ProductOptions")};
DROP TABLE {Quote("Orders")};
DROP TABLE {Quote("OrderLines")};
DROP TABLE {Quote("Foos")};
DROP TABLE {Quote("Bars")};
DROP TABLE {Quote("Bazs")};");
        }

        public virtual async Task DisposeAsync() => await DropTables();
    }
}
