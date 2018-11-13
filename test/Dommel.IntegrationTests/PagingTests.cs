using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Dommel.IntegrationTests
{
    [Collection("SQL")]
    public class PagingTests //: IAsyncLifetime
    {
        private static string GetConnectionString(string databaseName = "DommelTests") =>
            $"Server=(LocalDb)\\mssqllocaldb;Database={databaseName};Integrated Security=True";

        //private SqlConnection _connection;

        public async Task InitializeAsync()
        {
            using (var con = new SqlConnection(GetConnectionString("tempdb")))
            {
                await con.OpenAsync();
                await con.ExecuteAsync("drop database if exists [DommelTests]");
                await con.ExecuteAsync("create database [DommelTests]");
            }

            using (var connection = new SqlConnection(GetConnectionString()))
            {
                await connection.OpenAsync();

                await connection.ExecuteAsync("create table Products (Id int IDENTITY(1,1) PRIMARY KEY, Name varchar(255) not null)");
                await connection.InsertAsync(new Product { Name = "Chai" });
                await connection.InsertAsync(new Product { Name = "Chang" });
                await connection.InsertAsync(new Product { Name = "Aniseed Syrup" });
                await connection.InsertAsync(new Product { Name = "Chef Anton's Cajun Seasoning" });
                await connection.InsertAsync(new Product { Name = "Chef Anton's Gumbo Mix" });

                await connection.InsertAsync(new Product { Name = "Chai 2" });
                await connection.InsertAsync(new Product { Name = "Chang 2" });
                await connection.InsertAsync(new Product { Name = "Aniseed Syrup 2" });
                await connection.InsertAsync(new Product { Name = "Chef Anton's Cajun Seasoning 2" });
                await connection.InsertAsync(new Product { Name = "Chef Anton's Gumbo Mix 2" });

                await connection.InsertAsync(new Product { Name = "Chai 3" });
                await connection.InsertAsync(new Product { Name = "Chang 3" });
                await connection.InsertAsync(new Product { Name = "Aniseed Syrup 3" });
            }
        }

        public Task DisposeAsync()
        {
            //_connection?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Fetches_FirstPage()
        {
            await InitializeAsync();
            using (var con = new SqlConnection(GetConnectionString()))
            {
                var paged = (await con.GetPagedAsync<Product>(1, 5)).ToArray();
                Assert.Equal(5, paged.Length);
                Assert.Collection(paged,
                    p => Assert.Equal("Chai", p.Name),
                    p => Assert.Equal("Chang", p.Name),
                    p => Assert.Equal("Aniseed Syrup", p.Name),
                    p => Assert.Equal("Chef Anton's Cajun Seasoning", p.Name),
                    p => Assert.Equal("Chef Anton's Gumbo Mix", p.Name));
            }
        }

        [Fact]
        public async Task Fetches_SecondPage()
        {
            //await InitializeAsync();
            using (var con = new SqlConnection(GetConnectionString()))
            {
                var paged = (await con.GetPagedAsync<Product>(2, 5)).ToArray();
                Assert.Equal(5, paged.Length);
            }
        }

        [Fact]
        public async Task Fetches_ThirdPartialPage()
        {
            //await InitializeAsync();
            using (var con = new SqlConnection(GetConnectionString()))
            {
                var paged = (await con.GetPagedAsync<Product>(3, 5)).ToArray();
                Assert.Equal(3, paged.Length);
            }
        }

        [Fact]
        public async Task SelectPaged_FetchesFirstPage()
        {
            //await InitializeAsync();
            using (var con = new SqlConnection(GetConnectionString()))
            {
                var paged = (await con.SelectPagedAsync<Product>(p => p.Name == "Chai", 1, 5)).ToArray();
                Assert.Single(paged);
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
