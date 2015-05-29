using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;

#if DNXCORE50
using IDbTransaction = System.Data.Common.DbTransaction;
using IDbConnection = System.Data.Common.DbConnection;
#endif

namespace Dommel
{
    /// <summary>
    /// Simple CRUD operations for Dapper.
    /// </summary>
    public static class DommelMapper
    {
        private static readonly IDictionary<string, ISqlBuilder> _sqlBuilders = new Dictionary<string, ISqlBuilder>
                                                                                    {
                                                                                        { "sqlconnection", new SqlServerSqlBuilder() },
                                                                                        { "sqlceconnection", new SqlServerCeSqlBuilder() },
                                                                                        { "sqliteconnection", new SqliteSqlBuilder() },
                                                                                        { "npgsqlconnection", new PostgresSqlBuilder() },
                                                                                        { "mysqlconnection", new MySqlSqlBuilder() }
                                                                                    };

        private static readonly IDictionary<Type, string> _getQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _getAllQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _insertQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _updateQueryCache = new Dictionary<Type, string>();
        private static readonly IDictionary<Type, string> _deleteQueryCache = new Dictionary<Type, string>();

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <returns>The entity with the corresponding id.</returns>
        public static TEntity Get<TEntity>(this IDbConnection connection, object id) where TEntity : class
        {
            var type = typeof (TEntity);

            string sql;
            if (!_getQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var keyColumnName = Resolvers.Column(keyProperty);

                sql = string.Format("select * from {0} where {1} = @Id", tableName, keyColumnName);
                _getQueryCache[type] = sql;
            }

            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            return connection.Query<TEntity>(sql: sql, param: parameters).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, TReturn>(this IDbConnection connection, object id, Func<T1, T2, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, TReturn>(this IDbConnection connection,
                                                       object id,
                                                       Func<T1, T2, T3, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, TReturn>(this IDbConnection connection,
                                                           object id,
                                                           Func<T1, T2, T3, T4, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection,
                                                               object id,
                                                               Func<T1, T2, T3, T4, T5, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection,
                                                                   object id,
                                                                   Func<T1, T2, T3, T4, T5, T6, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the entity of type <typeparamref name="TReturn"/> with the specified id 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="T7">The seventh type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="id">The id of the entity in the database.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <returns>The entity with the corresponding id joined with the specified types.</returns>
        public static TReturn Get<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection,
                                                                       object id,
                                                                       Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map) where TReturn : class
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, id).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>A collection of entities of type <typeparamref name="TEntity"/>.</returns>
        public static IEnumerable<TEntity> GetAll<TEntity>(this IDbConnection connection, bool buffered = true) where TEntity : class
        {
            var type = typeof (TEntity);

            string sql;
            if (!_getAllQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = string.Format("select * from {0}", tableName);
                _getAllQueryCache[type] = sql;
            }

            return connection.Query<TEntity>(sql: sql, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/> 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/> 
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, TReturn>(this IDbConnection connection, Func<T1, T2, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/> 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/> 
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, TReturn>(this IDbConnection connection, Func<T1, T2, T3, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/> 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/> 
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/> 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/> 
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/> 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/> 
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, T6, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, buffered: buffered);
        }

        /// <summary>
        /// Retrieves all the entities of type <typeparamref name="TReturn"/> 
        /// joined with the types specified as type parameters.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="T7">The seventh type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="map">The mapping to perform on the entities in the result set.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>
        /// A collection of entities of type <typeparamref name="TReturn"/> 
        /// joined with the specified type types.
        /// </returns>
        public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map, bool buffered = true)
        {
            return MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, buffered: buffered);
        }

        private static IEnumerable<TReturn> MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(IDbConnection connection, Delegate map, object id = null, bool buffered = true)
        {
            var type = typeof (TReturn);

            var tableName = Resolvers.Table(type);
            var keyProperty = Resolvers.KeyProperty(type);
            var keyColumnName = Resolvers.Column(keyProperty);

            string sql = string.Format("select * from {0}", tableName);

            var includeTypes = new[]
                                   {
                                       typeof (T1),
                                       typeof (T2),
                                       typeof (T3),
                                       typeof (T4),
                                       typeof (T5),
                                       typeof (T6),
                                       typeof (T7)
                                   }
                .Where(t => t != typeof (DontMap))
                .ToArray();

            foreach (var includeType in includeTypes.Where(t => t != type))
            {
                var includeTableName = Resolvers.Table(includeType);
                var includeKeyProperty = Resolvers.KeyProperty(includeType);
                var includeKeyColumnName = Resolvers.Column(includeKeyProperty);
                var foreignKeyProperty = Resolvers.ForeignKeyProperty(type, includeType);

                sql += string.Format(" {0} join {1} on {2}.{3} = {1}.{4}",
                    Nullable.GetUnderlyingType(foreignKeyProperty.PropertyType) != null
                        ? "left"
                        : "inner",
                    includeTableName,
                    tableName,
                    foreignKeyProperty.Name,
                    includeKeyColumnName);
            }

            DynamicParameters parameters = null;
            if (id != null)
            {
                sql += string.Format(" where {0}.{1} = @{1}", tableName, keyColumnName);

                parameters = new DynamicParameters();
                parameters.Add("Id", id);
            }

            switch (includeTypes.Length)
            {
                case 2:
                    return connection.Query(sql, (Func<T1, T2, TReturn>)map, parameters, buffered: buffered);
                case 3:
                    return connection.Query(sql, (Func<T1, T2, T3, TReturn>)map, parameters, buffered: buffered);
                case 4:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, TReturn>)map, parameters, buffered: buffered);
                case 5:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, TReturn>)map, parameters, buffered: buffered);
                case 6:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, T6, TReturn>)map, parameters, buffered: buffered);
                case 7:
                    return connection.Query(sql, (Func<T1, T2, T3, T4, T5, T6, T7, TReturn>)map, parameters, buffered: buffered);
            }

            throw new InvalidOperationException();
        }

        private class DontMap
        {
        }

        /// <summary>
        /// Selects all the entities matching the specified predicate.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="predicate">A predicate to filter the results.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly, 
        /// or when the query is materialized (using <c>ToList()</c> for example). 
        /// </param>
        /// <returns>A collection of entities of type <typeparamref name="TEntity"/> matching the specified <paramref name="predicate"/>.</returns>
        public static IEnumerable<TEntity> Select<TEntity>(this IDbConnection connection, Expression<Func<TEntity, bool>> predicate, bool buffered = true)
        {
            var type = typeof (TEntity);

            string sql;
            if (!_getAllQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                sql = string.Format("select * from {0}", tableName);
                _getAllQueryCache[type] = sql;
            }

            DynamicParameters parameters;
            sql += new SqlExpression<TEntity>()
                .Where(predicate)
                .ToSql(out parameters);

            return connection.Query<TEntity>(sql: sql, param: parameters, buffered: buffered);
        }

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
            /// Builds a SQL expression for the specified filter expression.
            /// </summary>
            /// <param name="expression">The filter expression on the entity.</param>
            /// <returns>The current <see cref="DommelMapper.SqlExpression&lt;TEntity&gt;"/> instance.</returns>
            public virtual SqlExpression<TEntity> Where(Expression<Func<TEntity, bool>> expression)
            {
                AppendToWhere("and", expression);
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
                }

                return expression;
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
                    if (member.Expression != null)
                    {
                        return string.Format("{0} = '1'", VisitMemberAccess(member));
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
                        left = string.Format("{0} = '1'", VisitMemberAccess(member));
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
                        right = string.Format("{0} = '1'", VisitMemberAccess(member));
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

                    var paramName = "p" + _parameterIndex++;
                    _parameters.Add(paramName, value: right);
                    return string.Format("{0} {1} @{2}", left, operand, paramName);
                }

                return string.Format("{0} {1} {2}", left, operand, right);
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
                            o = string.Format("{0} = '1'", o);
                        }

                        return string.Format("not ({0})", o);
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
                var member = Expression.Convert(expression, typeof (object));
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

                var member = Expression.Convert(expression, typeof (object));
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
            /// Proccesses a member expression.
            /// </summary>
            /// <param name="expression">The member expression.</param>
            /// <returns>The result of the processing.</returns>
            protected virtual string MemberToColumn(MemberExpression expression)
            {
                return Resolvers.Column((PropertyInfo)expression.Member);
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

            /// <summary>
            /// Returns the current SQL query.
            /// </summary>
            /// <returns>The current SQL query.</returns>
            public override string ToString()
            {
                return _whereBuilder.ToString();
            }
        }

        /// <summary>
        /// Inserts the specified entity into the database and returns the id.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The id of the inserted entity.</returns>
        public static int Insert<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null) where TEntity : class
        {
            var type = typeof (TEntity);

            string sql;
            if (!_insertQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var typeProperties = Resolvers.Properties(type).Where(p => p != keyProperty).ToList();

                var columnNames = typeProperties.Select(Resolvers.Column).ToArray();
                var paramNames = typeProperties.Select(p => "@" + p.Name).ToArray();

                var builder = GetBuilder(connection);

                sql = builder.BuildInsert(tableName, columnNames, paramNames, keyProperty);

                _insertQueryCache[type] = sql;
            }

            var result = connection.Query<int>(sql, entity, transaction);
            return result.Single();
        }

        /// <summary>
        /// Updates the values of the specified entity in the database. 
        /// The return value indicates whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity in the database.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>A value indicating whether the update operation succeeded.</returns>
        public static bool Update<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null)
        {
            var type = typeof (TEntity);

            string sql;
            if (!_updateQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var typeProperties = Resolvers.Properties(type).Where(p => p != keyProperty).ToList();

                var columnNames = typeProperties.Select(p => string.Format("{0} = @{1}", Resolvers.Column(p), p.Name)).ToArray();

                sql = string.Format("update {0} set {1} where {2} = @{3}",
                    tableName,
                    string.Join(", ", columnNames),
                    Resolvers.Column(keyProperty),
                    keyProperty.Name);

                _updateQueryCache[type] = sql;
            }

            return connection.Execute(sql, entity, transaction) > 0;
        }

        /// <summary>
        /// Deletes the specified entity from the database. 
        /// Returns a value indicating whether the operation succeeded.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The connection to the database. This can either be open or closed.</param>
        /// <param name="entity">The entity to be deleted.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>A value indicating whether the delete operation succeeded.</returns>
        public static bool Delete<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction transaction = null)
        {
            var type = typeof (TEntity);

            string sql;
            if (!_deleteQueryCache.TryGetValue(type, out sql))
            {
                var tableName = Resolvers.Table(type);
                var keyProperty = Resolvers.KeyProperty(type);
                var keyColumnName = Resolvers.Column(keyProperty);

                sql = string.Format("delete from {0} where {1} = @{2}", tableName, keyColumnName, keyProperty.Name);

                _deleteQueryCache[type] = sql;
            }

            return connection.Execute(sql, entity, transaction) > 0;
        }

        /// <summary>
        /// Helper class for retrieving type metadata to build sql queries using configured resolvers.
        /// </summary>
        public static class Resolvers
        {
            private static readonly IDictionary<Type, string> _typeTableNameCache = new Dictionary<Type, string>();
            private static readonly IDictionary<string, string> _columnNameCache = new Dictionary<string, string>();
            private static readonly IDictionary<Type, PropertyInfo> _typeKeyPropertyCache = new Dictionary<Type, PropertyInfo>();
            private static readonly IDictionary<Type, PropertyInfo[]> _typePropertiesCache = new Dictionary<Type, PropertyInfo[]>();
            private static readonly IDictionary<string, PropertyInfo> _typeForeignKeyPropertyCache = new Dictionary<string, PropertyInfo>();

            /// <summary>
            /// Gets the key property for the specified type, using the configured <see cref="DommelMapper.IKeyPropertyResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="System.Type"/> to get the key property for.</param>
            /// <returns>The key property for <paramref name="type"/>.</returns>
            public static PropertyInfo KeyProperty(Type type)
            {
                PropertyInfo keyProperty;
                if (!_typeKeyPropertyCache.TryGetValue(type, out keyProperty))
                {
                    keyProperty = _keyPropertyResolver.ResolveKeyProperty(type);
                    _typeKeyPropertyCache[type] = keyProperty;
                }

                return keyProperty;
            }

            /// <summary>
            /// Gets the foreign key property for the specified source type and including type 
            /// using the configure d<see cref="DommelMapper.IForeignKeyPropertyResolver"/>.
            /// </summary>
            /// <param name="sourceType">The source type which should contain the foreign key property.</param>
            /// <param name="includingType">The type of the foreign key relation.</param>
            /// <returns>The foreign key property for <paramref name="sourceType"/> and <paramref name="includingType"/>.</returns>
            public static PropertyInfo ForeignKeyProperty(Type sourceType, Type includingType)
            {
                string key = string.Format("{0};{1}", sourceType.FullName, includingType.FullName);

                PropertyInfo keyProperty;
                if (!_typeForeignKeyPropertyCache.TryGetValue(key, out keyProperty))
                {
                    keyProperty = _foreignKeyPropertyResolver.ResolveForeignKeyProperty(sourceType, includingType);
                    _typeForeignKeyPropertyCache[key] = keyProperty;
                }

                return keyProperty;
            }

            /// <summary>
            /// Gets the properties to be mapped for the specified type, using the configured <see cref="DommelMapper.IPropertyResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="System.Type"/> to get the properties from.</param>
            /// <returns>>The collection of to be mapped properties of <paramref name="type"/>.</returns>
            public static IEnumerable<PropertyInfo> Properties(Type type)
            {
                PropertyInfo[] properties;
                if (!_typePropertiesCache.TryGetValue(type, out properties))
                {
                    properties = _propertyResolver.ResolveProperties(type).ToArray();
                    _typePropertiesCache[type] = properties;
                }

                return properties;
            }

            /// <summary>
            /// Gets the name of the table in the database for the specified type, 
            /// using the configured <see cref="DommelMapper.ITableNameResolver"/>.
            /// </summary>
            /// <param name="type">The <see cref="System.Type"/> to get the table name for.</param>
            /// <returns>The table name in the database for <paramref name="type"/>.</returns>
            public static string Table(Type type)
            {
                string name;
                if (!_typeTableNameCache.TryGetValue(type, out name))
                {
                    name = _tableNameResolver.ResolveTableName(type);
                    _typeTableNameCache[type] = name;
                }
                return name;
            }

            /// <summary>
            /// Gets the name of the column in the database for the specified type,
            /// using the configured <see cref="T:DommelMapper.IColumnNameResolver"/>.
            /// </summary>
            /// <param name="propertyInfo">The <see cref="System.Reflection.PropertyInfo"/> to get the column name for.</param>
            /// <returns>The column name in the database for <paramref name="propertyInfo"/>.</returns>
            public static string Column(PropertyInfo propertyInfo)
            {
                string key = string.Format("{0}.{1}", propertyInfo.DeclaringType, propertyInfo.Name);

                string columnName;
                if (!_columnNameCache.TryGetValue(key, out columnName))
                {
                    columnName = _columnNameResolver.ResolveColumnName(propertyInfo);
                    _columnNameCache[key] = columnName;
                }

                return columnName;
            }

            /// <summary>
            /// Provides access to default resolver implementations.
            /// </summary>
            public static class Default
            {
                /// <summary>
                /// The default column name resolver.
                /// </summary>
                public static readonly IColumnNameResolver ColumnNameResolver = new DefaultColumnNameResolver();

                /// <summary>
                /// The default property resolver.
                /// </summary>
                public static readonly IPropertyResolver PropertyResolver = new DefaultPropertyResolver();

                /// <summary>
                /// The default key property resolver.
                /// </summary>
                public static readonly IKeyPropertyResolver KeyPropertyResolver = new DefaultKeyPropertyResolver();

                /// <summary>
                /// The default table name resolver.
                /// </summary>
                public static readonly ITableNameResolver TableNameResolver = new DefaultTableNameResolver();
            }
        }

