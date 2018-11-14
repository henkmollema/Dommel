using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dommel.IntegrationTests
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // The foreign key to Categories table
        public int CategoryId { get; set; }

        // The navigation property
        public Category Category { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OrderLine.OrderId))]
        public ICollection<OrderLine> OrderLines { get; set; }
    }

    public class OrderLine
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public string Line { get; set; }
    }
}
