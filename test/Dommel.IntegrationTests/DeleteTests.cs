using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class DeleteTests
    {
        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void Delete(Database database)
        {
            using (var con = database.GetConnection())
            {
                var id = Convert.ToInt32(con.Insert(new Product { Name = "blah" }));
                var product = con.Get<Product>(id);
                Assert.NotNull(product);
                Assert.Equal("blah", product.Name);
                Assert.Equal(id, product.Id);

                con.Delete(product);
                Assert.Null(con.Get<Product>(id));
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task DeleteAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var id = Convert.ToInt32(await con.InsertAsync(new Product { Name = "blah" }));
                var product = await con.GetAsync<Product>(id);
                Assert.NotNull(product);
                Assert.Equal("blah", product.Name);
                Assert.Equal(id, product.Id);

                await con.DeleteAsync(product);
                Assert.Null(await con.GetAsync<Product>(id));
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void DeleteAll(Database database)
        {
            using (var con = database.GetConnection())
            {
                Assert.True(con.DeleteAll<Foo>());
                Assert.Empty(con.GetAll<Foo>());
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task DeleteAllAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                Assert.True(await con.DeleteAllAsync<Bar>());
                Assert.Empty(await con.GetAllAsync<Bar>());
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void DeleteMultiple(Database database)
        {
            using (var con = database.GetConnection())
            {
                var ps = new List<Product>
                {
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                };

                con.InsertAll(ps);

                Assert.Equal(3, con.Select<Product>(p => p.Name == "blah").Count());

                con.DeleteMultiple<Product>(p => p.Name == "blah");
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task DeleteMultipleAsync(Database database)
        {
            using (var con = database.GetConnection())
            {
                var ps = new List<Product>
                {
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                };

                await con.InsertAllAsync(ps);

                Assert.Equal(3, (await con.SelectAsync<Product>(p => p.Name == "blah")).Count());

                con.DeleteMultiple<Product>(p => p.Name == "blah");
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public void DeleteMultipleLike(Database database)
        {
            using (var con = database.GetConnection())
            {
                var ps = new List<Product>
                {
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                };

                con.InsertAll(ps);

                Assert.Equal(3, con.Select<Product>(p => p.Name == "blah").Count());

                con.DeleteMultiple<Product>(p => p.Name.Contains("bla"));
            }
        }

        [Theory]
        [ClassData(typeof(DatabaseTestData))]
        public async Task DeleteMultipleAsyncLike(Database database)
        {
            using (var con = database.GetConnection())
            {
                var ps = new List<Product>
                {
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                };

                await con.InsertAllAsync(ps);

                Assert.Equal(3, (await con.SelectAsync<Product>(p => p.Name == "blah")).Count());

                con.DeleteMultiple<Product>(p => p.Name.Contains("bla"));
            }
        }
    }
}
