using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dommel.IntegrationTests;
public class Pluf
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int PlufId { get; set; }

    public string? Name { get; set; }
}
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Column("FullName")]
    public string? Name { get; set; }

    public string? Slug { get; private set; }

    public void SetSlug(string slug) => Slug = slug;

    // The foreign key to Categories table
    public int CategoryId { get; set; }

    // The navigation property
    public Category? Category { get; set; }

    // One Product has many Options
    public ICollection<ProductOption> Options { get; set; } = new List<ProductOption>();
}

public class Category : IEquatable<Category>
{
    [Key]
    public int CategoryId { get; set; }

    public string? Name { get; set; }

    public bool Equals(Category? other) => CategoryId == other?.CategoryId;
}

public class ProductOption : IEquatable<ProductOption>
{
    public int Id { get; set; }

    // One ProductOption has one Product (no navigation)
    // Represents the foreign key to the product table
    public int ProductId { get; set; }

    public bool Equals(ProductOption? other) => Id == other?.Id;
}

public class Order
{
    public int Id { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(OrderLine.OrderId))]
    public ICollection<OrderLine>? OrderLines { get; set; }
}

public class OrderLine
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string? Line { get; set; }
}

public class Foo
{
    public int Id { get; set; }

    public string? Name { get; set; } = nameof(Foo);
}

public class Bar
{
    public int Id { get; set; }

    public string? Name { get; set; } = nameof(Bar);
}

public class Baz
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid BazId { get; set; }

    public string? Name { get; set; } = nameof(Baz);
}