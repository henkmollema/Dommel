using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Tests
{
    public class DefaultForeignKeyPropertyResolverTests
    {
        [Fact]
        public void Resolves_ThrowsWhenUnableToFind()
        {
            var resolver = new DefaultForeignKeyPropertyResolver();
            var fk = Assert.Throws<InvalidOperationException>(() => resolver.ResolveForeignKeyProperty(typeof(Product), typeof(Product), out var fkRelation));
            Assert.Equal("Could not resolve foreign key property. Source type 'Dommel.Tests.Product'; including type: 'Dommel.Tests.Product'.", fk.Message);
        }

        [Fact]
        public void Resolves_OneToOne_WithDefaultConventions()
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
        public void Resolves_OneToMany_WithDefaultConventions()
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
}
