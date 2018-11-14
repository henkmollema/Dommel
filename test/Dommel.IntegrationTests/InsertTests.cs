using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class InsertTestsSqlServer : InsertTests, IClassFixture<SqlServerDatabase>
    {
        public InsertTestsSqlServer(SqlServerDatabase database) : base(database)
        {
        }
    }

    public class InsertTestsMySql : InsertTests, IClassFixture<MySqlDatabase>
    {
        public InsertTestsMySql(MySqlDatabase database) : base(database)
        {
        }
    }

    public abstract class InsertTests : DommelTestBase
    {
        public InsertTests(Database database) : base(database)
        {
        }

        [Fact]
        public void Insert()
        {
            using (var con = GetConnection())
            {
                var id = Convert.ToInt32(con.Insert(new Product { Name = "blah" }));
                var product = con.Get<Product>(id);
                Assert.NotNull(product);
                Assert.Equal("blah", product.Name);
                Assert.Equal(id, product.Id);
            }
        }

        [Fact]
        public async Task InsertAsync()
        {
            using (var con = GetConnection())
            {
                var id = Convert.ToInt32(await con.InsertAsync(new Product { Name = "blah" }));
                var product = await con.GetAsync<Product>(id);
                Assert.NotNull(product);
                Assert.Equal("blah", product.Name);
                Assert.Equal(id, product.Id);
            }
        }

        [Fact]
        public void InsertAll()
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

                var blahs = con.Select<Product>(p => p.Name == "blah");
                Assert.Equal(3, blahs.Count());
            }
        }

        [Fact]
        public async Task InsertAllAsync()
        {
            using (var con = GetConnection())
            {
                var ps = new List<Product>
                {
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                    new Product { Name = "blah"},
                };

                await con.InsertAsyncAll(ps);

                var blahs = await con.SelectAsync<Product>(p => p.Name == "blah");
                Assert.Equal(3, blahs.Count());
            }
        }

        [Fact]
        public void InsertAllEmtyList()
        {
            using (var con = GetConnection())
            {
                var ps = new List<Product>();
                con.InsertAll(ps);
            }
        }

        [Fact]
        public async Task InsertAllAsyncEmtyList()
        {
            using (var con = GetConnection())
            {
                var ps = new List<Product>();
                await con.InsertAsyncAll(ps);
            }
        }
    }
}
