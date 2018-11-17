using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Moq;
using Moq.Dapper;
using Xunit;

namespace Dommel.Tests
{
    public class ProjectTests
    {
        [Fact]
        public void Project()
        {
            // Arrange
            var queries = new List<string>();
            var expectedQuery = "Project<ProjectedFoo>: select Id, Name, DateUpdated from ProjectedFoos where Id = @Id";
            var mock = new Mock<IDbConnection>();
            mock.SetupDapper(x => x.QueryFirstOrDefault<ProjectedFoo>(It.Is<string>(s => s == expectedQuery), It.IsAny<object>(), null, null, null))
                .Returns(new ProjectedFoo())
                .Verifiable();

            // Act
            DommelMapper.LogReceived += queries.Add;
            var p = mock.Object.Project<ProjectedFoo>(42);
            DommelMapper.LogReceived -= queries.Add;

            // Arrange
            Assert.NotNull(p);
            Assert.NotEmpty(queries);
            mock.Verify();
            Assert.Contains(expectedQuery, queries);
        }

        [Fact]
        public void ProjectAll()
        {
            // Arrange
            var queries = new List<string>();
            var expectedQuery = "ProjectAll<ProjectedFoo>: select Id, Name, DateUpdated from ProjectedFoos";
            var mock = new Mock<IDbConnection>();
            mock.SetupDapper(x => x.Query<ProjectedFoo>(It.Is<string>(s => s == expectedQuery), It.IsAny<object>(), null, true, null, null))
                .Returns(new[] { new ProjectedFoo() })
                .Verifiable();

            // Act
            DommelMapper.LogReceived += queries.Add;
            var p = mock.Object.ProjectAll<ProjectedFoo>();
            DommelMapper.LogReceived -= queries.Add;

            // Arrange
            Assert.Single(p);
            Assert.NotEmpty(queries);
            mock.Verify();
            Assert.Contains(expectedQuery, queries);
        }

        [Fact]
        public void ProjectPaged()
        {
            // Arrange
            var queries = new List<string>();
            var expectedQuery = "ProjectPaged<ProjectedFoo>: select Id, Name, DateUpdated from ProjectedFoos order by Id offset 0 rows fetch next 5 rows only"; ;
            var mock = new Mock<IDbConnection>();
            mock.SetupDapper(x => x.Query<ProjectedFoo>(It.Is<string>(s => s == expectedQuery), It.IsAny<object>(), null, true, null, null))
                .Returns(new[] { new ProjectedFoo() })
                .Verifiable();

            // Act
            DommelMapper.LogReceived += queries.Add;
            var p = mock.Object.ProjectPaged<ProjectedFoo>(pageNumber: 1, pageSize: 5);
            DommelMapper.LogReceived -= queries.Add;

            // Arrange
            Assert.Single(p);
            Assert.NotEmpty(queries);
            mock.Verify();
            Assert.Equal(expectedQuery, queries.Last());
            Assert.Contains(expectedQuery, queries);
        }

        public class ProjectedFoo
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime? DateUpdated { get; set; }
        }
    }
}
