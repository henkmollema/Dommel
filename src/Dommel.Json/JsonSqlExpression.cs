using System.Linq.Expressions;
using System.Reflection;
using static Dommel.DommelMapper;

namespace Dommel.Json
{
    internal class JsonSqlExpression<T> : SqlExpression<T>
        where T : class
    {
        public JsonSqlExpression(IJsonSqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }

        public new IJsonSqlBuilder SqlBuilder => (IJsonSqlBuilder)base.SqlBuilder;

        protected override object VisitMemberAccess(MemberExpression expression)
        {
            if (expression.Member is PropertyInfo jsonValue &&
                expression.Expression is MemberExpression jsonContainerExpr &&
                jsonContainerExpr.Member is PropertyInfo jsonContainer &&
                jsonContainer.IsDefined(typeof(JsonDataAttribute)))
            {
                return SqlBuilder.JsonValue(
                    VisitMemberAccess(jsonContainerExpr).ToString(),
                    ResolveColumnName(jsonValue));
            }

            return base.VisitMemberAccess(expression);
        }
    }
}
