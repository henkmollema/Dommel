using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests;

public class ProjectTests
{
    private static readonly ISqlBuilder SqlBuilder = new SqlServerSqlBuilder();

    [Fact]
    public void ProjectById()
    {
        var sql = BuildProjectById(SqlBuilder, typeof(ProjectedFoo), 42, out var parameters);
        Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos] where [ProjectedFoos].[Id] = @Id", sql);
        Assert.NotNull(parameters);
    }

    [Fact]
    public void ProjectByIds()
    {
        var sql = BuildProjectByIds(SqlBuilder, typeof(ProjectedProductsCategories), new object[] { 4, 2 }, out var parameters);
        Assert.Equal("select [ProductId], [CategoryId], [FullName] from [ProjectedProductsCategories] where [ProjectedProductsCategories].[ProductId] = @Id0 and [ProjectedProductsCategories].[CategoryId] = @Id1", sql);
        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.ParameterNames.Count());
        Assert.Equal(4, parameters.Get<int>("Id0"));
        Assert.Equal(2, parameters.Get<int>("Id1"));
    }

    [Fact]
    public void ProjectAll()
    {
        var sql = BuildProjectAllQuery(SqlBuilder, typeof(ProjectedFoo));
        Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos]", sql);
    }

    [Fact]
    public void ProjectPaged()
    {
        var sql = BuildProjectPagedQuery(SqlBuilder, typeof(ProjectedFoo), 1, 5);
        Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos] order by [ProjectedFoos].[Id] offset 0 rows fetch next 5 rows only", sql);
    }

    public class ProjectedFoo
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateTime? DateUpdated { get; set; }
    }

    public class ProjectedProductsCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductId { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }

        [Column("FullName")]
        public string? Name { get; set; }
    }
}
