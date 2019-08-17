using System;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace Dommel.Json
{
    internal class JsonObjectTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.Value = value == null || value is DBNull
                ? (object)DBNull.Value
                : JsonConvert.SerializeObject(value);
            parameter.DbType = DbType.String;
        }

        public object Parse(Type destinationType, object value) =>
            value is string str ? JsonConvert.DeserializeObject(str, destinationType) : null;
    }
}
