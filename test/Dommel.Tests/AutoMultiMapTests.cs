using System.Collections.Generic;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests;

public class AutoMultiMapTests
{
    [Fact]
    public void Map1_OneToOne()
    {
        var del = CreateMapDelegate<Product, Category, DontMap, DontMap, DontMap, DontMap, DontMap, Product>(new Dictionary<int, Product>());
        var product = new Product();
        var category = new Category();
        var x = del.DynamicInvoke(product, category);
        var mappedProduct = Assert.IsType<Product>(x);
        Assert.NotNull(mappedProduct.Category);
    }

    [Fact]
    public void Map2_OneToOne()
    {
        var del = CreateMapDelegate<Product, Category, Category, DontMap, DontMap, DontMap, DontMap, Product>(new Dictionary<int, Product>());
        var product = new Product();
        var category = new Category();
        var x = del.DynamicInvoke(product, category, category);
        var mappedProduct = Assert.IsType<Product>(x);
        Assert.NotNull(mappedProduct.Category);
    }

    [Fact]
    public void Map3_OneToOne()
    {
        var del = CreateMapDelegate<Product, Category, Category, Category, DontMap, DontMap, DontMap, Product>(new Dictionary<int, Product>());
        var product = new Product();
        var category = new Category();
        var x = del.DynamicInvoke(product, category, category, category);
        var mappedProduct = Assert.IsType<Product>(x);
        Assert.NotNull(mappedProduct.Category);
    }

    [Fact]
    public void Map4_OneToOne()
    {
        var del = CreateMapDelegate<Product, Category, Category, Category, Category, DontMap, DontMap, Product>(new Dictionary<int, Product>());
        var product = new Product();
        var category = new Category();
        var x = del.DynamicInvoke(product, category, category, category, category);
        var mappedProduct = Assert.IsType<Product>(x);
        Assert.NotNull(mappedProduct.Category);
    }

    [Fact]
    public void Map5_OneToOne()
    {
        var del = CreateMapDelegate<Product, Category, Category, Category, Category, Category, DontMap, Product>(new Dictionary<int, Product>());
        var product = new Product();
        var category = new Category();
        var x = del.DynamicInvoke(product, category, category, category, category, category);
        var mappedProduct = Assert.IsType<Product>(x);
        Assert.NotNull(mappedProduct.Category);
    }

    [Fact]
    public void Map6_OneToOne()
    {
        var del = CreateMapDelegate<Product, Category, Category, Category, Category, Category, Category, Product>(new Dictionary<int, Product>());
        var product = new Product();
        var category = new Category();
        var x = del.DynamicInvoke(product, category, category, category, category, category, category);
        var mappedProduct = Assert.IsType<Product>(x);
        Assert.NotNull(mappedProduct.Category);
    }

    [Fact]
    public void Map1_OneToMany()
    {
        // Arrange
        var results = new Dictionary<int, Order>();
        var del = CreateMapDelegate<Order, OrderLine, DontMap, DontMap, DontMap, DontMap, DontMap, Order>(results);

        var o1 = new Order { Id = 1, Reference = "Order1" };
        var o2 = new Order { Id = 2, Reference = "Order2" };

        // Act
        del.DynamicInvoke(o1, new OrderLine(1, "Line1"));
        del.DynamicInvoke(o1, new OrderLine(2, "Line2"));
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"));

        del.DynamicInvoke(o2, new OrderLine(4, "Line4"));

        // Assert
        Assert.Collection(results.Values,
            x =>
            {
                    // Order
                    Assert.Equal(1, x.Id);
                Assert.Equal("Order1", x.Reference);

                    // Order lines
                    Assert.Collection(x.OrderLines!,
                    x => Assert.Equal("Line1", x.Line),
                    x => Assert.Equal("Line2", x.Line),
                    x => Assert.Equal("Line3", x.Line));

                Assert.Null(x.Customer);
                Assert.Null(x.PricingSettings);
                Assert.Null(x.Logs);
                Assert.Null(x.Shipments);
                Assert.Null(x.Employee);
            },
            x =>
            {
                    // Order
                    Assert.Equal(2, x.Id);
                Assert.Equal("Order2", x.Reference);

                    // Order lines
                    Assert.Equal("Line4", Assert.Single(x.OrderLines!).Line);

                Assert.Null(x.Customer);
                Assert.Null(x.PricingSettings);
                Assert.Null(x.Logs);
                Assert.Null(x.Shipments);
                Assert.Null(x.Employee);
            });
    }

