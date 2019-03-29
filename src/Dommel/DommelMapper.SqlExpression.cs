using System;
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
            private static readonly Type EntityType = typeof(TEntity);
            private readonly ISqlBuilder _sqlBuilder;
            private readonly StringBuilder _whereBuilder = new StringBuilder();
            private readonly StringBuilder _orderByBuilder = new StringBuilder();
            private readonly DynamicParameters _parameters = new DynamicParameters();
            private string _selectQuery;
            private int _parameterIndex;

            /// <summary>
            /// Initializes a new instance of the <see cref="SqlExpression{TEntity}"/>
            /// class using the specified <see cref="ISqlBuilder"/>.
            /// </summary>
            /// <param name="sqlBuilder">The <see cref="ISqlBuilder"/> instance.</param>
            public SqlExpression(ISqlBuilder sqlBuilder)
            {
                _sqlBuilder = sqlBuilder;
            }

            public virtual SqlExpression<TEntity> Select()
            {
                _selectQuery = $"select * from {Resolvers.Table(typeof(TEntity), _sqlBuilder)}";
                return this;
            }

            public virtual SqlExpression<TEntity> Select(Expression<Func<TEntity, object>> selector)
            {
                if (selector == null)
                {
                    throw new ArgumentNullException(nameof(selector));
                }

                // Create object instance of anonymous type via expressions
                var newFunc = Expression.Lambda<Func<TEntity>>(
                    Expression.New(EntityType.GetConstructors()[0])).Compile();
                var obj = selector.Compile().Invoke(newFunc());

                // Resolve properties of anonymous type
                var props = new DefaultPropertyResolver().ResolveProperties(obj.GetType());
                var columns = props.Select(p => Resolvers.Column(p, _sqlBuilder));

                // Create the select query
                var tableName = Resolvers.Table(EntityType, _sqlBuilder);
                _selectQuery = $"select {string.Join(", ", columns)} from {tableName}";
                return this;
            }

            /// <summary>
            /// Builds a SQL expression for the specified filter expression.
            /// </summary>
            /// <param name="expression">The filter expression on the entity.</param>
            /// <returns>The current <see cref="SqlExpression{TEntity}"/> instance.</returns>
            public virtual SqlExpression<TEntity> Where(Expression<Func<TEntity, bool>> expression)
            {
                if (expression == null)
                {
                    throw new ArgumentNullException(nameof(expression));
                }
                if (_whereBuilder.Length > 0)
                {
                    throw new InvalidOperationException("Where statement already started. Use 'AndWhere' or 'OrWhere' to add additional statements.");
                }

                AppendToWhere(null, expression);
                return this;
            }

            public virtual SqlExpression<TEntity> AndWhere(Expression<Func<TEntity, bool>> expression)
            {
                if (_whereBuilder.Length == 0)
                {
                    throw new InvalidOperationException("Start the where statement with the 'Where' method.");
                }
                if (expression != null)
                {
                    AppendToWhere("and", expression);
                }
                return this;
            }

            public virtual SqlExpression<TEntity> OrWhere(Expression<Func<TEntity, bool>> expression)
            {
                if (_whereBuilder.Length == 0)
                {
                    throw new InvalidOperationException("Start the where statement with the 'Where' method.");
                }
                if (expression != null)
                {
                    AppendToWhere("or", expression);
                }
                return this;
            }

            private void AppendToWhere(string conditionOperator, Expression expression)
            {
                var sqlExpression = VisitExpression(expression).ToString();
                if (_whereBuilder.Length == 0)
                {
                    _whereBuilder.Append(" where ");
                }
                else
                {
                    _whereBuilder.AppendFormat(" {0} ", conditionOperator);
                }

                _whereBuilder.Append("(" + sqlExpression + ")");
            }

            public virtual SqlExpression<TEntity> OrderBy(Expression<Func<TEntity, object>> selector)
            {
                if (selector == null)
                {
                    throw new ArgumentNullException(nameof(selector));
                }
                if (_orderByBuilder.Length > 0)
                {
                    throw new InvalidOperationException("Order statement already started. Use 'ThenBy' or 'ThenByDescending' to add additional order statements.");
                }

                OrderByCore(selector, "asc");
                return this;
            }

            public virtual SqlExpression<TEntity> OrderByDescending(Expression<Func<TEntity, object>> selector)
            {
                if (selector == null)
                {
                    throw new ArgumentNullException(nameof(selector));
                }
                if (_orderByBuilder.Length > 0)
                {
                    throw new InvalidOperationException("Order statement already started. Use 'ThenBy' or 'ThenByDescending' to add additional order statements.");
                }
                OrderByCore(selector, "desc");
                return this;
            }

            public virtual SqlExpression<TEntity> ThenBy(Expression<Func<TEntity, object>> selector)
            {
                if (selector == null)
                {
                    throw new ArgumentNullException(nameof(selector));
                }
                if (_orderByBuilder.Length == 0)
                {
                    throw new InvalidOperationException("Order statement not started. Use 'OrderBy' or 'OrderByDescending' to start the order statement.");
                }
                OrderByCore(selector, "asc");
                return this;
            }

            public virtual SqlExpression<TEntity> ThenByDescending(Expression<Func<TEntity, object>> selector)
            {
                if (selector == null)
                {
                    throw new ArgumentNullException(nameof(selector));
                }
                if (_orderByBuilder.Length == 0)
                {
                    throw new InvalidOperationException("Order statement not started. Use 'OrderBy' or 'OrderByDescending' to start the order statement.");
                }
                OrderByCore(selector, "desc");
                return this;
            }

            private void OrderByCore(Expression<Func<TEntity, object>> selector, string direction)
            {
                var column = VisitExpression(selector.Body);
                if (_orderByBuilder.Length == 0)
                {
                    _orderByBuilder.Append($" order by {column} {direction}");
                }
                else
                {
                    _orderByBuilder.Append($", {column} {direction}");
                }
            }

            //private void OrderByCore(Expression<Func<TEntity, object>> selector, string direction)
            //{
            //    var anonType = selector.Compile().Invoke(new TEntity()).GetType();
            //    if (IsAnonymousType(anonType))
            //    {
            //        var columns = GetColumns(anonType);
            //        _orderByBuilder.Append($" order by {string.Join(", ", columns)} {direction}");
            //    }
            //    else
            //    {
            //        var column = VisitExpression(selector);
            //        _orderByBuilder.Append($" order by {column} {direction}");
            //    }
            //}

            //private static bool IsAnonymousType(Type type)
            //{
            //    return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
            //        && type.IsGenericType && type.Name.Contains("AnonymousType")
            //        && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
            //}

            //private string[] GetColumns(Type type)
            //{
            //    var props = new DefaultPropertyResolver().ResolveProperties(type);
            //    return props.Select(p => Resolvers.Column(p, SqlBuilder)).ToArray();
            //}

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
                        return VisitLambda((LambdaExpression)expression);

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
                if (expression.Arguments.Count == 0 || expression.Arguments.Count > 1)
                {
                    throw new ArgumentException("Contains-expression should contain exatcly one argument.", nameof(expression));
                }

                var value = VisitExpression(expression.Arguments[0]);
                string textLike;
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
                    default:
                        throw new ArgumentOutOfRangeException($"Invalid TextSearch value '{textSearch}'.", nameof(textSearch));
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
                    // Process left and right side of the "and/or" expression, e.g.:
                    // Foo == 42    or      Bar == 42
                    //   left    operand     right
                    //
                    if (expression.Left is MemberExpression leftMember && leftMember.Expression?.NodeType == ExpressionType.Parameter)
                    {
                        left = $"{VisitMemberAccess(leftMember)} = '1'";
                    }
                    else
                    {
                        left = VisitExpression(expression.Left);
                    }

                    if (expression.Right is MemberExpression rightMember && rightMember.Expression?.NodeType == ExpressionType.Parameter)
                    {
                        right = $"{VisitMemberAccess(rightMember)} = '1'";
                    }
                    else
                    {
                        right = VisitExpression(expression.Right);
                    }
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
                        else if (expression.NodeType == ExpressionType.NotEqual)
                        {
                            return $"{left} is not null";
                        }
                    }

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

                        if (expression.Operand is MemberExpression)
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
            protected virtual object VisitConstantExpression(ConstantExpression expression) => expression.Value;

            /// <summary>
            /// Proccesses a member expression.
            /// </summary>
            /// <param name="expression">The member expression.</param>
            /// <returns>The result of the processing.</returns>
            protected virtual string MemberToColumn(MemberExpression expression) =>
                Resolvers.Column((PropertyInfo)expression.Member, _sqlBuilder);

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
            /// Adds a parameter with the specified value to this SQL expression.
            /// </summary>
            /// <param name="value">The value of the parameter.</param>
            /// <param name="paramName">When this method returns, contains the generated parameter name.</param>
            protected virtual void AddParameter(object value, out string paramName)
            {
                _parameterIndex++;
                paramName = _sqlBuilder.PrefixParameter($"p{_parameterIndex}");
                _parameters.Add(paramName, value: value);
            }

            /// <summary>
            /// Returns the current SQL query.
            /// </summary>
            /// <returns>The current SQL query.</returns>
            public string ToSql()
            {
                var where = _whereBuilder.ToString();
                var orderBy = _orderByBuilder.ToString();
                var query = "";
                if (!string.IsNullOrEmpty(_selectQuery))
                {
                    query += _selectQuery;
                }
                if (!string.IsNullOrEmpty(where))
                {
                    query += where;
                }
                if (!string.IsNullOrEmpty(orderBy))
                {
                    query += orderBy;
                }
                return query;
            }

            /// <summary>
            /// Returns the current SQL query.
            /// </summary>
            /// <param name="parameters">When this method returns, contains the parameters for the query.</param>
            /// <returns>The current SQL query.</returns>
            public string ToSql(out DynamicParameters parameters)
            {
                parameters = _parameters;
                return ToSql();
            }

            /// <summary>
            /// Returns the current SQL query.
            /// </summary>
            /// <returns>The current SQL query.</returns>
            public override string ToString() => ToSql();
        }
    }
}
