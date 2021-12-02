using System.Linq.Expressions;
using System.Reflection;

namespace Dommel.Json;

internal class JsonSqlExpression<T> : SqlExpression<T>
    where T : class
{
    private readonly DommelJsonOptions _options;

    public JsonSqlExpression(IJsonSqlBuilder sqlBuilder, DommelJsonOptions options) : base(sqlBuilder)
    {
        _options = options;
    }

    public new IJsonSqlBuilder SqlBuilder => (IJsonSqlBuilder)base.SqlBuilder;

    protected override object VisitMemberAccess(MemberExpression expression)
    {
        if (expression.Member is PropertyInfo jsonValue &&
            expression.Expression is MemberExpression jsonContainerExpr &&
            jsonContainerExpr.Member is PropertyInfo jsonContainer &&
            jsonContainer.IsDefined(_options.JsonDataAttributeType))
        {
            return SqlBuilder.JsonValue(
                VisitMemberAccess(jsonContainerExpr).ToString(),
                ColumnNameResolver.ResolveColumnName(jsonValue));
        }

        return base.VisitMemberAccess(expression);
    }
}