    [Fact]
    public void Map2_OneToMany()
    {
        // Arrange
        var results = new Dictionary<int, Order>();
        var del = CreateMapDelegate<Order, OrderLine, Customer, DontMap, DontMap, DontMap, DontMap, Order>(results);

        var cus1 = new Customer { Id = 2001, Name = "Foo" };
        var o1 = new Order
        {
            Id = 1,
            Reference = "Order1",
            CustomerId = cus1.Id,
        };

        var cus2 = new Customer { Id = 2002, Name = "Bar" };
        var o2 = new Order
        {
            Id = 2,
            Reference = "Order2",
            CustomerId = cus2.Id,
        };

        // Act
        del.DynamicInvoke(o1, new OrderLine(1, "Line1"), cus1);
        del.DynamicInvoke(o1, new OrderLine(2, "Line2"), cus1);
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1);

        del.DynamicInvoke(o2, new OrderLine(4, "Line4"), cus2);

        // Assert
        Assert.Collection(results.Values,
            x =>
            {
                    // Order
                    Assert.Equal(1, x.Id);
                Assert.Equal("Order1", x.Reference);

                    // Order lines
                    Assert.Collection(x.OrderLines!,
                    x => Assert.Equal("Line1", x.Line),
                    x => Assert.Equal("Line2", x.Line),
                    x => Assert.Equal("Line3", x.Line));

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer!.Id);
                Assert.Equal("Foo", x.Customer.Name);

                Assert.Null(x.Logs);
                Assert.Null(x.Employee);
                Assert.Null(x.Shipments);
                Assert.Null(x.PricingSettings);
            },
            x =>
            {
                    // Order
                    Assert.Equal(2, x.Id);
                Assert.Equal("Order2", x.Reference);

                    // Order lines
                    Assert.Equal("Line4", Assert.Single(x.OrderLines!).Line);

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer!.Id);
                Assert.Equal("Bar", x.Customer.Name);

