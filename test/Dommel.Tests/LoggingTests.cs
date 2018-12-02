using Dapper;
using Moq;
using Moq.Dapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Xunit;

namespace Dommel.Tests
{
    [Collection("Use Dommel Log to check on results")]
    public class LoggingTests
    {
        [Fact]
        public void GetLogsSql()
        {
            // Arrange
            var logs = new List<string>();
            var mock = new Mock<IDbConnection>();
            mock.SetupDapper(x => x.QueryFirstOrDefault<Foo>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .Returns(new Foo());

            // Initialize resolver caches so these messages are not logged
            mock.Object.Get<Foo>(1);
            DommelMapper.LogReceived = s => logs.Add(s);

            // Act
            mock.Object.Get<Foo>(1);

            // Assert
            Assert.Single(logs);
            Assert.Equal("Get<Foo>: select * from [Foos] where [Id] = @Id", logs[0]);
        }

        [Fact]
        public void GetByIdsLogsSql()
        {
            // Arrange
            var logs = new List<string>();
            var mock = new Mock<IDbConnection>();
            mock.SetupDapper(x => x.QueryFirstOrDefault<Foo>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .Returns(new Foo());

            // Initialize resolver caches so these messages are not logged
            mock.Object.Get<Bar>("key1", "key2", "key3");
            DommelMapper.LogReceived = s => logs.Add(s);

            // Act
            mock.Object.Get<Bar>("key1", "key2", "key3");

            // Assert
            Assert.Single(logs);
            Assert.Equal("Get<Bar>: select * from [Bars] where [Id] = @Id0 and [KeyColumn2] = @Id1 and [KeyColumn3] = @Id2", logs[0]);
        }

        [Fact]
        public void GetTwiceLogsSqlTwice()
        {
            // Arrange
            var logs = new List<string>();
            var mock = new Mock<IDbConnection>();
            mock.SetupDapper(x => x.QueryFirstOrDefault<Foo>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .Returns(new Foo());

            // Initialize resolver caches so these messages are not logged
            mock.Object.Get<Foo>(1);
            DommelMapper.LogReceived = s => logs.Add(s);

            // Act
            mock.Object.Get<Foo>(1);
            mock.Object.Get<Foo>(1);

            // Assert
            Assert.Equal(2, logs.Count);
            Assert.Equal("Get<Foo>: select * from [Foos] where [Id] = @Id", logs[0]);
            Assert.Equal("Get<Foo>: select * from [Foos] where [Id] = @Id", logs[1]);
        }

        [Fact]
        public void GetBuilderLogsChosenBuilder()
        {
            // Arrange
            var logs = new List<string>();
            var mock = new Mock<IDbConnection>();
            DommelMapper.LogReceived = s => logs.Add(s);

            // Act
            DommelMapper.GetSqlBuilder(mock.Object);

            // Assert
            Assert.True(logs.Count > 0);
            Assert.Contains("Selected SQL Builder 'SqlServerSqlBuilder' for connection type 'IDbConnectionProxy'", logs);
        }
    }

    public class Foo
    {
        public int Id { get; set; }
    }

    public class Bar
    {
        public string Id { get; set; }

        [Key]
        public string KeyColumn2 {get;set;}

        [Key]
        public string KeyColumn3 {get;set;}
    }
}
