using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Xunit;

namespace Dommel.Json.Tests;

public class JsonObjectTypeHandlerTests
{
    private static readonly JsonObjectTypeHandler TypeHandler = new();

    [Fact]
    public void SetValue_CreatesJsonString()
    {
        var obj = new
        {
            Foo = "Bar",
            Baz = 123
        };
        var param = new SqlParameter();

        // Act
        TypeHandler.SetValue(param, obj);

        // Assert
        var json = JsonConvert.SerializeObject(obj);
        Assert.Equal(json, param.Value);
        Assert.Equal(DbType.String, param.DbType);
    }

    [Fact]
    public void SetValue_HandlesNull()
    {
        // Arrange
        var param = new SqlParameter();

        // Act
        TypeHandler.SetValue(param, null);

        // Assert
        Assert.Equal(DBNull.Value, param.Value);
        Assert.Equal(DbType.String, param.DbType);
    }

    [Fact]
    public void SetValue_HandlesDBNull()
    {
        // Arrange
        var param = new SqlParameter();

        // Act
        TypeHandler.SetValue(param, DBNull.Value);

        // Assert
        Assert.Equal(DBNull.Value, param.Value);
        Assert.Equal(DbType.String, param.DbType);
    }

    [Fact]
    public void Parse_DeserializesJsonString()
    {
        // Arrange
        var data = new LeadData
        {
            FirstName = "Foo",
            LastName = "Bar",
            Birthdate = new DateTime(1985, 7, 1),
            Email = "foo@example.com",
        };
        var json = JsonConvert.SerializeObject(data);

        // Act
        var obj = TypeHandler.Parse(typeof(LeadData), json);

        // Assert
        var parsedData = Assert.IsType<LeadData>(obj);
        Assert.Equal(data.FirstName, parsedData.FirstName);
        Assert.Equal(data.LastName, parsedData.LastName);
        Assert.Equal(data.Birthdate, parsedData.Birthdate);
        Assert.Equal(data.Email, parsedData.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(123)]
    [InlineData(123.4)]
    public void Parse_ReturnsNullForNonString(object value) => Assert.Null(TypeHandler.Parse(typeof(LeadData), value));
}
