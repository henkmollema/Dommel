using System;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Tests
{
    public class KeyPropertyInfoTests
    {
        [Fact]
        public void ThrowsForNullPropertyInfo()
        {
            Assert.Throws<ArgumentNullException>("property", () => new KeyPropertyInfo(null!));
            Assert.Throws<ArgumentNullException>("property", () => new KeyPropertyInfo(null!, default));
        }

        [Fact]
        public void UsesGeneratedOptionsIdentityDefault()
        {
            var kpi = new KeyPropertyInfo(typeof(Foo).GetProperty("Id")!);
            Assert.Equal(DatabaseGeneratedOption.Identity, kpi.GeneratedOption);
            Assert.True(kpi.IsGenerated);
            Assert.Equal(typeof(Foo).GetProperty("Id"), kpi.Property);
        }

        [Fact]
        public void DeterminesDatabaseGeneratedOption_Computed()
        {
            var kpi = new KeyPropertyInfo(typeof(Bar).GetProperty("Id")!);
            Assert.Equal(DatabaseGeneratedOption.Computed, kpi.GeneratedOption);
            Assert.True(kpi.IsGenerated);
            Assert.Equal(typeof(Bar).GetProperty("Id"), kpi.Property);
        }

        [Fact]
        public void DeterminesDatabaseGeneratedOption_None()
        {
            var kpi = new KeyPropertyInfo(typeof(Baz).GetProperty("Id")!);
            Assert.Equal(DatabaseGeneratedOption.None, kpi.GeneratedOption);
            Assert.False(kpi.IsGenerated);
            Assert.Equal(typeof(Baz).GetProperty("Id"), kpi.Property);
        }

        [Theory]
        [InlineData(DatabaseGeneratedOption.None)]
        [InlineData(DatabaseGeneratedOption.Identity)]
        [InlineData(DatabaseGeneratedOption.Computed)]
        public void UsesSpecifiedDatabaseGeneratedOption(DatabaseGeneratedOption databaseGeneratedOption)
        {
            var kpi = new KeyPropertyInfo(typeof(Bar).GetProperty("Id")!, databaseGeneratedOption);
            Assert.Equal(databaseGeneratedOption, kpi.GeneratedOption);
            Assert.Equal(typeof(Bar).GetProperty("Id"), kpi.Property);
        }

        private class Foo
        {
            public int Id { get; set; }
        }

        private class Bar
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            public Guid Id { get; set; }
        }

        private class Baz
        {
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public Guid Id { get; set; }
        }
    }
}
