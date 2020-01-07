using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Dapper;
using Xunit;

namespace Dommel.Json.Tests
{
    public class DommelJsonMapperTests
    {
        public DommelJsonMapperTests()
        {
            DommelJsonMapper.AddJson(typeof(Lead).Assembly);
        }

        [Fact]
        public void AddsJsonPropertyResolver()
        {
            var propertyResolver = Assert.IsType<JsonPropertyResolver>(DommelMapper.PropertyResolver);
            Assert.Equal(typeof(LeadData), Assert.Single(propertyResolver.JsonTypes));
        }

        [Fact]
        public void AddsJsonSqlBuilders()
        {
            Assert.IsType<SqlServerSqlBuilder>(DommelMapper.SqlBuilders["sqlconnection"]);
            Assert.IsType<SqlServerCeSqlBuilder>(DommelMapper.SqlBuilders["sqlceconnection"]);
            Assert.IsType<SqliteSqlBuilder>(DommelMapper.SqlBuilders["sqliteconnection"]);
            Assert.IsType<PostgresSqlBuilder>(DommelMapper.SqlBuilders["npgsqlconnection"]);
            Assert.IsType<MySqlSqlBuilder>(DommelMapper.SqlBuilders["mysqlconnection"]);
        }

        [Fact]
        public void AddsCustomSqlExpressionFactory()
        {
            Assert.IsType<JsonSqlExpression<Lead>>(
                DommelMapper.SqlExpressionFactory.Invoke(typeof(Lead), new MySqlSqlBuilder()));
        }

        [Fact]
        public void ThrowsWhenNotJsonSqlBuilder()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => DommelMapper.SqlExpressionFactory.Invoke(typeof(Lead), new Dommel.MySqlSqlBuilder()));
            Assert.Equal($"The specified SQL builder type should be assignable from {nameof(IJsonSqlBuilder)}.", ex.Message);
        }


        [Fact]
        public void AddsDapperTypeHandler()
        {
            var typeHandlers = GetDapperTypeHandlers();
            Assert.Contains(typeHandlers, kvp => kvp.Value is JsonObjectTypeHandler);
        }

        // Dirty hack to determine whether the Dapper type handler has been added
        private static Dictionary<Type, SqlMapper.ITypeHandler> GetDapperTypeHandlers() =>
            (Dictionary<Type, SqlMapper.ITypeHandler>)typeof(SqlMapper).GetField("typeHandlers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

        [Fact]
        public void AddsCustomJsonTypeHandler()
        {
            // Act
            DommelJsonMapper.AddJson(new DommelJsonOptions
            {
                EntityAssemblies = new[] { typeof(Lead).Assembly },
                JsonTypeHandler = () => new CustomJsonTypeHandler()
            });

            // Assert
            var typeHandlers = GetDapperTypeHandlers();
            Assert.Contains(typeHandlers, kvp => kvp.Value is CustomJsonTypeHandler);

        }

        private class CustomJsonTypeHandler : SqlMapper.ITypeHandler
        {
            public object Parse(Type destinationType, object value) => throw new NotImplementedException();
            public void SetValue(IDbDataParameter parameter, object value) => throw new NotImplementedException();
        }

        [Fact]
        public void ThrowsWhenNullEntityAssemblies()
        {
            var ex = Assert.Throws<ArgumentException>("options", () => DommelJsonMapper.AddJson(new DommelJsonOptions()));
            Assert.Equal(new ArgumentException("No entity assemblies specified.", "options").Message, ex.Message);
        }

        [Fact]
        public void ThrowsWhenEmptyEntityAssemblies()
        {
            var ex = Assert.Throws<ArgumentException>("options", () => DommelJsonMapper.AddJson(new DommelJsonOptions { EntityAssemblies = new Assembly[0] }));
            Assert.Equal(new ArgumentException("No entity assemblies specified.", "options").Message, ex.Message);
        }
    }
}
