using Dommel.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dommel.IntegrationTests
{
    public class Product
    {
        [Identity]
        public int Id { get; set; }

        public string Name { get; set; }

        // The foreign key to Categories table
        public int CategoryId { get; set; }

        // The navigation property
        public Category Category { get; set; }
    }

    public class Category
    {
        [Identity]
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Order
    {
        [Identity]
        public int Id { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OrderLine.OrderId))]
        public ICollection<OrderLine> OrderLines { get; set; }
    }

    public class OrderLine
    {
        [Identity]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string Line { get; set; }
    }

    public class Foo
    {
        [Identity]
        public int Id { get; set; }

        public string Name { get; set; } = nameof(Foo);
    }

    public class Bar
    {
        [Identity]
        public int Id { get; set; }

        public string Name { get; set; } = nameof(Bar);
    }
}
