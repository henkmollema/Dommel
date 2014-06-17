using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Dapper.FluentMap;
using Dapper.FluentMap.Mapping;

namespace Dommel.ConsoleTest
{
    public class ProductMap : EntityMap<Product>
    {
        public ProductMap()
        {
            //Map(p => p.Id).ToColumn("autID");
            Map(p => p.Name).ToColumn("strName");
        }
    }

    public class CustomTableNameResolver : SqlMapperExtensions.ITableNameResolver
    {
        public string ResolveTableName(Type type)
        {
            return string.Format("tbl{0}", type.Name);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var con = new SqlConnection("Data Source=sql2012;Initial Catalog=DapperTest;Integrated Security=True"))
            {
                SqlMapperExtensions.SetTableNameResolver(new CustomTableNameResolver());
                FluentMapper.Intialize(c => c.AddMap(new ProductMap()));


                for (int i = 1; i <= 7; i++)
                {
                    var sw = Stopwatch.StartNew();
                    var p = con.Get<Product>(i);
                    sw.Stop();

                    Console.WriteLine("{0}: {1}", p.Id, p.Name);
                    Console.WriteLine("Query executed in {0}ms", sw.Elapsed.TotalMilliseconds);
                    Console.WriteLine("");
                }
            }

            Console.ReadKey();
        }
    }

    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
