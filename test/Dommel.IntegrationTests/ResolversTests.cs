using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Tests
{
    public class ResolversTests
    {
        private readonly SqlConnection sqlConnection = new SqlConnection();

        [Fact]
        public void Table_WithSchema()
        {
            Assert.Equal("[dbo].[Qux]", Resolvers.Table(typeof(FooQux), sqlConnection));
            Assert.Equal("[foo].[dbo].[Qux]", Resolvers.Table(typeof(FooDboQux), sqlConnection));
        }

        [Fact]
        public void Table_NoCacheConflictNestedClass()
        {
            Assert.Equal("[BarA]", Resolvers.Table(typeof(Foo.Bar), sqlConnection));
            Assert.Equal("[BarB]", Resolvers.Table(typeof(Baz.Bar), sqlConnection));
        }

        public class Foo
        {
            [Table("BarA")]
            public class Bar
            {
                [Column("BazA")]
                public string? Baz { get; set; }
            }

            [Table("BarA")]
            public class BarChild
            {
                public int BarId { get; set; }
            }
        }

        public class Baz
        {
            [Table("BarB")]
            public class Bar
            {
                [Column("BazB")]
                public string? Baz { get; set; }
            }

            [Table("BarA")]
            public class BarChild
            {
                public int BarId { get; set; }
            }
        }

        [Table("Qux", Schema = "foo.dbo")]
        public class FooDboQux
        {
            public int Id { get; set; }
        }

        [Table("Qux", Schema = "dbo")]
        public class FooQux
        {
            public int Id { get; set; }
        }
    }
}
