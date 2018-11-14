using System;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace Dommel.IntegrationTests
{
    public abstract class Database
    {
        protected static readonly bool IsAppVeyor = bool.TryParse(Environment.GetEnvironmentVariable("AppVeyor"), out var appVeyor) ? appVeyor : false;

        public virtual string TempDbDatabaseName => "tempdb";

        public virtual string DefaultDatabaseName => "dommeltests";

        public abstract DbConnection GetConnection(string databaseName);

        public abstract Task CreateDatabase();

        public abstract Task<bool> CreateTables();

        public virtual async Task DropTables()
        {
            using (var con = GetConnection(DefaultDatabaseName))
            {
                await con.ExecuteAsync(@"
DROP TABLE Categories;
DROP TABLE Products;
DROP TABLE Orders;
DROP TABLE OrderLines;");
            }
        }
    }
}
