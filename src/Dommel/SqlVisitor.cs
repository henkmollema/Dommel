using System;
using System.Linq.Expressions;
using System.Text;
using Dapper;

namespace Dommel;

/// <summary>
/// Visitor pattern implementation to visit an expression tree and build a SQL expression.
/// </summary>
public class SqlVisitor(ISqlBuilder sqlBuilder)
{
    private int _parameterIndex;
    private DynamicParameters _parameters = new();

    /// <summary>
    /// Gets the <see cref="ISqlBuilder"/> used to build the SQL expression.
    /// </summary>
    public ISqlBuilder SqlBuilder => sqlBuilder;

    /// <summary>
    /// Gets the parameters created after visiting the SQL expression.
    /// </summary>
    public DynamicParameters Parameters => _parameters;

    /// <summary>
    /// Gets the <see cref="IColumnNameResolver"/> used to resolve column names.
    /// </summary>
    public IColumnNameResolver ColumnNameResolver => DommelMapper.ColumnNameResolver;

    /// <summary>
    /// Visits the expression.
    /// </summary>
    /// <param name="expression">The expression to visit.</param>
    /// <param name="parameters"></param>
    /// <returns>The result of the visit.</returns>
    public virtual object VisitExpression(Expression expression, out DynamicParameters parameters)
    {
        parameters = _parameters = new();
        return VisitExpression(expression);
    }

    /// <summary>
    /// Visits the expression.
    /// </summary>
    /// <param name="expression">The expression to visit.</param>
    /// <returns>The result of the visit.</returns>
    public virtual object VisitExpression(Expression expression) => expression.NodeType switch
    {
        ExpressionType.Lambda => VisitLambda((LambdaExpression)expression),
        ExpressionType.LessThan or ExpressionType.LessThanOrEqual or ExpressionType.GreaterThan or
        ExpressionType.GreaterThanOrEqual or ExpressionType.Equal or ExpressionType.NotEqual or
        ExpressionType.And or ExpressionType.AndAlso or ExpressionType.Or or ExpressionType.OrElse or
        ExpressionType.Add or ExpressionType.Subtract or ExpressionType.Multiply or ExpressionType.Divide
            => VisitBinary((BinaryExpression)expression),
        ExpressionType.Convert or ExpressionType.Not => VisitUnary((UnaryExpression)expression),
        ExpressionType.New => VisitNew((NewExpression)expression),
        ExpressionType.MemberAccess => VisitMemberAccess((MemberExpression)expression),
        ExpressionType.Constant => VisitConstantExpression((ConstantExpression)expression),
        ExpressionType.Call => VisitCallExpression((MethodCallExpression)expression),
        ExpressionType.Invoke => VisitExpression(((InvocationExpression)expression).Expression),
        _ => expression,
    };

    /// <summary>
    /// Process a method call expression.
    /// </summary>
    /// <param name="expression">The method call expression.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual object VisitCallExpression(MethodCallExpression expression)
    {
        var method = expression.Method.Name.ToLower();
        switch (method)
        {
            case "contains":
                // Is this a string-contains or array-contains expression?
                if (expression.Object != null && expression.Object.Type == typeof(string))
                {
                    return VisitContainsExpression(expression, TextSearch.Contains);
                }
                else
                {
                    return VisitInExpression(expression);
                }
            case "startswith":
                return VisitContainsExpression(expression, TextSearch.StartsWith);
            case "endswith":
                return VisitContainsExpression(expression, TextSearch.EndsWith);
            case "tostring":
                return VisitToStringExpression(expression);
            default:
                break;
        }

        return expression;
    }

    /// <summary>
    /// Processes a contains expression as IN clause
    /// </summary>
    /// <param name="expression">The method call expression.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual object VisitInExpression(MethodCallExpression expression)
    {
        Expression collection;
        Expression property;
        if (expression.Object == null && expression.Arguments.Count == 2)
        {
            // The method is a static method, and has 2 arguments.
            // usually, it's from System.Linq.Enumerable
            collection = expression.Arguments[0];
            property = expression.Arguments[1];
        }
        else if (expression.Object != null && expression.Arguments.Count == 1)
        {
            // The method is an instance method, and has only 1 argument.
            // usually, it's from System.Collections.IList
            collection = expression.Object;
            property = expression.Arguments[0];
        }
        else
        {
            throw new Exception("Unsupported method call: " + expression.Method.Name);
        }

        var inClause = new StringBuilder("(");
        foreach (var value in (System.Collections.IEnumerable)VisitMemberAccess((MemberExpression)collection))
        {
            AddParameter(value, out var paramName);
            inClause.Append($"{paramName},");
        }
        if (inClause.Length == 1)
        {
            inClause.Append("null,");
        }
        inClause[inClause.Length - 1] = ')';

        return $"{VisitExpression(property)} in {inClause}";
    }

