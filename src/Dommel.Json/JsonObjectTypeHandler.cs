using System;
using System.Data;
using System.Text.Json;
using Dapper;

namespace Dommel.Json
{
    internal class JsonObjectTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = value == null || value is DBNull
                ? (object)DBNull.Value
                : JsonSerializer.Serialize(value);
            parameter.DbType = DbType.String;
        }

        public object Parse(Type destinationType, object value) =>
            value is string str ? JsonSerializer.Deserialize(str, destinationType) : null;
    }
}
