using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dommel.Tests;

public class Product
{
    public int Id { get; set; }

    [Column("FullName")]
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
    public int Id { get; set; }

    public string? Reference { get; set; }

    public List<OrderLine>? OrderLines { get; set; }

    public List<Shipment>? Shipments { get; set; }

    public List<OrderLog>? Logs { get; set; }

    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public int EmployeeId { get; set; }

    public Employee? Employee { get; set; }

    public int PricingSettingsId { get; set; }

    public PricingSettings? PricingSettings { get; set; }
}

public class OrderLine : IEquatable<OrderLine>
{
    public OrderLine()
    {
    }

    public OrderLine(int id, string line)
    {
        Id = id;
        Line = line;
    }

    public int Id { get; set; }

    public int OrderId { get; set; }

    public string? Line { get; set; }

    public bool Equals(OrderLine? other) => Id == other?.Id;
}

public class Customer
{
    public int Id { get; set; }

    public string? Name { get; set; }
}

public class Employee
{
    public int Id { get; set; }

    public string? Name { get; set; }
}

public class OrderLog : IEquatable<OrderLog>
{
    public OrderLog()
    {
    }

    public OrderLog(int id, string message)
    {
        Id = id;
        Message = message;
    }

    public int Id { get; set; }

    public int OrderId { get; set; }

    public string? Message { get; set; }

    public bool Equals(OrderLog? other) => Id == other?.Id;
}

public class PricingSettings
{
    public int Id { get; set; }

    public decimal VatPercentage { get; set; } = 21.0M;
}

public class Shipment : IEquatable<Shipment>
{
    public Shipment()
    {
    }

    public Shipment(int id, string location)
    {
        Id = id;
        Location = location;
    }

    public int Id { get; set; }

    public int OrderId { get; set; }

    public string? Location { get; set; }

    public bool Equals(Shipment? other) => Id == other?.Id;
}