                Assert.Null(x.Logs);
                Assert.Null(x.Employee);
                Assert.Null(x.Shipments);
                Assert.Null(x.PricingSettings);
            });
    }

    [Fact]
    public void Map3_OneToMany()
    {
        // Arrange
        var results = new Dictionary<int, Order>();
        var del = CreateMapDelegate<Order, OrderLine, Customer, PricingSettings, DontMap, DontMap, DontMap, Order>(results);

        var ps = new PricingSettings { Id = 13 };
        var cus1 = new Customer { Id = 2001, Name = "Foo" };
        var o1 = new Order
        {
            Id = 1,
            Reference = "Order1",
            CustomerId = cus1.Id,
            PricingSettingsId = ps.Id,
        };

        var cus2 = new Customer { Id = 2002, Name = "Bar" };
        var o2 = new Order
        {
            Id = 2,
            Reference = "Order2",
            CustomerId = cus2.Id,
            PricingSettingsId = ps.Id,
        };

        // Act
        del.DynamicInvoke(o1, new OrderLine(1, "Line1"), cus1, ps);
        del.DynamicInvoke(o1, new OrderLine(2, "Line2"), cus1, ps);
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps);

        del.DynamicInvoke(o2, new OrderLine(4, "Line4"), cus2, ps);

        // Assert
        Assert.Collection(results.Values,
            x =>
            {
                    // Order
                    Assert.Equal(1, x.Id);
                Assert.Equal("Order1", x.Reference);

                    // Order lines
                    Assert.Collection(x.OrderLines!,
                    x => Assert.Equal("Line1", x.Line),
                    x => Assert.Equal("Line2", x.Line),
                    x => Assert.Equal("Line3", x.Line));

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer!.Id);
                Assert.Equal("Foo", x.Customer.Name);

                    // Pricing settings
                    Assert.NotNull(x.PricingSettings);
                Assert.Equal(x.PricingSettingsId, x.PricingSettings?.Id);

                Assert.Null(x.Employee);
                Assert.Null(x.Shipments);
                Assert.Null(x.Logs);
            },
            x =>
            {
                    // Order
                    Assert.Equal(2, x.Id);
                Assert.Equal("Order2", x.Reference);

                    // Order lines
                    Assert.Equal("Line4", Assert.Single(x.OrderLines!).Line);

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer!.Id);
                Assert.Equal("Bar", x.Customer.Name);

                    // Pricing settings
                    Assert.NotNull(x.PricingSettings);
                Assert.Equal(x.PricingSettingsId, x.PricingSettings?.Id);

                Assert.Null(x.Employee);
                Assert.Null(x.Shipments);
                Assert.Null(x.Logs);
            });
    }

    [Fact]
    public void Map4_OneToMany()
    {
        // Arrange
        var results = new Dictionary<int, Order>();
        var del = CreateMapDelegate<Order, OrderLine, Customer, PricingSettings, OrderLog, DontMap, DontMap, Order>(results);

        var ps = new PricingSettings { Id = 13 };
        var cus1 = new Customer { Id = 2001, Name = "Foo" };
        var o1 = new Order
        {
            Id = 1,
            Reference = "Order1",
            CustomerId = cus1.Id,
            PricingSettingsId = ps.Id,
        };

        var cus2 = new Customer { Id = 2002, Name = "Bar" };
        var o2 = new Order
        {
            Id = 2,
            Reference = "Order2",
            CustomerId = cus2.Id,
            PricingSettingsId = ps.Id,
        };

        // Act
        del.DynamicInvoke(o1, new OrderLine(1, "Line1"), cus1, ps, new OrderLog(1, "A"));
        del.DynamicInvoke(o1, new OrderLine(2, "Line2"), cus1, ps, new OrderLog(2, "B"));
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(3, "C"));
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(4, "D"));
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(5, "E"));

        del.DynamicInvoke(o2, new OrderLine(4, "Line4"), cus2, ps, new OrderLog(6, "F"));
        del.DynamicInvoke(o2, new OrderLine(4, "Line4"), cus2, ps, new OrderLog(7, "G"));

        // Assert
        Assert.Collection(results.Values,
            x =>
            {
                    // Order
                    Assert.Equal(1, x.Id);
                Assert.Equal("Order1", x.Reference);

                    // Order lines
                    Assert.Collection(x.OrderLines!,
                    x => Assert.Equal("Line1", x.Line),
                    x => Assert.Equal("Line2", x.Line),
                    x => Assert.Equal("Line3", x.Line));

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer!.Id);
                Assert.Equal("Foo", x.Customer.Name);

                    // Pricing settings
                    Assert.NotNull(x.PricingSettings);
                Assert.Equal(x.PricingSettingsId, x.PricingSettings?.Id);

                    // Order logs
                    Assert.Collection(x.Logs!,
                    x => Assert.Equal("A", x.Message),
                    x => Assert.Equal("B", x.Message),
                    x => Assert.Equal("C", x.Message),
                    x => Assert.Equal("D", x.Message),
                    x => Assert.Equal("E", x.Message));

                Assert.Null(x.Employee);
                Assert.Null(x.Shipments);
            },
            x =>
            {
                    // Order
                    Assert.Equal(2, x.Id);
                Assert.Equal("Order2", x.Reference);

                    // Order lines
                    Assert.Equal("Line4", Assert.Single(x.OrderLines!).Line);

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer!.Id);
                Assert.Equal("Bar", x.Customer.Name);

                    // Pricing settings
                    Assert.NotNull(x.PricingSettings);
                Assert.Equal(x.PricingSettingsId, x.PricingSettings?.Id);

                    // Order logs
                    Assert.Collection(x.Logs!,
                    x => Assert.Equal("F", x.Message),
                    x => Assert.Equal("G", x.Message));

                Assert.Null(x.Employee);
                Assert.Null(x.Shipments);
            });
    }

    [Fact]
    public void Map5_OneToMany()
    {
        // Arrange
        var results = new Dictionary<int, Order>();
        var del = CreateMapDelegate<Order, OrderLine, Customer, PricingSettings, OrderLog, Employee, DontMap, Order>(results);

        var empl = new Employee { Id = 3, Name = "Sam" };
        var ps = new PricingSettings { Id = 13 };
        var cus1 = new Customer { Id = 2001, Name = "Foo" };
        var o1 = new Order
        {
            Id = 1,
            Reference = "Order1",
            CustomerId = cus1.Id,
            EmployeeId = empl.Id,
            PricingSettingsId = ps.Id,
        };

        var cus2 = new Customer { Id = 2002, Name = "Bar" };
        var o2 = new Order
        {
            Id = 2,
            Reference = "Order2",
            CustomerId = cus2.Id,
            EmployeeId = empl.Id,
            PricingSettingsId = ps.Id,
        };

        // Act
        del.DynamicInvoke(o1, new OrderLine(1, "Line1"), cus1, ps, new OrderLog(1, "A"), empl);
        del.DynamicInvoke(o1, new OrderLine(2, "Line2"), cus1, ps, new OrderLog(2, "B"), empl);
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(3, "C"), empl);
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(4, "D"), empl);
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(5, "E"), empl);

        del.DynamicInvoke(o2, new OrderLine(4, "Line4"), cus2, ps, new OrderLog(6, "F"), empl);
        del.DynamicInvoke(o2, new OrderLine(4, "Line4"), cus2, ps, new OrderLog(7, "G"), empl);

        // Assert
        Assert.Collection(results.Values,
            x =>
            {
                    // Order
                    Assert.Equal(1, x.Id);
                Assert.Equal("Order1", x.Reference);

                    // Order lines
                    Assert.Collection(x.OrderLines!,
                    x => Assert.Equal("Line1", x.Line),
                    x => Assert.Equal("Line2", x.Line),
                    x => Assert.Equal("Line3", x.Line));

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer?.Id);
                Assert.Equal("Foo", x.Customer?.Name);

                    // Pricing settings
                    Assert.NotNull(x.PricingSettings);
                Assert.Equal(x.PricingSettingsId, x.PricingSettings?.Id);

                    // Order logs
                    Assert.Collection(x.Logs!,
                    x => Assert.Equal("A", x.Message),
                    x => Assert.Equal("B", x.Message),
                    x => Assert.Equal("C", x.Message),
                    x => Assert.Equal("D", x.Message),
                    x => Assert.Equal("E", x.Message));

                    // Employee
                    Assert.NotNull(x.Employee);
                Assert.Equal(x.EmployeeId, x.Employee?.Id);
                Assert.Equal("Sam", x.Employee?.Name);

                    // Shipments
                    Assert.Null(x.Shipments);
            },
            x =>
            {
                    // Order
                    Assert.Equal(2, x.Id);
                Assert.Equal("Order2", x.Reference);

                    // Order lines
                    Assert.Equal("Line4", Assert.Single(x.OrderLines!).Line);

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer?.Id);
                Assert.Equal("Bar", x.Customer?.Name);

                    // Pricing settings
                    Assert.NotNull(x.PricingSettings);
                Assert.Equal(x.PricingSettingsId, x.PricingSettings?.Id);

                    // Order logs
                    Assert.Collection(x.Logs!,
                    x => Assert.Equal("F", x.Message),
                    x => Assert.Equal("G", x.Message));

                    // Employee
                    Assert.NotNull(x.Employee);
                Assert.Equal(x.EmployeeId, x.Employee?.Id);
                Assert.Equal("Sam", x.Employee?.Name);

                    // Shipments
                    Assert.Null(x.Shipments);
            });
    }

    [Fact]
    public void Map6_OneToMany()
    {
        // Arrange
        var results = new Dictionary<int, Order>();
        var del = CreateMapDelegate<Order, OrderLine, Customer, PricingSettings, OrderLog, Employee, Shipment, Order>(results);

        var empl = new Employee { Id = 3, Name = "Sam" };
        var ps = new PricingSettings { Id = 13 };
        var cus1 = new Customer { Id = 2001, Name = "Foo" };
        var o1 = new Order
        {
            Id = 1,
            Reference = "Order1",
            CustomerId = cus1.Id,
            EmployeeId = empl.Id,
            PricingSettingsId = ps.Id,
        };

        var cus2 = new Customer { Id = 2002, Name = "Bar" };
        var o2 = new Order
        {
            Id = 2,
            Reference = "Order2",
            CustomerId = cus2.Id,
            EmployeeId = empl.Id,
            PricingSettingsId = ps.Id,
        };

        // Act
        del.DynamicInvoke(o1, new OrderLine(1, "Line1"), cus1, ps, new OrderLog(1, "A"), empl, new Shipment(1, "Foo"));
        del.DynamicInvoke(o1, new OrderLine(2, "Line2"), cus1, ps, new OrderLog(2, "B"), empl, new Shipment(2, "Bar"));
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(3, "C"), empl, new Shipment(2, "Bar"));
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(4, "D"), empl, new Shipment(2, "Bar"));
        del.DynamicInvoke(o1, new OrderLine(3, "Line3"), cus1, ps, new OrderLog(5, "E"), empl, new Shipment(2, "Bar"));

        del.DynamicInvoke(o2, new OrderLine(4, "Line4"), cus2, ps, new OrderLog(6, "F"), empl, new Shipment(3, "Baz"));
        del.DynamicInvoke(o2, new OrderLine(4, "Line4"), cus2, ps, new OrderLog(7, "G"), empl, new Shipment(3, "Baz"));

        // Assert
        Assert.Collection(results.Values,
            x =>
            {
                    // Order
                    Assert.Equal(1, x.Id);
                Assert.Equal("Order1", x.Reference);

                    // Order lines
                    Assert.Collection(x.OrderLines!,
                    x => Assert.Equal("Line1", x.Line),
                    x => Assert.Equal("Line2", x.Line),
                    x => Assert.Equal("Line3", x.Line));

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer!.Id);
                Assert.Equal("Foo", x.Customer.Name);

                    // Pricing settings
                    Assert.NotNull(x.PricingSettings);
                Assert.Equal(x.PricingSettingsId, x.PricingSettings?.Id);

                    // Order logs
                    Assert.Collection(x.Logs!,
                    x => Assert.Equal("A", x.Message),
                    x => Assert.Equal("B", x.Message),
                    x => Assert.Equal("C", x.Message),
                    x => Assert.Equal("D", x.Message),
                    x => Assert.Equal("E", x.Message));

                    // Employee
                    Assert.NotNull(x.Employee);
                Assert.Equal(x.EmployeeId, x.Employee!.Id);
                Assert.Equal("Sam", x.Employee.Name);

                    // Shipments
                    Assert.Collection(x.Shipments!,
                    x => Assert.Equal("Foo", x.Location),
                    x => Assert.Equal("Bar", x.Location));
            },
            x =>
            {
                    // Order
                    Assert.Equal(2, x.Id);
                Assert.Equal("Order2", x.Reference);

                    // Order lines
                    Assert.Equal("Line4", Assert.Single(x.OrderLines!).Line);

                    // Customer
                    Assert.NotNull(x.Customer);
                Assert.Equal(x.CustomerId, x.Customer!.Id);
                Assert.Equal("Bar", x.Customer.Name);

                    // Pricing settings
                    Assert.NotNull(x.PricingSettings);
                Assert.Equal(x.PricingSettingsId, x.PricingSettings?.Id);

                    // Order logs
                    Assert.Collection(x.Logs!,
                    x => Assert.Equal("F", x.Message),
                    x => Assert.Equal("G", x.Message));

                    // Employee
                    Assert.NotNull(x.Employee);
                Assert.Equal(x.EmployeeId, x.Employee!.Id);
                Assert.Equal("Sam", x.Employee.Name);

                    // Shipments
                    Assert.Equal("Baz", Assert.Single(x.Shipments!).Location);
            });
    }
}
