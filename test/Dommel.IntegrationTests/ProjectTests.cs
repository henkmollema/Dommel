using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Xunit;

namespace Dommel.IntegrationTests;

[Collection("Database")]
public class ProjectTests
{
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Project(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var p = con.Project<ProductSmall>(1);
        Assert.NotNull(p);
        Assert.NotEqual(0, p!.ProductId);
        Assert.NotNull(p.Name);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ProjectAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var p = await con.ProjectAsync<ProductSmall>(1);
        Assert.NotNull(p);
        Assert.NotEqual(0, p!.ProductId);
        Assert.NotNull(p.Name);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Project_ParamsOverload(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var p = con.Project<ProductSmall>(new object[] { 1 });
        Assert.NotNull(p);
        Assert.Equal(1, p!.ProductId);
        Assert.False(string.IsNullOrEmpty(p.Name));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ProjectAsync_ParamsOverload(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var p = await con.ProjectAsync<ProductSmall>(new object[] { 1 });
        Assert.NotNull(p);
        Assert.Equal(1, p!.ProductId);
        Assert.False(string.IsNullOrEmpty(p.Name));
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Project_ThrowsWhenCompositeKey(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ex = Assert.Throws<InvalidOperationException>(() => con.Project<ProjectedProductsCategories>(1));
        Assert.Equal("Entity ProjectedProductsCategories contains more than one key property.Use the Project<T> overload which supports passing multiple IDs.", ex.Message);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ProjectAsync_ThrowsWhenCompositeKey(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => con.ProjectAsync<ProjectedProductsCategories>(1));
        Assert.Equal("Entity ProjectedProductsCategories contains more than one key property.Use the Project<T> overload which supports passing multiple IDs.", ex.Message);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Project_CompositeKey(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var p = con.Project<ProjectedProductsCategories>(3, 1);
        Assert.NotNull(p);
        Assert.Equal(3, p!.Prod_id);
        Assert.Equal(1, p.CategoryId);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ProjectAsync_CompositeKey(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var p = await con.ProjectAsync<ProjectedProductsCategories>(3, 1);
        Assert.NotNull(p);
        Assert.Equal(3, p!.Prod_id);
        Assert.Equal(1, p.CategoryId);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void Project_ThrowsWhenCompositeKeyArgumentsDontMatch(DatabaseDriver database)
    {
        DommelMapper.QueryCache.Clear();
        using var con = database.GetConnection();
        var ex = Assert.Throws<InvalidOperationException>(() => con.Project<ProjectedProductsCategories>(1, 2, 3));
        Assert.Equal("Number of key columns (2) of type ProjectedProductsCategories does not match with the number of specified IDs (3).", ex.Message);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ProjectAsync_ThrowsWhenCompositeKeyArgumentsDontMatch(DatabaseDriver database)
    {
        DommelMapper.QueryCache.Clear();
        using var con = database.GetConnection();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => con.ProjectAsync<ProjectedProductsCategories>(1, 2, 3));
        Assert.Equal("Number of key columns (2) of type ProjectedProductsCategories does not match with the number of specified IDs (3).", ex.Message);
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void ProjectAll(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ps = con.ProjectAll<ProductSmall>();
        Assert.NotEmpty(ps);
        Assert.All(ps, p =>
        {
            Assert.NotEqual(0, p.ProductId);
            Assert.NotNull(p.Name);
        });
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ProjectAllAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ps = await con.ProjectAllAsync<ProductSmall>();
        Assert.NotEmpty(ps);
        Assert.All(ps, p =>
        {
            Assert.NotEqual(0, p.ProductId);
            Assert.NotNull(p.Name);
        });
    }
    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public void ProjectPagedAll(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ps = con.ProjectPaged<ProductSmall>(pageNumber: 1, pageSize: 5);
        Assert.NotEmpty(ps);
        Assert.All(ps, p =>
        {
            Assert.NotEqual(0, p.ProductId);
            Assert.NotNull(p.Name);
        });
    }

    [Theory]
    [ClassData(typeof(DatabaseTestData))]
    public async Task ProjectPagedAsync(DatabaseDriver database)
    {
        using var con = database.GetConnection();
        var ps = await con.ProjectPagedAsync<ProductSmall>(pageNumber: 1, pageSize: 5);
        Assert.NotEmpty(ps);
        Assert.All(ps, p =>
        {
            Assert.NotEqual(0, p.ProductId);
            Assert.NotNull(p.Name);
        });
    }

    // Subset of the default Product entity
    [Table("Products")]
    public class ProductSmall
    {
        [Key]
        public int ProductId { get; set; }

        [Column("FullName")]
        public string? Name { get; set; }
    }

    [Table("ProductsCategories")]
    public class ProjectedProductsCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ProductId")]
        public int Prod_id { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CategoryId { get; set; }
    }
}