using System;
using System.Data;
using Xunit;
using static Dommel.DommelMapper;

namespace Dommel.Tests
{
    public class ProjectTests
    {
        private static readonly ISqlBuilder SqlBuilder = new SqlServerSqlBuilder();

        [Fact]
        public void ProjectById()
        {
            var sql = BuildProjectById(SqlBuilder, typeof(ProjectedFoo), 42, out var parameters);
            Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos] where [Id] = @Id", sql);
            Assert.NotNull(parameters);
        }

        [Fact]
        public void ProjectAll()
        {
            var sql = BuildProjectAllQuery(SqlBuilder, typeof(ProjectedFoo));
            Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos]", sql);
        }

        [Fact]
        public void ProjectPaged()
        {
            var sql = BuildProjectPagedQuery(SqlBuilder, typeof(ProjectedFoo), 1, 5);
            Assert.Equal("select [Id], [Name], [DateUpdated] from [ProjectedFoos] order by [Id] offset 0 rows fetch next 5 rows only", sql);
        }

        public class ProjectedFoo
        {
            public int Id { get; set; }

            public string? Name { get; set; }

            public DateTime? DateUpdated { get; set; }
        }
    }
}