    /// <summary>
    /// Processes a contains expression for string.
    /// </summary>
    /// <param name="expression">The method call expression.</param>
    /// <param name="textSearch">Type of search.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual object VisitContainsExpression(MethodCallExpression expression, TextSearch textSearch)
    {
        var column = VisitExpression(expression.Object!);
        if (expression.Arguments.Count == 0 || expression.Arguments.Count > 1)
        {
            throw new ArgumentException("Contains-expression should contain exactly one argument.", nameof(expression));
        }

        var value = VisitExpression(expression.Arguments[0]);
        var textLike = textSearch switch
        {
            TextSearch.Contains => $"%{value}%",
            TextSearch.StartsWith => $"{value}%",
            TextSearch.EndsWith => $"%{value}",
            _ => throw new ArgumentOutOfRangeException(nameof(textSearch), $"Invalid TextSearch value '{textSearch}'."),
        };
        AddParameter(textLike, out var paramName);

        return sqlBuilder.LikeExpression(column.ToString()!, paramName);
    }

    /// <summary>
    /// Processes ToString expression to CAST columns into CHAR before comparison.
    /// </summary>
    /// <returns></returns>
    protected virtual object VisitToStringExpression(MethodCallExpression expression)
    {
        var column = VisitExpression(expression.Object!);
        if (expression.Arguments.Count >= 1)
        {
            throw new ArgumentException("ToString-expression should not contain any argument.", nameof(expression));
        }

        return $"CAST({column} AS CHAR)";
    }

    /// <summary>
    /// Processes a lambda expression.
    /// </summary>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual object VisitLambda(LambdaExpression expression)
    {
        if (expression.Body.NodeType == ExpressionType.MemberAccess)
        {
            var member = expression.Body as MemberExpression;
            if (member?.Expression != null)
            {
                return $"{VisitMemberAccess(member)} = '1'";
            }
        }

        return VisitExpression(expression.Body);
    }

    private static bool IsAndOr(ExpressionType expressionType) => expressionType is ExpressionType.AndAlso or ExpressionType.OrElse;

    /// <summary>
    /// Processes a binary expression.
    /// </summary>
    /// <param name="expression">The binary expression.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual object VisitBinary(BinaryExpression expression)
    {
        object left, right;
        var operand = GetOperant(expression.NodeType);
        if (IsAndOr(expression.NodeType))
        {
            // Process left and right side of the "and/or" expression, e.g.:
            // Foo == 42    or      Bar == 42
            //   left    operand     right

            if (expression.Left is MemberExpression leftMember && leftMember.Expression?.NodeType == ExpressionType.Parameter)
            {
                left = $"{VisitMemberAccess(leftMember)} = '1'";
            }
            else
            {
                left = VisitExpression(expression.Left);
                if (IsAndOr(expression.Left.NodeType) && expression.Left.NodeType != expression.NodeType)
                {
                    // Wrap left expression in parentheses when that side is an and/or expression and the current expression is the opposite
                    left = $"({left})";
                }
            }

            if (expression.Right is MemberExpression rightMember && rightMember.Expression?.NodeType == ExpressionType.Parameter)
            {
                right = $"{VisitMemberAccess(rightMember)} = '1'";
            }
            else
            {
                right = VisitExpression(expression.Right);
                if (IsAndOr(expression.Right.NodeType) && expression.Right.NodeType != expression.NodeType)
                {
                    // Wrap right expression in parentheses when that side is an and/or expression and the current expression is the opposite
                    right = $"({right})";
                }
            }

            return $"{left} {operand} {right}";
        }
        else
        {
            // It's a single expression, e.g. Foo == 42
            left = VisitExpression(expression.Left);
            right = VisitExpression(expression.Right);

            if (right == null)
            {
                // Special case 'is (not) null' syntax
                if (expression.NodeType == ExpressionType.Equal)
                {
                    return $"{left} is null";
                }
                else
                {
                    return $"{left} is not null";
                }
            }

            AddParameter(right, out var paramName);
            return $"{left} {operand} {paramName}";
        }
    }

