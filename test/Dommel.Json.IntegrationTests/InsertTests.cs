using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dommel.IntegrationTests;
using Xunit;

namespace Dommel.Json.IntegrationTests
{
    [Collection("JSON Database")]
    public class InsertTests
    {
        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public void InsertsNullObject(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = con.Insert(new Lead
            {
                Email = "foo@example.com",
                Data = null!,
            });

            var lead = con.Get<Lead>(id);
            Assert.NotNull(lead);
            Assert.Null(lead.Data);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public async Task InsertsNullObjectAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = await con.InsertAsync(new Lead
            {
                Email = "foo@example.com",
                Data = null!,
            });

            var lead = await con.GetAsync<Lead>(id);
            Assert.NotNull(lead);
            Assert.Null(lead.Data);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public void InsertsEmptyJsonObject(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = con.Insert(new Lead
            {
                Email = "foo@example.com",
                Data = new LeadData(),
            });

            var lead = con.Get<Lead>(id);
            Assert.NotNull(lead);
            Assert.NotNull(lead.Data);
            Assert.Null(lead.Data?.FirstName);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public async Task InsertsEmptyJsonObjectAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = await con.InsertAsync(new Lead
            {
                Email = "foo@example.com",
                Data = new LeadData(),
            });

            var lead = await con.GetAsync<Lead>(id);
            Assert.NotNull(lead);
            Assert.NotNull(lead.Data);
            Assert.Null(lead.Data?.FirstName);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public void InsertsJsonObject(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = con.Insert(new Lead
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

            var lead = con.Get<Lead>(id);
            Assert.NotNull(lead);
            Assert.NotNull(lead.Data);
            Assert.NotNull(lead.Data?.FirstName);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public async Task InsertsJsonObjectAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = await con.InsertAsync(new Lead
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

            var lead = await con.GetAsync<Lead>(id);
            Assert.NotNull(lead);
            Assert.NotNull(lead.Data);
            Assert.NotNull(lead.Data?.FirstName);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public void InsertsDictionary(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = con.Insert(new Lead
            {
                Email = "foo@example.com",
                Metadata = new Dictionary<string, string>
                {
                    ["Foo"] = "Bar",
                    ["DateModified"] = DateTime.UtcNow.ToString(),
                }
            });

            var lead = con.Get<Lead>(id);
            Assert.NotNull(lead);
            Assert.NotNull(lead.Metadata);
            Assert.Equal("Bar", Assert.Contains("Foo", lead.Metadata));
            Assert.Contains("DateModified", lead.Metadata);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public async Task InsertsDictionaryAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var id = await con.InsertAsync(new Lead
            {
                Email = "foo@example.com",
                Metadata = new Dictionary<string, string>
                {
                    ["Foo"] = "Bar",
                    ["DateModified"] = DateTime.UtcNow.ToString(),
                }
            });

            var lead = await con.GetAsync<Lead>(id);
            Assert.NotNull(lead);
            Assert.NotNull(lead.Metadata);
            Assert.Equal("Bar", Assert.Contains("Foo", lead.Metadata));
            Assert.Contains("DateModified", lead.Metadata);
        }
    }
}
