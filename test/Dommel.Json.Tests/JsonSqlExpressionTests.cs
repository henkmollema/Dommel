using Xunit;

namespace Dommel.Json.Tests;

public class JsonSqlExpressionTests
{
    [Fact]
    public void GeneratesMySqlJsonValue()
    {
        // Arrange
        var sqlBuilder = new MySqlSqlBuilder();
        var sqlExpression = new SqlExpression<Lead>(sqlBuilder, new JsonSqlVisitor(sqlBuilder, new DommelJsonOptions()));

        // Act
        var str = sqlExpression.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where `Leads`.`Data`->'$.LastName' = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Fact]
    public void GeneratesSqlServerJsonValue()
    {
        // Arrange
        var sqlBuilder = new SqlServerSqlBuilder();
        var sqlExpression = new SqlExpression<Lead>(sqlBuilder, new JsonSqlVisitor(sqlBuilder, new DommelJsonOptions()));

        // Act
        var str = sqlExpression.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where JSON_VALUE([Leads].[Data], '$.LastName') = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Fact]
    public void GeneratesSqliteJsonValue()
    {
        // Arrange
        var sqlBuilder = new SqliteSqlBuilder();
        var sqlExpression = new SqlExpression<Lead>(sqlBuilder, new JsonSqlVisitor(sqlBuilder, new DommelJsonOptions()));

        // Act
        var str = sqlExpression.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where JSON_EXTRACT(Leads.Data, '$.LastName') = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Fact]
    public void GeneratesSqlServerCeJsonValue()
    {
        // Arrange
        var sqlBuilder = new SqlServerCeSqlBuilder();
        var sqlExpression = new SqlExpression<Lead>(sqlBuilder, new JsonSqlVisitor(sqlBuilder, new DommelJsonOptions()));

        // Act
        var str = sqlExpression.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where JSON_VALUE([Leads].[Data], '$.LastName') = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }

    [Fact]
    public void GeneratesPostgresJsonValue()
    {
        // Arrange
        var sqlBuilder = new PostgresSqlBuilder();
        var sqlExpression = new SqlExpression<Lead>(sqlBuilder, new JsonSqlVisitor(sqlBuilder, new DommelJsonOptions()));

        // Act
        var str = sqlExpression.Where(p => p.Data.LastName == "Foo").ToSql(out var parameters);

        // Assert
        Assert.Equal(" where \"Leads\".\"Data\"->>'LastName' = @p1", str);
        Assert.Equal("p1", Assert.Single(parameters.ParameterNames));
    }
}
