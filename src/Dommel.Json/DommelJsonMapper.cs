using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;

namespace Dommel.Json;

/// <summary>
/// Extensions to configure JSON support for Dommel.
/// </summary>
public static class DommelJsonMapper
{
    /// <summary>
    /// Configures Dommel JSON support on the entities in the specified <paramref name="assemblies"/>.
    /// </summary>
    /// <param name="assemblies">The assembly to scan</param>
    public static void AddJson(params Assembly[] assemblies) => AddJson(new DommelJsonOptions { EntityAssemblies = assemblies });

    /// <summary>
    /// Configures Dommel JSON support using the specified <see cref="DommelJsonOptions"/>.
    /// </summary>
    /// <param name="options"></param>
    public static void AddJson(DommelJsonOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }
        if (options.EntityAssemblies == null || options.EntityAssemblies.Length == 0)
        {
            throw new ArgumentException("No entity assemblies specified.", nameof(options));
        }

        // Add SQL builders with JSON value support
        DommelMapper.AddSqlBuilder("sqlconnection", new SqlServerSqlBuilder());
        DommelMapper.AddSqlBuilder("sqlceconnection", new SqlServerCeSqlBuilder());
        DommelMapper.AddSqlBuilder("sqliteconnection", new SqliteSqlBuilder());
        DommelMapper.AddSqlBuilder("npgsqlconnection", new PostgresSqlBuilder());
        DommelMapper.AddSqlBuilder("mysqlconnection", new MySqlSqlBuilder());

        // Add a custom SqlExpression<T> factory with JSON support
        DommelMapper.SqlExpressionFactory = (type, sqlBuilder) =>
        {
            if (!(sqlBuilder is IJsonSqlBuilder))
            {
                throw new InvalidOperationException($"The specified SQL builder type should be assignable from {nameof(IJsonSqlBuilder)}.");
            }

            var sqlExpression = typeof(JsonSqlExpression<>).MakeGenericType(type);
            return Activator.CreateInstance(sqlExpression, sqlBuilder, options);
        };

        // Add a Dapper type mapper with JSON support for
        // properties annotated with the [JsonData] attribute.
        var jsonTypeHander = options.JsonTypeHandler?.Invoke() ?? new JsonObjectTypeHandler();
        var jsonTypes = new List<Type>();
        foreach (var assembly in options.EntityAssemblies)
        {
            foreach (var type in assembly.ExportedTypes)
            {
                foreach (var property in type.GetRuntimeProperties())
                {
                    var jsonDataAttr = property.GetCustomAttribute(options.JsonDataAttributeType);
                    if (jsonDataAttr != null)
                    {
                        SqlMapper.AddTypeHandler(property.PropertyType, jsonTypeHander);
                        jsonTypes.Add(property.PropertyType);
                    }
                }
            }
        }

        // Set a property resolver which considers the types discovered above
        // as primitive types so they will be used in insert and update queries.
        DommelMapper.SetPropertyResolver(new JsonPropertyResolver(jsonTypes));
    }
}
