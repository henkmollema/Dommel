using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class DefaultForeignKeyPropertyResolverTests
    {
        [Fact]
        public void Resolves_OneToOne_WithDefaultConvetions()
        {
            // Arrange
            var resolver = new DefaultForeignKeyPropertyResolver();

            // Act
            var fk = resolver.ResolveForeignKeyProperty(typeof(Product), typeof(Category), out var fkRelation);

            // Assert
            Assert.Equal(typeof(Product).GetProperty(nameof(Product.CategoryId)), fk);
            Assert.Equal(ForeignKeyRelation.OneToOne, fkRelation);
        }

        [Fact]
        public void Resolves_OneToMany_WithDefaultConvetions()
        {
            // Arrange
            var resolver = new DefaultForeignKeyPropertyResolver();

            // Act
            var fk = resolver.ResolveForeignKeyProperty(typeof(Product), typeof(ProductOption), out var fkRelation);

            // Assert
            Assert.Equal(typeof(ProductOption).GetProperty(nameof(ProductOption.ProductId)), fk);
            Assert.Equal(ForeignKeyRelation.OneToMany, fkRelation);
        }

        [Fact]
        public void Resolves_OneToOne_WithAttributes()
        {
            // Arrange
            var resolver = new DefaultForeignKeyPropertyResolver();

            // Act
            var fk = resolver.ResolveForeignKeyProperty(typeof(ProductDto), typeof(CategoryDto), out var fkRelation);

            // Assert
            Assert.Equal(typeof(ProductDto).GetProperty(nameof(ProductDto.CategoryId)), fk);
            Assert.Equal(ForeignKeyRelation.OneToOne, fkRelation);
        }

        [Fact]
        public void Resolves_OneToMany_WithAttributes()
        {
            // Arrange
            var resolver = new DefaultForeignKeyPropertyResolver();

            // Act
            var fk = resolver.ResolveForeignKeyProperty(typeof(ProductDto), typeof(ProductOptionDto), out var fkRelation);

            // Assert
            Assert.Equal(typeof(ProductOptionDto).GetProperty(nameof(ProductOptionDto.ProductId)), fk);
            Assert.Equal(ForeignKeyRelation.OneToMany, fkRelation);
        }
    }

    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        // One Product has one Category
        public Category? Category { get; set; }

        // Represents the foreign key to the category table
        public int CategoryId { get; set; }

        // One Product has many Options
        public List<ProductOption>? Options { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    public class ProductOption
    {
        public int Id { get; set; }

        // One ProductOption has one Product (no navigation)
        // Represents the foreign key to the product table
        public int ProductId { get; set; }
    }

    public class ProductDto
    {
        // One Product has One
        [ForeignKey("CategoryId")]
        public CategoryDto? Category { get; set; }

        public int CategoryId { get; set; }

        // One Product has many Options
        [ForeignKey("ProductId")]
        public List<ProductOptionDto>? Options { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    public class ProductOptionDto
    {
        public int Id { get; set; }

        // One ProductOption has one Product (no navigation)
        public int ProductId { get; set; }
    }

    public class Order
    {
        // One Order has many Products
        // TODO: How to support this?
        public List<Product>? Products { get; set; }
    }
}