#region Property resolving
        private static IPropertyResolver _propertyResolver = new DefaultPropertyResolver();

        /// <summary>
        /// Defines methods for resolving the properties of entities. 
        /// Custom implementations can be registerd with <see cref="M:SetPropertyResolver()"/>.
        /// </summary>
        public interface IPropertyResolver
        {
            /// <summary>
            /// Resolves the properties to be mapped for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the properties to be mapped for.</param>
            /// <returns>A collection of <see cref="PropertyInfo"/>'s of the <paramref name="type"/>.</returns>
            IEnumerable<PropertyInfo> ResolveProperties(Type type);
        }

        /// <summary>
        /// Sets the <see cref="DommelMapper.IPropertyResolver"/> implementation for resolving key of entities.
        /// </summary>
        /// <param name="propertyResolver">An instance of <see cref="DommelMapper.IPropertyResolver"/>.</param>
        public static void SetPropertyResolver(IPropertyResolver propertyResolver)
        {
            _propertyResolver = propertyResolver;
        }

        /// <summary>
        /// Represents the base class for property resolvers.
        /// </summary>
        public abstract class PropertyResolverBase : IPropertyResolver
        {
            private static readonly HashSet<Type> _primitiveTypes = new HashSet<Type>
                                                                        {
                                                                            typeof (object),
                                                                            typeof (string),
                                                                            typeof (decimal),
                                                                            typeof (double),
                                                                            typeof (float),
                                                                            typeof (DateTime),
                                                                            typeof (TimeSpan)
                                                                        };

            /// <summary>
            /// Resolves the properties to be mapped for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the properties to be mapped for.</param>
            /// <returns>A collection of <see cref="PropertyInfo"/>'s of the <paramref name="type"/>.</returns>
            public abstract IEnumerable<PropertyInfo> ResolveProperties(Type type);

            /// <summary>
            /// Gets a collection of types that are considered 'primitive' for Dommel but are not for the CLR.
            /// Override this if you need your own implementation of this.
            /// </summary>
            protected virtual HashSet<Type> PrimitiveTypes
            {
                get
                {
                    return _primitiveTypes;
                }
            }

            /// <summary>
            /// Filters the complex types from the specified collection of properties.
            /// </summary>
            /// <param name="properties">A collection of properties.</param>
            /// <returns>The properties that are considered 'primitive' of <paramref name="properties"/>.</returns>
            protected virtual IEnumerable<PropertyInfo> FilterComplexTypes(IEnumerable<PropertyInfo> properties)
            {
                foreach (var property in properties)
                {
                    var type = property.PropertyType;
                    type = Nullable.GetUnderlyingType(type) ?? type;

#if DNXCORE50
                    if (type.GetTypeInfo().IsPrimitive || type.GetTypeInfo().IsEnum || PrimitiveTypes.Contains(type))
#else
                    if (type.IsPrimitive || type.IsEnum || PrimitiveTypes.Contains(type))
#endif
                    {
                        yield return property;
                    }
                }
            }
        }

        /// <summary>
        /// Default implemenation of the <see cref="DommelMapper.IPropertyResolver"/> interface.
        /// </summary>
        public class DefaultPropertyResolver : PropertyResolverBase
        {
            public override IEnumerable<PropertyInfo> ResolveProperties(Type type)
            {
                return FilterComplexTypes(type.GetProperties());
            }
        }
