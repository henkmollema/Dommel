using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests
{
    public class PagingTests
    {
        private static string GetConnectionString()
        {
            var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
            var fileName = Path.Combine(currentDir.Parent.Parent.Parent.FullName, "App_Data", "Dommel.mdf");
            var conStr = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;AttachDBFilename=" + fileName;
            return conStr;
        }

        [Fact]
        public void Fetches_FirstPage()
        {
            using (var con = new SqlConnection(GetConnectionString()))
            {
                var paged = con.GetPaged<Product>(1, 5).ToArray();
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
        public void Fetches_SecondPage()
        {
            using (var con = new SqlConnection(GetConnectionString()))
            {
                var paged = con.GetPaged<Product>(2, 5).ToArray();
                Assert.Equal(5, paged.Length);
            }
        }

        [Fact]
        public async Task Fetches_ThirdPartialPage()
        {
            using (var con = new SqlConnection(GetConnectionString()))
            {
                var paged = (await con.GetPagedAsync<Product>(3, 5)).ToArray();
                Assert.Equal(3, paged.Length);
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
