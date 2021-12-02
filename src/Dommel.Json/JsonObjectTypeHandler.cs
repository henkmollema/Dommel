using System;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapper;

namespace Dommel.Json;

internal class JsonObjectTypeHandler : SqlMapper.ITypeHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public void SetValue(IDbDataParameter parameter, object? value)
    {
        parameter.Value = value is null || value is DBNull
            ? (object)DBNull.Value
            : JsonSerializer.Serialize(value, JsonOptions);
        parameter.DbType = DbType.String;
    }

    public object? Parse(Type destinationType, object? value) =>
        value is string str ? JsonSerializer.Deserialize(str, destinationType, JsonOptions) : null;
}
