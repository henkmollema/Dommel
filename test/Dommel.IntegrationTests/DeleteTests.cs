using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class DeleteTestsSqlServer : DeleteTests, IClassFixture<SqlServerDatabase>
    {
        public DeleteTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class DeleteTestsMySql : DeleteTests, IClassFixture<MySqlDatabase>
    {
        public DeleteTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class DeleteTests : DommelTestBase
    {
        public DeleteTests(Database database) : base(database)
        {
        }

        [Fact]
        public void Delete()
        {
            using (var con = GetConnection())
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

        [Fact]
        public async Task DeleteAsync()
        {
            using (var con = GetConnection())
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

        [Fact]
        public void DeleteAll()
        {
            using (var con = GetConnection())
            {
                Assert.True(con.DeleteAll<Product>());
                Assert.Empty(con.GetAll<Product>());
            }
        }

        [Fact]
        public async Task DeleteAllAsync()
        {
            using (var con = GetConnection())
            {
                Assert.True(await con.DeleteAllAsync<Product>());
                Assert.Empty(await con.GetAllAsync<Product>());
            }
        }

        [Fact]
        public void DeleteMultiple()
        {
            using (var con = GetConnection())
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

        [Fact]
        public async Task DeleteMultipleAsync()
        {
            using (var con = GetConnection())
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

        [Fact]
        public void DeleteMultipleLike()
        {
            using (var con = GetConnection())
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

        [Fact]
        public async Task DeleteMultipleAsyncLike()
        {
            using (var con = GetConnection())
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
