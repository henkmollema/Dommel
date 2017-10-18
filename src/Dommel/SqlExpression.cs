using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dommel
{
    /// <summary>
    /// Represents a typed SQL expression.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class SqlExpression<TEntity>
    {
        private readonly StringBuilder _whereBuilder = new StringBuilder();
        private readonly DynamicParameters _parameters = new DynamicParameters();
        private readonly StringBuilder _orderBy = new StringBuilder();
        private readonly StringBuilder _paginationOffset = new StringBuilder();
        private int _parameterIndex;

        /// <summary>
        /// Builds a SQL expression for the specified filter expression.
        /// </summary>
        /// <param name="expression">The filter expression on the entity.</param>
        /// <returns>The current <see cref="DommelMapper.SqlExpression&lt;TEntity&gt;"/> instance.</returns>
        public virtual SqlExpression<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            if (expression != null)
            {
                AppendToWhere("and", expression);
            }
            return this;
        }

        /// <summary>
        /// Builds a SQL expression for the specified filter expression.
        /// </summary>
        /// <param name="expression">The filter expression on the entity.</param>
        /// <param name="orderByAsc">Order by type.</param>
        /// <returns></returns>
        public SqlExpression<TEntity> BuildOrderSql(IList<SortField> sortFields)
        {
            if (sortFields == null || sortFields.Count() == 0) return this;

            _orderBy.Append("ORDER BY ");
            foreach (var sortField in sortFields)
            {
                _orderBy.Append($"{Resolvers.Table(typeof(TEntity))}.{Resolvers.Column(typeof(TEntity), sortField.FieldName)} {sortField.Direction}");
                if (sortFields.Count() > 1)
                {
                    _orderBy.Append(",");
                }
            }
            return this;
        }

        public SqlExpression<TEntity> BuildPaginationOffset(int page = 0, int limit = 0)
        {
            if (page == 0 || limit == 0) return this;
            var offset = (page - 1) * limit;
            _paginationOffset.Append($"OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY");
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
                    var m = expression as MethodCallExpression;
                    if (m.Object != null && m.Object.Type != typeof(string) && m.Method.Name == "Contains" && m.Arguments.Count == 1)
                    {
                        // We are going to assume this is an IEnumerable and evaluate the contains as an IN at this point.
                        var args = m.Arguments;
                        var columnName = VisitExpression(m.Arguments[0]);
                        var values = VisitMemberAccess((MemberExpression)m.Object);

                        var sql = $"{columnName} IN ({CreateInParamSql((IEnumerable)values)})";
                        return sql;
                    }
                    if (m.Object != null && m.Object.Type == typeof(string) && m.Method.Name == "Contains" && m.Arguments.Count == 1)
                    {
                        // We are going to assume this is an IEnumerable and evaluate the contains as an IN at this point.
                        var args = m.Arguments;
                        var value = VisitExpression(m.Arguments[0]);
                        var columnName = VisitMemberAccess((MemberExpression)m.Object);

                        var sql = $"{columnName} LIKE '%{value}%'";
                        return sql;
                    }
                    if (m.Object == null && m.Method.Name == "Contains" && m.Arguments.Count == 2)
                    {
                        var args = m.Arguments;
                        var values = VisitMemberAccess((MemberExpression)args[0]);
                        var columnName = VisitExpression(args[1]);

                        var sql = $"{columnName} IN ({CreateInParamSql((IEnumerable)values)})";
                        return sql;
                    }
                    return expression;
            }

            return expression;
        }

        private string CreateInParamSql(IEnumerable values)
        {
            var sbParams = new StringBuilder();
            foreach (var item in values)
            {
                if (sbParams.Length > 0)
                    sbParams.Append(",");

                var paramName = "p" + _parameterIndex++;
                _parameters.Add(paramName, value: item);

                sbParams.Append($"@{paramName}");
            }
            return sbParams.ToString();
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
                var member = expression.Left as MemberExpression;
                if (member != null &&
                    member.Expression != null &&
                    member.Expression.NodeType == ExpressionType.Parameter)
                {
                    left = $"{VisitMemberAccess(member)} = '1'";
                }
                else
                {
                    left = $"({VisitExpression(expression.Left)})";
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
                    right = $"({VisitExpression(expression.Right)})";
                }
            }
            else
            {
                // It's a single expression.
                left = VisitExpression(expression.Left);
                right = VisitExpression(expression.Right);

                var paramName = "p" + _parameterIndex++;
                _parameters.Add(paramName, value: right);
                return $"{left} {operand} @{paramName}";
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
                        return Expression.Lambda(expression).Compile().DynamicInvoke();

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
                return $"{Resolvers.Table(expression.Expression.Type)}.{Resolvers.Column((PropertyInfo)expression.Member)}";
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
        protected virtual object VisitConstantExpression(ConstantExpression expression)
        {
            return expression.Value ?? "null";
        }

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
        public string ToSql()
        {
            return _whereBuilder.ToString();
        }

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

        public string ToSql(out string orderBy, out string paginationOffset)
        {
            orderBy = _orderBy.ToString().TrimEnd(',');
            paginationOffset = _paginationOffset.ToString();
            return _whereBuilder.ToString();
        }

        public string ToSql(out DynamicParameters parameters, out string orderBy, out string paginationOffset)
        {
            parameters = _parameters;
            orderBy = _orderBy.ToString().TrimEnd(',');
            paginationOffset = _paginationOffset.ToString();
            return _whereBuilder.ToString();
        }

        /// <summary>
        /// Returns the current SQL query.
        /// </summary>
        /// <returns>The current SQL query.</returns>
        public override string ToString()
        {
            return _whereBuilder.ToString();
        }
    }
}