    /// <summary>
    /// Processes a unary expression.
    /// </summary>
    /// <param name="expression">The unary expression.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual object VisitUnary(UnaryExpression expression)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.Not:
                var o = VisitExpression(expression.Operand);
                if (o is not string)
                {
                    return !(bool)o;
                }

                if (expression.Operand is MemberExpression)
                {
                    o = $"{o} = '1'";
                }

                return $"not ({o})";
            case ExpressionType.Convert:
                if (expression.Method != null)
                {
                    return Expression.Lambda(expression).Compile()!.DynamicInvoke()!;
                }
                break;
        }

        return VisitExpression(expression.Operand);
    }

    /// <summary>
    /// Processes a new expression.
    /// </summary>
    /// <param name="expression">The new expression.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual object VisitNew(NewExpression expression)
    {
        var member = Expression.Convert(expression, typeof(object));
        var lambda = Expression.Lambda<Func<object>>(member);
        var getter = lambda.Compile();
        return getter();
    }

    /// <summary>
    /// Processes a member access expression.
    /// </summary>
    /// <param name="expression">The member access expression.</param>
    /// <returns>The result of the processing.</returns>
    public virtual object VisitMemberAccess(MemberExpression expression)
    {
        if (expression.Expression?.NodeType == ExpressionType.Parameter)
        {
            return MemberToColumn(expression);
        }

        var member = Expression.Convert(expression, typeof(object));
        var lambda = Expression.Lambda<Func<object>>(member);
        var getter = lambda.Compile();
        return getter();
    }

    /// <summary>
    /// Processes a constant expression.
    /// </summary>
    /// <param name="expression">The constant expression.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual object VisitConstantExpression(ConstantExpression expression) => expression.Value!;

    /// <summary>
    /// Processes a member expression.
    /// </summary>
    /// <param name="expression">The member expression.</param>
    /// <returns>The result of the processing.</returns>
    protected virtual string MemberToColumn(MemberExpression expression) =>
        Resolvers.Column(expression.Expression!.Type.GetProperty(expression.Member.Name)!, sqlBuilder);

    /// <summary>
    /// Returns the expression operant for the specified expression type.
    /// </summary>
    /// <param name="expressionType">The expression type for node of an expression tree.</param>
    /// <returns>The expression operand equivalent of the <paramref name="expressionType"/>.</returns>
    protected virtual string GetOperant(ExpressionType expressionType) => expressionType switch
    {
        ExpressionType.Equal => "=",
        ExpressionType.NotEqual => "<>",
        ExpressionType.GreaterThan => ">",
        ExpressionType.GreaterThanOrEqual => ">=",
        ExpressionType.LessThan => "<",
        ExpressionType.LessThanOrEqual => "<=",
        ExpressionType.AndAlso => "and",
        ExpressionType.OrElse => "or",
        ExpressionType.Add => "+",
        ExpressionType.Subtract => "-",
        ExpressionType.Multiply => "*",
        ExpressionType.Divide => "/",
        ExpressionType.Modulo => "MOD",
        ExpressionType.Coalesce => "COALESCE",
        _ => expressionType.ToString(),
    };

    /// <summary>
    /// Specifies the type of text search to use.
    /// </summary>
    public enum TextSearch
    {
        /// <summary>
        /// Matches anywhere in a string.
        /// </summary>
        Contains,

        /// <summary>
        /// Matches the start of a string.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Matches the end of a string.
        /// </summary>
        EndsWith
    }

    /// <summary>
    /// Adds a parameter with the specified value to this SQL expression.
    /// </summary>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="paramName">When this method returns, contains the generated parameter name.</param>
    public virtual void AddParameter(object value, out string paramName)
    {
        _parameterIndex++;
        paramName = sqlBuilder.PrefixParameter($"p{_parameterIndex}");
        _parameters.Add(paramName, value: value);
    }
}
