﻿using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Represents a typed SQL expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public class SqlExpression<TEntity>
        {
            private readonly StringBuilder _whereBuilder = new StringBuilder();
            private readonly DynamicParameters _parameters = new DynamicParameters();
            private int _parameterIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlExpression{TEntity}"/>
            /// class using the specified <see cref="ISqlBuilder"/>.
            /// </summary>
            /// <param name="sqlBuilder">The <see cref="ISqlBuilder"/> instance.</param>
            public SqlExpression(ISqlBuilder sqlBuilder)
            {
                SqlBuilder = sqlBuilder;
            }

            /// <summary>
            /// Gets the <see cref="ISqlBuilder"/> instance used to build queries with.
            /// </summary>
            public ISqlBuilder SqlBuilder { get; }

            /// <summary>
            /// Builds a SQL expression for the specified filter expression.
            /// </summary>
            /// <param name="expression">The filter expression on the entity.</param>
            /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
            public virtual SqlExpression<TEntity> Where(Expression<Func<TEntity, bool>> expression)
            {
                if (expression != null)
                {
                    AppendToWhere("and", expression);
                }
                return this;
            }

            private void AppendToWhere(string conditionOperator, Expression expression)
            {
                var sqlExpression = VisitExpression(expression).ToString();
                AppendToWhere(conditionOperator, sqlExpression);
            }

            private void AppendToWhere(string conditionOperator, string sqlExpression)
            {
                if (_whereBuilder.Length == 0)
                {
                    _whereBuilder.Append(" where ");
                }
                else
                {
                    _whereBuilder.AppendFormat(" {0} ", conditionOperator);
                }

                _whereBuilder.Append(sqlExpression);
            }

            /// <summary>
            /// Visits the expression.
            /// </summary>
            /// <param name="expression">The expression to visit.</param>
            /// <returns>The result of the visit.</returns>
            protected virtual object VisitExpression(Expression expression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Lambda:
                        return VisitLambda(expression as LambdaExpression);

                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        return VisitBinary((BinaryExpression)expression);

                    case ExpressionType.Convert:
                    case ExpressionType.Not:
                        return VisitUnary((UnaryExpression)expression);

                    case ExpressionType.New:
                        return VisitNew((NewExpression)expression);

                    case ExpressionType.MemberAccess:
                        return VisitMemberAccess((MemberExpression)expression);

                    case ExpressionType.Constant:
                        return VisitConstantExpression((ConstantExpression)expression);
                    case ExpressionType.Call:
                        return VisitCallExpression((MethodCallExpression)expression);
                    case ExpressionType.Invoke:
                        return VisitExpression(((InvocationExpression)expression).Expression);
                }

                return expression;
            }

            /// <summary>
            /// Specifies the type of text search to use.
            /// </summary>
            protected enum TextSearch
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
                        //check if the method is instance method and caller is type of string.
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
                // The method is a static method, and has 2 arguments.
                // usually, it's from System.Linq.Enumerable
                if (expression.Object == null && expression.Arguments.Count == 2)
                {
                    collection = expression.Arguments[0];
                    property = expression.Arguments[1];
                }
                // The method is an instance method, and has only 1 argument.
                // usually, it's from System.Collections.IList
                else if (expression.Object != null && expression.Arguments.Count == 1)
                {
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
                var column = MemberToColumn((MemberExpression)expression.Object);

                var value = VisitConstantExpression((ConstantExpression)expression.Arguments.FirstOrDefault());
                var textLike = "";

                switch (textSearch)
                {
                    case TextSearch.Contains:
                        textLike = $"%{value}%";
                        break;
                    case TextSearch.StartsWith:
                        textLike = $"{value}%";
                        break;
                    case TextSearch.EndsWith:
                        textLike = $"%{value}";
                        break;
                }

                AddParameter(textLike, out var paramName);
                return $"{column} like {paramName}";
            }

            /// <summary>
            /// Processes a lambda expression.
            /// </summary>
            /// <param name="epxression">The lambda expression.</param>
            /// <returns>The result of the processing.</returns>
            protected virtual object VisitLambda(LambdaExpression epxression)
            {
                if (epxression.Body.NodeType == ExpressionType.MemberAccess)
                {
                    var member = epxression.Body as MemberExpression;
                    if (member?.Expression != null)
                    {
                        return $"{VisitMemberAccess(member)} = '1'";
                    }
                }

                return VisitExpression(epxression.Body);
            }

            /// <summary>
            /// Processes a binary expression.
            /// </summary>
            /// <param name="expression">The binary expression.</param>
            /// <returns>The result of the processing.</returns>
            protected virtual object VisitBinary(BinaryExpression expression)
            {
                object left, right;
                var operand = BindOperant(expression.NodeType);
                if (operand == "and" || operand == "or")
                {
                    // Left side.
                    if (expression.Left is MemberExpression member &&
                        member.Expression != null &&
                        member.Expression.NodeType == ExpressionType.Parameter)
                    {
                        left = $"{VisitMemberAccess(member)} = '1'";
                    }
                    else
                    {
                        left = VisitExpression(expression.Left);
                    }

                    // Right side.
                    member = expression.Right as MemberExpression;
                    if (member != null &&
                        member.Expression != null &&
                        member.Expression.NodeType == ExpressionType.Parameter)
                    {
                        right = $"{VisitMemberAccess(member)} = '1'";
                    }
                    else
                    {
                        right = VisitExpression(expression.Right);
                    }
                }
                else
                {
                    // It's a single expression.
                    left = VisitExpression(expression.Left);
                    right = VisitExpression(expression.Right);

                    AddParameter(right, out var paramName);
                    return $"{left} {operand} {paramName}";
                }

                return $"{left} {operand} {right}";
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
                        if (!(o is string))
                        {
                            return !(bool)o;
                        }

                        var memberExpression = expression.Operand as MemberExpression;
                        if (memberExpression != null &&
                            Resolvers.Properties(memberExpression.Expression.Type).Any(p => p.Name == (string)o))
                        {
                            o = $"{o} = '1'";
                        }

                        return $"not ({o})";
                    case ExpressionType.Convert:
                        if (expression.Method != null)
                        {
                            return Expression.Lambda(expression).Compile().DynamicInvoke();
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
            protected virtual object VisitMemberAccess(MemberExpression expression)
            {
                if (expression.Expression != null && expression.Expression.NodeType == ExpressionType.Parameter)
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
            protected virtual object VisitConstantExpression(ConstantExpression expression) => expression.Value ?? "null";

            /// <summary>
            /// Proccesses a member expression.
            /// </summary>
            /// <param name="expression">The member expression.</param>
            /// <returns>The result of the processing.</returns>
            protected virtual string MemberToColumn(MemberExpression expression) =>
                Resolvers.Column((PropertyInfo)expression.Member, SqlBuilder);

            /// <summary>
            /// Returns the expression operand for the specified expression type.
            /// </summary>
            /// <param name="expressionType">The expression type for node of an expression tree.</param>
            /// <returns>The expression operand equivalent of the <paramref name="expressionType"/>.</returns>
            protected virtual string BindOperant(ExpressionType expressionType)
            {
                switch (expressionType)
                {
                    case ExpressionType.Equal:
                        return "=";
                    case ExpressionType.NotEqual:
                        return "<>";
                    case ExpressionType.GreaterThan:
                        return ">";
                    case ExpressionType.GreaterThanOrEqual:
                        return ">=";
                    case ExpressionType.LessThan:
                        return "<";
                    case ExpressionType.LessThanOrEqual:
                        return "<=";
                    case ExpressionType.AndAlso:
                        return "and";
                    case ExpressionType.OrElse:
                        return "or";
                    case ExpressionType.Add:
                        return "+";
                    case ExpressionType.Subtract:
                        return "-";
                    case ExpressionType.Multiply:
                        return "*";
                    case ExpressionType.Divide:
                        return "/";
                    case ExpressionType.Modulo:
                        return "MOD";
                    case ExpressionType.Coalesce:
                        return "COALESCE";
                    default:
                        return expressionType.ToString();
                }
            }

            /// <summary>
            /// Returns the current SQL query.
            /// </summary>
            /// <returns>The current SQL query.</returns>
            public string ToSql() => _whereBuilder.ToString();

            /// <summary>
            /// Returns the current SQL query.
            /// </summary>
            /// <param name="parameters">When this method returns, contains the parameters for the query.</param>
            /// <returns>The current SQL query.</returns>
            public string ToSql(out DynamicParameters parameters)
            {
                parameters = _parameters;
                return _whereBuilder.ToString();
            }

            /// <summary>
            /// Returns the current SQL query.
            /// </summary>
            /// <returns>The current SQL query.</returns>
            public override string ToString() => _whereBuilder.ToString();

            /// <summary>
            /// Adds a parameter with the specified value to this SQL expression.
            /// </summary>
            /// <param name="value">The value of the parameter.</param>
            /// <param name="paramName">When this method returns, contains the generated parameter name.</param>
            public virtual void AddParameter(object value, out string paramName)
            {
                _parameterIndex++;
                paramName = SqlBuilder.PrefixParameter($"p{_parameterIndex}");
                _parameters.Add(paramName, value: value);
            }
        }
    }
}
