using System;
using System.Data.SqlClient;
using System.Reflection;
using Dapper;
using Xunit;

namespace Dommel.IntegrationTests
{
    [Collection("Database")]
    public class InsertGuidTests
    {
        public class Baz
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string Name { get; set; }
        }

        public class GuidSqlServerSqlBuilder : DommelMapper.SqlServerSqlBuilder
        {
            public override string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty) =>
                $"set nocount on insert into {tableName} ({string.Join(", ", columnNames)}) output inserted.Id values ({string.Join(", ", paramNames)})";
        }

        [Fact]
        public void InsertGuidPrimaryKey()
        {
            using (var con = new SqlServerDatabaseDriver().GetConnection())
            {
                con.Execute("CREATE TABLE dbo.Bazs (Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(), Name VARCHAR(255));");

                object identity;
                try
                {
                    DommelMapper.AddSqlBuilder(typeof(SqlConnection), new GuidSqlServerSqlBuilder());
                    identity = con.Insert(new Baz { Name = "blah" });
                }
                finally
                {
                    DommelMapper.AddSqlBuilder(typeof(SqlConnection), new DommelMapper.SqlServerSqlBuilder());
                }

                Assert.NotNull(identity);
                var id = Assert.IsType<Guid>(identity);
                var baz = con.Get<Baz>(id);
                Assert.NotNull(baz);
                Assert.Equal("blah", baz.Name);
                Assert.Equal(id, baz.Id);
                con.Execute("DROP TABLE dbo.Bazs");
            }
        }
    }
}
