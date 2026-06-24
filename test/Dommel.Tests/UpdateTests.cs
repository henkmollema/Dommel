using System;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests;

public class UpdateTests
{
    private static readonly SqlServerSqlBuilder SqlServer = new();
    private static readonly MySqlSqlBuilder MySql = new();
    private static readonly PostgresSqlBuilder Postgres = new();

    [Fact]
    public void BuildUpdateMultipleQuery_ConstantAndArithmetic_SqlServer()
    {
        var sql = BuildUpdateMultipleQuery<Product>(
            SqlServer,
            update => update.SetProperty(p => p.Name, "Chai").SetProperty(p => p.CategoryId, p => p.CategoryId * 2),
            p => p.CategoryId == 1,
            out var parameters);

        Assert.Equal(
            "update [Products] set [FullName] = @p1, [CategoryId] = [Products].[CategoryId] * @p2 where [Products].[CategoryId] = @p3",
            sql);
        Assert.Collection(parameters.ParameterNames,
            name => Assert.Equal("Chai", parameters.Get<string>(name)),
            name => Assert.Equal(2, parameters.Get<int>(name)),
            name => Assert.Equal(1, parameters.Get<int>(name)));
    }

    [Fact]
    public void BuildUpdateMultipleQuery_ConstantValues_MySql()
    {
        var sql = BuildUpdateMultipleQuery<Product>(
            MySql,
            update => update.SetProperty(p => p.Name, "<removed>").SetProperty(p => p.CategoryId, 0),
            p => p.CategoryId == 5,
            out var parameters);

        Assert.Equal(
            "update `Products` set `FullName` = @p1, `CategoryId` = @p2 where `Products`.`CategoryId` = @p3",
            sql);
        Assert.Equal("<removed>", parameters.Get<string>("p1"));
        Assert.Equal(0, parameters.Get<int>("p2"));
        Assert.Equal(5, parameters.Get<int>("p3"));
    }

    [Fact]
    public void BuildUpdateMultipleQuery_DoesNotQualifyAssignmentTarget_Postgres()
    {
        // Postgres rejects a table-qualified assignment target, so the set-clause column
        // must be unqualified while the where-clause column stays qualified.
        var sql = BuildUpdateMultipleQuery<Product>(
            Postgres,
            update => update.SetProperty(p => p.Name, "x"),
            p => p.Id == 1,
            out _);

        Assert.Equal(
            "update \"Products\" set \"FullName\" = @p1 where \"Products\".\"Id\" = @p2",
            sql);
    }

    [Fact]
    public void BuildUpdateMultipleQuery_ColumnReferenceValueSelector_IsNotParameterized()
    {
        var sql = BuildUpdateMultipleQuery<Product>(
            SqlServer,
            update => update.SetProperty(p => p.CategoryId, p => p.Id),
            p => p.Id == 1,
            out var parameters);

        Assert.Equal(
            "update [Products] set [CategoryId] = [Products].[Id] where [Products].[Id] = @p1",
            sql);

        // Only the where-clause produced a parameter; the column reference did not.
        Assert.Single(parameters.ParameterNames);
    }

    [Fact]
    public void BuildUpdateMultipleQuery_PreservesSourceOrderOfSetProperty()
    {
        var sql = BuildUpdateMultipleQuery<Product>(
            SqlServer,
            update => update.SetProperty(p => p.CategoryId, 1).SetProperty(p => p.Name, "a"),
            p => p.Id == 1,
            out _);

        Assert.Equal(
            "update [Products] set [CategoryId] = @p1, [FullName] = @p2 where [Products].[Id] = @p3",
            sql);
    }

    [Fact]
    public void BuildUpdateMultipleQuery_NoSetProperty_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            BuildUpdateMultipleQuery<Product>(SqlServer, update => update, p => p.Id == 1, out _));
        Assert.Contains("At least one SetProperty call is required.", ex.Message);
    }
}
