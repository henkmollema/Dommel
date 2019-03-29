using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class FromTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Sample(DatabaseDriver database)
        {
            using (var con = database.GetConnection())
            {
                _ = con.GetAll<Product>();
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SampleAsync(DatabaseDriver database)
        {
            using (var con = database.GetConnection())
            {
                var products = await con.FromAsync<Product>(
                    sql => sql.Select(p => new { p.Name, p.CategoryId })
                        .Where(p => p.Name.StartsWith("Chai"))
                        .OrderBy(p => p.CategoryId));

                Assert.Equal(2, products.Count());
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task SampleAsync2(DatabaseDriver database)
        {
            using (var con = database.GetConnection())
            {
                var products = await con.FromAsync<Product>(sql => 
                    sql.Select(p => new { p.Name, p.CategoryId })
                        .Where(p => p.Name.StartsWith("Chai") && p.CategoryId == 1)
                        .OrWhere(p => p.Name != null)
                        .OrderBy(p => p.CategoryId)
                        .ThenByDescending(p => p.Name));

                Assert.NotEmpty(products);
            }
        }
    }
}
