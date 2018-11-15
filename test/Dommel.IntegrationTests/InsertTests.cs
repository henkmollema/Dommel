using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class InsertTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Insert(Database database)
        {
            using (var con = database.GetConnection())
            {
                var id = Convert.ToInt32(con.Insert(new Product { Name = "blah" }));
                var product = con.Get<Product>(id);
                Assert.NotNull(product);
                Assert.Equal("blah", product.Name);
                Assert.Equal(id, product.Id);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task InsertAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var id = Convert.ToInt32(await con.InsertAsync(new Product { Name = "blah" }));
                var product = await con.GetAsync<Product>(id);
                Assert.NotNull(product);
                Assert.Equal("blah", product.Name);
                Assert.Equal(id, product.Id);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void InsertAll(Database database)
        {
            using (var con = database.GetConnection())
            {
                var ps = new List<Foo>
                {
                    new Foo { Name = "blah" },
                    new Foo { Name = "blah" },
                    new Foo { Name = "blah" },
                };

                con.InsertAll(ps);

                var blahs = con.Select<Foo>(p => p.Name == "blah");
                Assert.Equal(3, blahs.Count());
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task InsertAllAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var ps = new List<Bar>
                {
                    new Bar { Name = "blah" },
                    new Bar { Name = "blah" },
                    new Bar { Name = "blah" },
                };

                await con.InsertAllAsync(ps);

                var blahs = await con.SelectAsync<Bar>(p => p.Name == "blah");
                Assert.Equal(3, blahs.Count());
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void InsertAllEmtyList(Database database)
        {
            using (var con = database.GetConnection())
            {
                var ps = new List<Product>();
                con.InsertAll(ps);
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task InsertAllAsyncEmtyList(Database database)
        {
            using (var con = database.GetConnection())
            {
                var ps = new List<Product>();
                await con.InsertAllAsync(ps);
            }
        }
    }
}
