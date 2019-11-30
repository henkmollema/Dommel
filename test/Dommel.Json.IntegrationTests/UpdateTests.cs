using System;
using System.Data.Common;
using System.Threading.Tasks;
using Dommel.IntegrationTests;
using Xunit;

namespace Dommel.Json.IntegrationTests
{
    [Collection("JSON Database")]
    public class UpdateTests
    {
        private Lead InsertLead(DbConnection con)
        {
            var id = con.Insert(new Lead
            {
                Data = new LeadData
                {
                    FirstName = "Foo",
                }
            });
            return con.Get<Lead>(id);
        }

        private async Task<object> InsertLeadAsync(DbConnection con)
        {
            var id = await con.InsertAsync(new Lead
            {
                Data = new LeadData
                {
                    FirstName = "Foo",
                }
            });
            return await con.GetAsync<Lead>(id);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public void SelectSingleStatement(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            // Arrange
            var lead = InsertLead(con);

            // Act
            lead.Data!.FirstName = "Bar";
            con.Update(lead);

            // Assert
            var updatedLead = con.Get<Lead>(lead.Id);
            Assert.Equal("Bar", updatedLead.Data?.FirstName);
        }

        [Theory]
        [ClassData(typeof(JsonDatabaseTestData))]
        public async Task SelectSingleStatementAsync(DatabaseDriver database)
        {
            using var con = database.GetConnection();
            var lead = await InsertLeadAsync(con);
        }
    }
}