#endregion

#region Key property resolving
        private static IKeyPropertyResolver _keyPropertyResolver = new DefaultKeyPropertyResolver();

        /// <summary>
        /// Sets the <see cref="DommelMapper.IKeyPropertyResolver"/> implementation for resolving key properties of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="DommelMapper.IKeyPropertyResolver"/>.</param>
        public static void SetKeyPropertyResolver(IKeyPropertyResolver resolver)
        {
            _keyPropertyResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving the key property of entities. 
        /// Custom implementations can be registerd with <see cref="M:SetKeyPropertyResolver()"/>.
        /// </summary>
        public interface IKeyPropertyResolver
        {
            /// <summary>
            /// Resolves the key property for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the key property for.</param>
            /// <returns>A <see cref="PropertyInfo"/> instance of the key property of <paramref name="type"/>.</returns>
            PropertyInfo ResolveKeyProperty(Type type);
        }

        /// <summary>
        /// Implements the <see cref="DommelMapper.IKeyPropertyResolver"/> interface by resolving key properties
        /// with the [Key] attribute or with the name 'Id'.
        /// </summary>
        public class DefaultKeyPropertyResolver : IKeyPropertyResolver
        {
            /// <summary>
            /// Finds the key property by looking for a property with the [Key] attribute or with the name 'Id'.
            /// </summary>
            public virtual PropertyInfo ResolveKeyProperty(Type type)
            {
                var allProps = Resolvers.Properties(type).ToList();

                // Look for properties with the [Key] attribute.
                var keyProps = allProps.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

                if (keyProps.Count == 0)
                {
                    // Search for properties named as 'Id' as fallback.
                    keyProps = allProps.Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (keyProps.Count == 0)
                {
                    throw new Exception(string.Format("Could not find the key property for type '{0}'.", type.FullName));
                }

                if (keyProps.Count > 1)
                {
                    throw new Exception(string.Format("Multiple key properties were found for type '{0}'.", type.FullName));
                }

                return keyProps[0];
            }
        }
#endregion

#region Foreign key property resolving
        private static IForeignKeyPropertyResolver _foreignKeyPropertyResolver = new DefaultForeignKeyPropertyResolver();

        /// <summary>
        /// Sets the <see cref="T:DommelMapper.IForeignKeyPropertyResolver"/> implementation for resolving foreign key properties.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="T:DommelMapper.IForeignKeyPropertyResolver"/>.</param>
        public static void SetForeignKeyPropertyResolver(IForeignKeyPropertyResolver resolver)
        {
            _foreignKeyPropertyResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving foreign key properties.
        /// </summary>
        public interface IForeignKeyPropertyResolver
        {
            /// <summary>
            /// Resolves the foreign key property for the specified source type and including type.
            /// </summary>
            /// <param name="sourceType">The source type which should contain the foreign key property.</param>
            /// <param name="includingType">The type of the foreign key relation.</param>
            /// <returns>The foreign key property for <paramref name="sourceType"/> and <paramref name="includingType"/>.</returns>
            PropertyInfo ResolveForeignKeyProperty(Type sourceType, Type includingType);
        }

        /// <summary>
        /// Implements the <see cref="T:DommelMapper.IForeignKeyPropertyResolver"/> interface.
        /// </summary>
        public class DefaultForeignKeyPropertyResolver : IForeignKeyPropertyResolver
        {
            /// <summary>
            /// Resolves the foreign key property for the specified source type and including type 
            /// by using <paramref name="includingType"/> + Id as property name.
            /// </summary>
            /// <param name="sourceType">The source type which should contain the foreign key property.</param>
            /// <param name="includingType">The type of the foreign key relation.</param>
            /// <returns>The foreign key property for <paramref name="sourceType"/> and <paramref name="includingType"/>.</returns>
            public virtual PropertyInfo ResolveForeignKeyProperty(Type sourceType, Type includingType)
            {
                var foreignKeyName = includingType.Name + "Id";
                var foreignKeyProperty = sourceType.GetProperties().FirstOrDefault(p => p.Name == foreignKeyName);

                if (foreignKeyProperty == null)
                {
                    var msg = string.Format("Could not for foreign key property for type '{0}' in type '{1}'.", includingType.FullName, sourceType.FullName);
                    throw new Exception(msg);
                }

                return foreignKeyProperty;
            }
        }
#endregion

#region Table name resolving
        private static ITableNameResolver _tableNameResolver = new DefaultTableNameResolver();

        /// <summary>
        /// Sets the <see cref="T:Dommel.ITableNameResolver"/> implementation for resolving table names for entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="T:Dommel.ITableNameResolver"/>.</param>
        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving table names of entities. 
        /// Custom implementations can be registerd with <see cref="M:SetTableNameResolver()"/>.
        /// </summary>
        public interface ITableNameResolver
        {
            /// <summary>
            /// Resolves the table name for the specified type.
            /// </summary>
            /// <param name="type">The type to resolve the table name for.</param>
            /// <returns>A string containing the resolved table name for for <paramref name="type"/>.</returns>
            string ResolveTableName(Type type);
        }

        /// <summary>
        /// Implements the <see cref="T:Dommel.ITableNameResolver"/> interface by resolving table names 
        /// by making the type name plural and removing the 'I' prefix for interfaces. 
        /// </summary>
        public class DefaultTableNameResolver : ITableNameResolver
        {
            /// <summary>
            /// Resolves the table name by making the type plural (+ 's', Product -> Products) 
            /// and removing the 'I' prefix for interfaces.
            /// </summary>
            public virtual string ResolveTableName(Type type)
            {
                string name = type.Name + "s";
#if DNXCORE50
                if (type.GetTypeInfo().IsInterface && name.StartsWith("I"))
#else
                if (type.IsInterface && name.StartsWith("I"))
#endif
                {
                    name = name.Substring(1);
                }

                // todo: add [Table] attribute support.
                return name;
            }
        }
#endregion

#region Column name resolving
        private static IColumnNameResolver _columnNameResolver = new DefaultColumnNameResolver();

        /// <summary>
        /// Sets the <see cref="T:Dommel.IColumnNameResolver"/> implementation for resolving column names.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="T:Dommel.IColumnNameResolver"/>.</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver)
        {
            _columnNameResolver = resolver;
        }

        /// <summary>
        /// Defines methods for resolving column names for entities. 
        /// Custom implementations can be registerd with <see cref="M:SetColumnNameResolver()"/>.
        /// </summary>
        public interface IColumnNameResolver
        {
            /// <summary>
            /// Resolves the column name for the specified property.
            /// </summary>
            /// <param name="propertyInfo">The property of the entity.</param>
            /// <returns>The column name for the property.</returns>
            string ResolveColumnName(PropertyInfo propertyInfo);
        }

        /// <summary>
        /// Implements the <see cref="DommelMapper.IKeyPropertyResolver"/>.
        /// </summary>
        public class DefaultColumnNameResolver : IColumnNameResolver
        {
            /// <summary>
            /// Resolves the column name for the property. This is just the name of the property.
            /// </summary>
            public virtual string ResolveColumnName(PropertyInfo propertyInfo)
            {
                return propertyInfo.Name;
            }
        }
#endregion

#region Sql builders
        /// <summary>
        /// Adds a custom implementation of <see cref="T:DommelMapper.ISqlBuilder"/> 
        /// for the specified ADO.NET connection type.
        /// </summary>
        /// <param name="connectionType">
        /// The ADO.NET conncetion type to use the <paramref name="builder"/> with. 
        /// Example: <c>typeof(SqlConnection)</c>.
        /// </param>
        /// <param name="builder">An implementation of the <see cref="T:DommelMapper.ISqlBuilder interface"/>.</param>
        public static void AddSqlBuilder(Type connectionType, ISqlBuilder builder)
        {
            _sqlBuilders[connectionType.Name.ToLower()] = builder;
        }

        private static ISqlBuilder GetBuilder(IDbConnection connection)
        {
            var connectionName = connection.GetType().Name.ToLower();
            ISqlBuilder builder;
            return _sqlBuilders.TryGetValue(connectionName, out builder) ? builder : new SqlServerSqlBuilder();
        }

        /// <summary>
        /// Defines methods for building specialized SQL queries.
        /// </summary>
        public interface ISqlBuilder
        {
            /// <summary>
            /// Builds an insert query using the specified table name, column names and parameter names. 
            /// A query to fetch the new id will be included as well.
            /// </summary>
            /// <param name="tableName">The name of the table to query.</param>
            /// <param name="columnNames">The names of the columns in the table.</param>
            /// <param name="paramNames">The names of the parameters in the database command.</param>
            /// <param name="keyProperty">The key property. This can be used to query a specific column for the new id. This is optional.</param>
            /// <returns>An insert query including a query to fetch the new id.</returns>
            string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty);
        }

        private sealed class SqlServerSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return string.Format("set nocount on insert into {0} ({1}) values ({2}) select cast(scope_identity() as int)",
                    tableName,
                    string.Join(", ", columnNames),
                    string.Join(", ", paramNames));
            }
        }

        private sealed class SqlServerCeSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return string.Format("insert into {0} ({1}) values ({2}) select cast(@@IDENTITY as int)",
                    tableName,
                    string.Join(", ", columnNames),
                    string.Join(", ", paramNames));
            }
        }

        private sealed class SqliteSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return string.Format("insert into {0} ({1}) values ({2}); select last_insert_rowid() id",
                    tableName,
                    string.Join(", ", columnNames),
                    string.Join(", ", paramNames));
            }
        }

        private sealed class MySqlSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                return string.Format("insert into {0} ({1}) values ({2}) select LAST_INSERT_ID() id",
                    tableName,
                    string.Join(", ", columnNames),
                    string.Join(", ", paramNames));
            }
        }

        private sealed class PostgresSqlBuilder : ISqlBuilder
        {
            public string BuildInsert(string tableName, string[] columnNames, string[] paramNames, PropertyInfo keyProperty)
            {
                var sql = string.Format("insert into {0} ({1}) values ({2}) select last_insert_rowid() id",
                    tableName,
                    string.Join(", ", columnNames),
                    string.Join(", ", paramNames));

                if (keyProperty != null)
                {
                    var keyColumnName = Resolvers.Column(keyProperty);

                    sql += " RETURNING " + keyColumnName;
                }
                else
                {
                    // todo: what behavior is desired here?
                    throw new Exception("A key property is required for the PostgresSqlBuilder.");
                }

                return sql;
            }
        }
#endregion
    }
}
