using System;
using System.ComponentModel.DataAnnotations.Schema;
using Xunit;

namespace Dommel.Tests
{
    public class ColumnPropertyInfoTests
    {
        [Fact]
        public void ThrowsForNullPropertyInfo()
        {
            Assert.Throws<ArgumentNullException>("property", () => new ColumnPropertyInfo(null!));
            Assert.Throws<ArgumentNullException>("property", () => new ColumnPropertyInfo(null!, isKey: false));
            Assert.Throws<ArgumentNullException>("property", () => new ColumnPropertyInfo(null!, isKey: true));
            Assert.Throws<ArgumentNullException>("property", () => new ColumnPropertyInfo(null!, default(DatabaseGeneratedOption)));
        }

        [Fact]
        public void UsesGeneratedOptionsNone_ForRegularProperties()
        {
            var cpi = new ColumnPropertyInfo(typeof(Foo).GetProperty("Name")!);
            Assert.Equal(DatabaseGeneratedOption.None, cpi.GeneratedOption);
            Assert.False(cpi.IsGenerated);
            Assert.Equal(typeof(Foo).GetProperty("Name"), cpi.Property);
        }

        [Fact]
        public void UsesGeneratedOptionsIdentity_ForKeyProperties()
        {
            var cpi = new ColumnPropertyInfo(typeof(Foo).GetProperty("Id")!, isKey: true);
            Assert.Equal(DatabaseGeneratedOption.Identity, cpi.GeneratedOption);
            Assert.True(cpi.IsGenerated);
            Assert.Equal(typeof(Foo).GetProperty("Id"), cpi.Property);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeterminesDatabaseGeneratedOption_Computed(bool isKey)
        {
            var cpi = new ColumnPropertyInfo(typeof(Bar).GetProperty("Id")!, isKey);
            Assert.Equal(DatabaseGeneratedOption.Computed, cpi.GeneratedOption);
            Assert.True(cpi.IsGenerated);
            Assert.Equal(typeof(Bar).GetProperty("Id"), cpi.Property);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeterminesDatabaseGeneratedOption_None(bool isKey)
        {
            var cpi = new ColumnPropertyInfo(typeof(Baz).GetProperty("Id")!, isKey);
            Assert.Equal(DatabaseGeneratedOption.None, cpi.GeneratedOption);
            Assert.False(cpi.IsGenerated);
            Assert.Equal(typeof(Baz).GetProperty("Id"), cpi.Property);
        }

        [Theory]
        [InlineData(DatabaseGeneratedOption.None)]
        [InlineData(DatabaseGeneratedOption.Identity)]
        [InlineData(DatabaseGeneratedOption.Computed)]
        public void UsesSpecifiedDatabaseGeneratedOption(DatabaseGeneratedOption databaseGeneratedOption)
        {
            var kpi = new ColumnPropertyInfo(typeof(Bar).GetProperty("Id")!, databaseGeneratedOption);
            Assert.Equal(databaseGeneratedOption, kpi.GeneratedOption);
            Assert.Equal(typeof(Bar).GetProperty("Id"), kpi.Property);
        }

        private class Foo
        {
            public int Id { get; set; }

            public string? Name { get; set; }
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
