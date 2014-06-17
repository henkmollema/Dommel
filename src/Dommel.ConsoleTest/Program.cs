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
            Map(p => p.Id).ToColumn("autID");
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

    public class CustomKeyPropertyResolver : SqlMapperExtensions.IKeyPropertyResolver
    {
        public PropertyInfo ResolveKeyProperty(Type type)
        {
            return null;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var con = new SqlConnection("Data Source=.\\sql2012;Initial Catalog=DapperTest;Integrated Security=True"))
            {
                SqlMapperExtensions.SetTableNameResolver(new CustomTableNameResolver());
                FluentMapper.Intialize(c => c.AddMap(new ProductMap()));


                var sw = Stopwatch.StartNew();
                var p = con.Get<Product>(1);
                sw.Stop();

                Console.WriteLine(p.Id);
                Console.WriteLine(p.Name);

                Console.WriteLine("Query executed in {0}ms", sw.Elapsed.TotalMilliseconds);

                Console.WriteLine("");

                var sw2 = Stopwatch.StartNew();
                var p2 = con.Get<Product>(2);
                sw2.Stop();

                Console.WriteLine(p2.Id);
                Console.WriteLine(p2.Name);

                Console.WriteLine("Query executed in {0}ms", sw2.Elapsed.TotalMilliseconds);

                Console.WriteLine("");

                var sw3 = Stopwatch.StartNew();
                var p3 = con.Get<Product>(3);
                sw3.Stop();

                Console.WriteLine(p3.Id);
                Console.WriteLine(p3.Name);

                Console.WriteLine("Query executed in {0}ms", sw3.Elapsed.TotalMilliseconds);
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
