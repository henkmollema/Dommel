using System;
using System.Data.Common;
using System.Threading.Tasks;
using Dommel.IntegrationTests;
using Xunit;

namespace Dommel.Json.IntegrationTests;

[Collection("JSON Database")]
public class DeleteTests
{
    private static object InsertLead(DbConnection con)
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

    private static async Task<object> InsertLeadAsync(DbConnection con)
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
    public void SingleStatement(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var id = InsertLead(con);
        Assert.True(con.DeleteMultiple<Lead>(p => p.Data!.Email == "foo@example.com") > 0);
        var leads = con.Select<Lead>(p => p.Data!.Email == "foo@example.com");
        Assert.Empty(leads);
    }

    [Theory]
    [ClassData(typeof(JsonDatabaseTestData))]
    public async Task SingleStatementAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var id = await InsertLeadAsync(con);
        Assert.True(await con.DeleteMultipleAsync<Lead>(p => p.Data!.Email == "foo@example.com") > 0);
        var leads = await con.SelectAsync<Lead>(p => p.Data!.Email == "foo@example.com");
        Assert.Empty(leads);
    }

    [Theory]
    [ClassData(typeof(JsonDatabaseTestData))]
    public void AndStatement(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var id = InsertLead(con);
        Assert.True(con.DeleteMultiple<Lead>(p => p.Data!.FirstName == "Foo" && p.Data.LastName == "Bar" && p.Email == "foo@example.com") > 0);
        var leads = con.Select<Lead>(p => p.Data!.FirstName == "Foo" && p.Data.LastName == "Bar" && p.Email == "foo@example.com");
        Assert.Empty(leads);
    }

    [Theory]
    [ClassData(typeof(JsonDatabaseTestData))]
    public async Task AndStatementAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var id = await InsertLeadAsync(con);
        Assert.True(await con.DeleteMultipleAsync<Lead>(p => p.Data!.FirstName == "Foo" && p.Data.LastName == "Bar" && p.Email == "foo@example.com") > 0);
        var leads = await con.SelectAsync<Lead>(p => p.Data!.FirstName == "Foo" && p.Data.LastName == "Bar" && p.Email == "foo@example.com");
        Assert.Empty(leads);
    }

    [Theory]
    [ClassData(typeof(JsonDatabaseTestData))]
    public void OrStatement(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var id = InsertLead(con);
        Assert.True(con.DeleteMultiple<Lead>(p => p.Data!.FirstName == "Foo" && p.Data.LastName == "Bar" || p.Email == "foo@example.com") > 0);
        var leads = con.Select<Lead>(p => p.Data!.FirstName == "Foo" && p.Data.LastName == "Bar" || p.Email == "foo@example.com");
        Assert.Empty(leads);
    }

    [Theory]
    [ClassData(typeof(JsonDatabaseTestData))]
    public async Task OrStatementAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var id = await InsertLeadAsync(con);
        Assert.True(await con.DeleteMultipleAsync<Lead>(p => p.Data!.FirstName == "Foo" && p.Data.LastName == "Bar" || p.Email == "foo@example.com") > 0);
        var leads = await con.SelectAsync<Lead>(p => p.Data!.FirstName == "Foo" && p.Data.LastName == "Bar" || p.Email == "foo@example.com");
        Assert.Empty(leads);
    }
}
