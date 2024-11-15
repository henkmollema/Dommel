using System.Linq.Expressions;
using System.Reflection;

namespace Dommel.Json;

internal class JsonSqlVisitor : SqlVisitor
{
    private readonly DommelJsonOptions _options;

    public JsonSqlVisitor(IJsonSqlBuilder sqlBuilder, DommelJsonOptions options) : base(sqlBuilder)
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
                VisitMemberAccess(jsonContainerExpr).ToString()!,
                ColumnNameResolver.ResolveColumnName(jsonValue));
        }

        return base.VisitMemberAccess(expression);
    }
}
