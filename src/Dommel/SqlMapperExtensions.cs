using System.Data;

namespace Dommel
{
    public static class SqlMapperExtensions
    {
        public static T Get<T>(this IDbConnection con, object id)
        {
            return default(T);
        }
    }
}
