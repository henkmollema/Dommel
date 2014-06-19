using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Dapper;
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

    //public class CustomTableNameResolver : SqlMapperExtensions.ITableNameResolver
    //{
    //    public string ResolveTableName(Type type)
    //    {
    //        return string.Format("tbl{0}", type.Name);
    //    }
    //}

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var con = new SqlConnection("Data Source=sql2012;Initial Catalog=DapperTest;Integrated Security=True"))
            {
                //SqlMapperExtensions.SetTableNameResolver(new CustomTableNameResolver());
                //FluentMapper.Intialize(c => c.AddMap(new ProductMap()));

                using (Profiler.Start())
                {
                    var p = con.Get<Product>(1);

                    //p.Name = string.Format("Product: {0:dd-MM HH:mm:ss}", DateTime.Now);
                    p.Created = DateTime.Now;

                    con.Update(p);
                }

                //for (int i = 0; i < 5; i++)
                //{
                //    using (Profiler.Start("Insert Dommel product"))
                //    {
                //        con.Insert(new Product { Name = "Dommel Product", NameUrlOptimized = "product", Description = "Nice stuff" });
                //    }
                //}

                //for (int i = 1; i <= 4; i++)
                //{
                //    var sw = Stopwatch.StartNew();
                //    var p = con.Get<Product>(i);
                //    sw.Stop();
                //
                //    Console.WriteLine("{0}: {1}", p.Id, p.Name);
                //    Console.WriteLine("Query executed in {0}ms", sw.Elapsed.TotalMilliseconds);
                //    Console.WriteLine("");
                //}
            }

            Console.ReadKey();
        }
    }

    public class Product
    {
        public int Id { get; set; }

        public DateTime? Created { get; set; }

        public string Name { get; set; }

        public string NameUrlOptimized { get; set; }

        public string Description { get; set; }
    }
}
