using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class InsertOutputParameterTests
    {
        public class Qux
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string? Name { get; set; }
        }

        public class GuidSqlServerSqlBuilder : SqlServerSqlBuilder
        {
            public override string BuildInsert(Type type, string tableName, string[] columnNames, string[] paramNames) =>
                $"set nocount on insert into {tableName} ({string.Join(", ", columnNames)}) output inserted.Id values ({string.Join(", ", paramNames)})";
        }

        [Fact]
        public async Task InsertGuidPrimaryKey()
        {
            if (CI.IsTravis)
            {
                // Don't run SQL Server test on Linux
                return;
            }

            using var con = new SqlServerDatabaseDriver().GetConnection();
            await con.ExecuteAsync("CREATE TABLE dbo.Quxs (Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), Name VARCHAR(255));");
            try
            {
                object identity;
                try
                {
                    DommelMapper.AddSqlBuilder(typeof(SqlConnection), new GuidSqlServerSqlBuilder());
                    identity = await con.InsertAsync(new Qux { Name = "blah" });
                }
                finally
                {
                    DommelMapper.AddSqlBuilder(typeof(SqlConnection), new SqlServerSqlBuilder());
                }

                Assert.NotNull(identity);
                var id = Assert.IsType<Guid>(identity);
                var baz = await con.GetAsync<Qux>(id);
                Assert.NotNull(baz);
                Assert.Equal("blah", baz.Name);
                Assert.Equal(id, baz.Id);
            }
            finally
            {
                await con.ExecuteAsync("DROP TABLE dbo.Quxs");
            }
        }
    }
}
