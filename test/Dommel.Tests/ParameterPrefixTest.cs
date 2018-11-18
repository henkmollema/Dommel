using Dapper;
using Moq;
using Moq.Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using System.Text;
using Xunit;
using static Dommel.DommelMapper;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace Dommel.Tests
{
    [Collection("Use Dommel Log to check on results")]
    public class ParameterPrefixTest
    {
        private readonly Mock<IDataBaseParameterPrefix> mock = new Mock<IDataBaseParameterPrefix>();

        public ParameterPrefixTest()
        {
            mock.As<IDbConnection>().SetupDapper(x => x.QueryFirstOrDefault<FooParameterPrefix>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .Returns(new FooParameterPrefix());

            var connectionType = mock.Object.GetType();

            // Change the default Sql Connection
            DommelMapper.AddSqlBuilder(connectionType, new ExampleBuilderBuilder());
        }

        [Fact]
        public void SqlExpressionDirectAccess()
        {
            var builder = new ExampleBuilderBuilder();
            var sqlExpression = new DommelMapper.SqlExpression<FooParameterPrefix>(builder);

            var expression = sqlExpression.Where(p => p.Bar.Contains("test"));
            var sql = expression.ToSql(out var dynamicParameters);

            Assert.Equal("where Bar like #p1", sql.Trim());
            Assert.Single(dynamicParameters.ParameterNames);
            Assert.Equal("%test%", dynamicParameters.Get<string>("#p1"));
        }

        [Fact]
        public void TestGet()
        {
            var logs = new List<string>();
            // Initialize resolver caches so these messages are not logged
            mock.Object.Get<FooParameterPrefix>(1);

            DommelMapper.LogReceived = s => logs.Add(s);

            // Act
            mock.Object.Get<FooParameterPrefix>(1);

            Assert.Equal("Get<FooParameterPrefix>: select * from tblFoo where Id = #Id", logs[0]);
        }

        [Fact]
        public void TestGetIds()
        {
            List<string> logs = new List<string>();
            // Initialize resolver caches so these messages are not logged
            mock.Object.Get<FooTwoIds>(1, 2);

            DommelMapper.LogReceived = s => logs.Add(s);

            // Act
            mock.Object.Get<FooTwoIds>(1, 2);

            Assert.Equal("Get<FooTwoIds>: select * from tblFooTwoIds where One = #Id0 and Two = #Id1", logs[0]);
        }

        [Fact]
        public void TestDelete()
        {
            List<string> logs = new List<string>();
            // Initialize resolver caches so these messages are not logged
            mock.Object.Delete<FooParameterPrefix>(new FooParameterPrefix { Id = 1 });
            DommelMapper.LogReceived = s => logs.Add(s);

            // Act
            mock.Object.Delete<FooParameterPrefix>(new FooParameterPrefix { Id = 1 });

            Assert.Equal("Delete<FooParameterPrefix>: delete from tblFoo where Id = #Id", logs[0]);
        }

        [Fact]
        public void TestUpdate()
        {
            var logs = new List<string>();
            // Initialize resolver caches so these messages are not logged
            mock.Object.Update<FooParameterPrefix>(new FooParameterPrefix { Id = 1, Bar = "test" });
            DommelMapper.LogReceived = s => logs.Add(s);

            // Act
            mock.Object.Update<FooParameterPrefix>(new FooParameterPrefix { Id = 1, Bar = "test" });

            Assert.Equal("Update<FooParameterPrefix>: update tblFoo set Bar = #Bar where Id = #Id", logs[0]);
        }

        public interface IDataBaseParameterPrefix : IDbConnection { }

        /// <summary>
        /// <see cref="ISqlBuilder"/> implementation for example.
        /// </summary>
        public sealed class ExampleBuilderBuilder : ISqlBuilder
        {
            /// <inheritdoc/>
            public string PrefixParameter(string paramName) => $"#{paramName}";

            /// <inheritdoc/>
            public string QuoteIdentifier(string identifier) => identifier;

            /// <inheritdoc/>
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return $"insert into {tableName} ({string.Join(", ", columnNames)}) values ({string.Join(", ", paramNames)}); select last_insert_rowid() id";
            }

            /// <inheritdoc/>
            public string BuildPaging(string orderBy, int pageNumber, int pageSize)
            {
                var start = pageNumber >= 1 ? (pageNumber - 1) * pageSize : 0;
                return $" {orderBy} LIMIT {start}, {pageSize}";
            }
        }

        [Table("tblFoo")]
        public class FooParameterPrefix
        {
            public int Id { get; set; }

            public string Bar { get; set; }
        }

        [Table("tblFooTwoIds")]
        public class FooTwoIds
        {
            [Key]
            public int One { get; set; }

            [Key]
            public int Two { get; set; }

            public string Bar { get; set; }
        }
    }
}
