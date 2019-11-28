using System;
using System.Data.Common;
using System.Threading.Tasks;
using Dommel.IntegrationTests;
using Xunit;

namespace Dommel.Json.IntegrationTests
{
    [Collection("JSON Database")]
    public class SelectTests
    {
        private object InsertLead(DbConnection con)
        {
            return con.Insert(new Lead
            {
                Email = "foo@example.com",
                Data = new LeadData
                {
                    FirstName = "Foo",
                    LastName = "Bar",
                    Birthdate = new DateTime(1985, 7, 1),
                    Email = "foo@example.com",
                }
            });
        }

        private async Task<object> InsertLeadAsync(DbConnection con)
        {
            return await con.InsertAsync(new Lead
            {
                Email = "foo@example.com",
                Data = new LeadData
                {
                    FirstName = "Foo",
                    LastName = "Bar",
                    Birthdate = new DateTime(1985, 7, 1),
                    Email = "foo@example.com",
                }
            });
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public void SelectSingleStatement(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = InsertLead(con);
            var leads = con.Select<Lead>(p => p.Data.Email == "foo@example.com");
            Assert.NotEmpty(leads);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public async Task SelectSingleStatementAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = await InsertLeadAsync(con);
            var leads = await con.SelectAsync<Lead>(p => p.Data.Email == "foo@example.com");
            Assert.NotEmpty(leads);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public void SelectAndStatement(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = InsertLead(con);
            var leads = con.Select<Lead>(p => p.Data.FirstName == "Foo" && p.Data.LastName == "Bar" && p.Email == "foo@example.com");
            Assert.NotEmpty(leads);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public async Task SelectAndStatementAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = await InsertLeadAsync(con);
            var leads = await con.SelectAsync<Lead>(p => p.Data.FirstName == "Foo" && p.Data.LastName == "Bar" && p.Email == "foo@example.com");
            Assert.NotEmpty(leads);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public void SelectOrStatement(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = InsertLead(con);
            var leads = con.Select<Lead>(p => p.Data.FirstName == "Foo" && p.Data.LastName == "Bar" || p.Email == "foo@example.com");
            Assert.NotEmpty(leads);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public async Task SelectOrStatementAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = await InsertLeadAsync(con);
            var leads = await con.SelectAsync<Lead>(p => p.Data.FirstName == "Foo" && p.Data.LastName == "Bar" || p.Email == "foo@example.com");
            Assert.NotEmpty(leads);
        }
    }
}
