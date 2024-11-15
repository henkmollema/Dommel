using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Updates the values of the specified entity in the database.
    /// The return value indicates whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entity">The entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>A value indicating whether the update operation succeeded.</returns>
    public static bool Update<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null)
    {
        var sql = BuildUpdateQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return connection.Execute(sql, entity, transaction) > 0;
    }

    /// <summary>
    /// Updates the values of the specified entity in the database.
    /// The return value indicates whether the operation succeeded.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="entity">The entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>A value indicating whether the update operation succeeded.</returns>
    public static async Task<bool> UpdateAsync<TEntity>(this IDbConnection connection, TEntity entity, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        var sql = BuildUpdateQuery(GetSqlBuilder(connection), typeof(TEntity));
        LogQuery<TEntity>(sql);
        return await connection.ExecuteAsync(new CommandDefinition(sql, entity, transaction: transaction, cancellationToken: cancellationToken)) > 0;
    }

    public static async Task<bool> UpdateMultipleAsync<TEntity>(
        this IDbConnection connection,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateExpression,
        Expression<Func<TEntity, bool>> whereExpression,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        var sql = BuildUpdateMultipleQuery(GetSqlBuilder(connection), updateExpression, whereExpression, out var parameters);
        LogQuery<TEntity>(sql);
        return await connection.ExecuteAsync(new CommandDefinition(sql, parameters, transaction: transaction, cancellationToken: cancellationToken)) > 0;
    }

    internal static string BuildUpdateQuery(ISqlBuilder sqlBuilder, Type type)
    {
        var cacheKey = new QueryCacheKey(QueryCacheType.Update, sqlBuilder, type);
        if (!QueryCache.TryGetValue(cacheKey, out var sql))
        {
            var tableName = Resolvers.Table(type, sqlBuilder);

            // Use all non-key and non-generated properties for updates
            var keyProperties = Resolvers.KeyProperties(type);
            var typeProperties = Resolvers.Properties(type)
                .Where(x => !x.IsGenerated)
                .Select(x => x.Property)
                .Except(keyProperties.Where(p => p.IsGenerated).Select(p => p.Property));

            var columnNames = typeProperties.Select(p => $"{Resolvers.Column(p, sqlBuilder, false)} = {sqlBuilder.PrefixParameter(p.Name)}").ToArray();
            var whereClauses = keyProperties.Select(p => $"{Resolvers.Column(p.Property, sqlBuilder, false)} = {sqlBuilder.PrefixParameter(p.Property.Name)}");
            sql = $"update {tableName} set {string.Join(", ", columnNames)} where {string.Join(" and ", whereClauses)}";

            QueryCache.TryAdd(cacheKey, sql);
        }

        return sql;
    }

    internal static string BuildUpdateMultipleQuery<TEntity>(ISqlBuilder sqlBuilder, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> updateExpression, Expression<Func<TEntity, bool>> whereExpression, out DynamicParameters parameters)
    {
        var entityType = typeof(TEntity);
        var sql = new StringBuilder($"update {Resolvers.Table(entityType, sqlBuilder)} set ");

        var setters = new List<(LambdaExpression, Expression)>();
        PopulateSetPropertyCalls(updateExpression.Body, setters, updateExpression.Parameters[0]);
        if (setters.Count == 0)
        {
            throw new Exception();
        }

        var whereSql = CreateSqlExpression<TEntity>(sqlBuilder)
            .Where(whereExpression)
            .ToSql(out parameters);

        var visitor = new SqlVisitor(sqlBuilder);
        foreach (var (prop, val) in setters)
        {
            if (prop.Body is not MemberExpression memberExpression)
            {
                throw new Exception();
            }

            var p = visitor.VisitMemberAccess(memberExpression);
            sql.Append(p).Append(" = ");

            if (val is ConstantExpression constantExpression)
            {
                var parameterName = sqlBuilder.PrefixParameter("p" + (parameters.ParameterNames.Count() + 1));
                parameters.Add(parameterName, constantExpression.Value);
                sql.Append(parameterName);
            }
            else
            {
                var v = visitor.VisitExpression(val, out var expressionParameters).ToString()!;
                var expressionParameter = expressionParameters.ParameterNames.Single();
                var parameterName = sqlBuilder.PrefixParameter("p" + (parameters.ParameterNames.Count() + 1));
                v = v.Replace(sqlBuilder.PrefixParameter("p1"), parameterName);
                parameters.Add(parameterName, expressionParameters.Get<object>(expressionParameter));
                sql.Append(v);
            }
            sql.Append(", ");
        }

        sql.Remove(sql.Length - 2, 2); // Remove the last comma and space

        // Append the where statement
        sql.Append(whereSql);

        //foreach (var p in visitor.Parameters.ParameterNames)
        //{
        //    parameters.Add(p, parameters.Get<object>(p));
        //}

        return sql.ToString();

        static void PopulateSetPropertyCalls(
            Expression expression,
            List<(LambdaExpression, Expression)> list,
            ParameterExpression parameter)
        {
            switch (expression)
            {
                case ParameterExpression p when parameter == p:
                    break;

                case MethodCallExpression
                {
                    Method:
                    {
                        IsGenericMethod: true,
                        Name: nameof(SetPropertyCalls<int>.SetProperty),
                        DeclaringType.IsGenericType: true
                    }
                } methodCallExpression when methodCallExpression.Method.DeclaringType.GetGenericTypeDefinition() == typeof(SetPropertyCalls<>):
                    list.Add(((LambdaExpression)methodCallExpression.Arguments[0], methodCallExpression.Arguments[1]));
                    PopulateSetPropertyCalls(methodCallExpression.Object!, list, parameter);
                    break;
            }
        }
    }

    /// <summary>
    /// Supports setting multiple properties in an update query (<c>UpdateMultiple</c>).
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to apply updates to.</typeparam>
    public sealed class SetPropertyCalls<TEntity>
    {
        private SetPropertyCalls() { }

        public SetPropertyCalls<TEntity> SetProperty<TProperty>(
        Func<TEntity, TProperty> propertyExpression,
        Func<TEntity, TProperty> valueExpression)
        => throw new InvalidOperationException("SetProperty should only be used in UpdateMultiple lambda expressions.");

        public SetPropertyCalls<TEntity> SetProperty<TProperty>(
        Func<TEntity, TProperty> propertyExpression,
        TProperty value)
        => throw new InvalidOperationException("SetProperty should only be used in UpdateMultiple lambda expressions.");

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string? ToString() => base.ToString();

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => base.Equals(obj);

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();
    }
}
