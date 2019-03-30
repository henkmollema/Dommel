using System;
using System.Data;
using Xunit;

namespace Dommel.Tests
{
    public class ProjectTests
    {
        private static readonly IDbConnection DbConnection = new BarDbConnection();

        [Fact]
        public void ProjectById()
        {
            var sql = DommelMapper.BuildProjectById(DbConnection, typeof(ProjectedFoo), 42, out var parameters);
            Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos] where [Id] = @Id", sql);
            Assert.NotNull(parameters);
        }

        [Fact]
        public void ProjectAll()
        {
            var sql = DommelMapper.BuildProjectAllQuery(DbConnection, typeof(ProjectedFoo));
            Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos]", sql);
        }

        [Fact]
        public void ProjectPaged()
        {
            var sql = DommelMapper.BuildProjectPagedQuery(DbConnection, typeof(ProjectedFoo), 1, 5);
            Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos] order by [Id] offset 0 rows fetch next 5 rows only", sql);
        }

        public class ProjectedFoo
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime? DateUpdated { get; set; }
        }
    }
}
