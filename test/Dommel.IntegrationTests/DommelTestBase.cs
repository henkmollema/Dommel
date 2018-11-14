using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Dommel.IntegrationTests
{
    [Collection("IntegrationTests")]
    public abstract class DommelTestBase : IAsyncLifetime
    {
        public DommelTestBase(Database database)
        {
            Database = database;
        }

        protected Database Database { get; }

        protected DbConnection GetConnection() => Database.GetConnection(Database.DefaultDatabaseName);

        public virtual async Task InitializeAsync()
        {
            using (var connection = Database.GetConnection(Database.TempDbDatabaseName))
            {
                await Database.CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                await connection.OpenAsync();
                var created = await Database.CreateTables();

                // Is the table created? If so, insert dummy data
                if (created)
                {
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

                        new Product { CategoryId = categoryId, Name = "Chai 3" },
                        new Product { CategoryId = categoryId, Name = "Chang 3" },
                        new Product { CategoryId = categoryId, Name = "Aniseed Syrup 3" },
                    };

                    await connection.InsertAsyncAll(products);

                    var orderId = Convert.ToInt32(await connection.InsertAsync(new Order()));
                    var orderLines = new List<OrderLine>
                    {
                        new OrderLine { OrderId = orderId, Line = "Line 1"},
                        new OrderLine { OrderId = orderId, Line = "Line 2"},
                        new OrderLine { OrderId = orderId, Line = "Line 3"},
                    };
                    await connection.InsertAsyncAll(orderLines);
                }
            }
        }

        public virtual async Task DisposeAsync()
        {
            await Database.DropTables();
            DommelMapper.ClearQueryCache();
        }
    }
}
