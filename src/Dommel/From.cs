using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Dommel
{
    public static partial class DommelMapper
    {
        /// <summary>
        /// Executes an expression to query data from <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity to query data from.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static IEnumerable<TEntity> From<TEntity>(this IDbConnection con, Action<SqlExpression<TEntity>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
        {
            var sqlExpression = CreateSqlExpression<TEntity>(GetSqlBuilder(con));
            sqlBuilder(sqlExpression);
            var sql = sqlExpression.ToSql(out var parameters);
            return con.Query<TEntity>(sql, parameters, transaction, buffered);
        }

        /// <summary>
        /// Executes an expression to query data from <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="T7">The seventh type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static IEnumerable<TReturn> From<T1, T2, T3, T4, T5, T6, T7, TReturn>(
            this IDbConnection con, Action<SqlExpression<TReturn>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
            where T1 : TReturn
        {
            var map = CreateMapDelegate<T1, T2, T3, T3, T4, T5, T6, TReturn>();
            var sql = FromJoinSql<T1, T2, T3, T4, T5, T6, T7, TReturn>(con, sqlBuilder, out var includeTypes, out var parameters);
            var splitOn = CreateSplitOn(includeTypes);

            return includeTypes.Length switch
            {
                2 => con.Query(sql, (Func<T1, T2, TReturn>)map, parameters, transaction, buffered, splitOn),
                3 => con.Query(sql, (Func<T1, T2, T3, TReturn>)map, parameters, transaction, buffered, splitOn),
                4 => con.Query(sql, (Func<T1, T2, T3, T4, TReturn>)map, parameters, transaction, buffered, splitOn),
                5 => con.Query(sql, (Func<T1, T2, T3, T4, T5, TReturn>)map, parameters, transaction, buffered, splitOn),
                6 => con.Query(sql, (Func<T1, T2, T3, T4, T5, T6, TReturn>)map, parameters, transaction, buffered, splitOn),
                7 => con.Query(sql, (Func<T1, T2, T3, T4, T5, T6, T7, TReturn>)map, parameters, transaction, buffered, splitOn),
                _ => throw new InvalidOperationException($"Invalid amount of include types: {includeTypes.Length}."),
            };
        }

        /// <summary>
        /// Executes an expression to query data from <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The entity to query data from.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static async Task<IEnumerable<TEntity>> FromAsync<TEntity>(this IDbConnection con, Action<SqlExpression<TEntity>> sqlBuilder, IDbTransaction? transaction = null)
        {
            var sqlExpression = CreateSqlExpression<TEntity>(GetSqlBuilder(con));
            sqlBuilder(sqlExpression);
            var sql = sqlExpression.ToSql(out var parameters);
            return await con.QueryAsync<TEntity>(sql, parameters, transaction);
        }

        /// <summary>
        /// Executes an expression to query data from <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static Task<IEnumerable<TReturn>> FromAsync<T1, T2, TReturn>(
            this IDbConnection con, Action<SqlExpression<TReturn>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
            where T1 : TReturn
            => FromAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(con, sqlBuilder, transaction, buffered);

        /// <summary>
        /// Executes an expression to query data from <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static Task<IEnumerable<TReturn>> FromAsync<T1, T2, T3, TReturn>(
            this IDbConnection con, Action<SqlExpression<TReturn>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
            where T1 : TReturn
            => FromAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(con, sqlBuilder, transaction, buffered);

        /// <summary>
        /// Executes an expression to query data from <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static Task<IEnumerable<TReturn>> FromAsync<T1, T2, T3, T4, TReturn>(
            this IDbConnection con, Action<SqlExpression<TReturn>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
            where T1 : TReturn
            => FromAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(con, sqlBuilder, transaction, buffered);

        /// <summary>
        /// Executes an expression to query data from <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static Task<IEnumerable<TReturn>> FromAsync<T1, T2, T3, T4, T5, TReturn>(
            this IDbConnection con, Action<SqlExpression<TReturn>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
            where T1 : TReturn
            => FromAsync<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(con, sqlBuilder, transaction, buffered);

        /// <summary>
        /// Executes an expression to query data from <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static Task<IEnumerable<TReturn>> FromAsync<T1, T2, T3, T4, T5, T6, TReturn>(
            this IDbConnection con, Action<SqlExpression<TReturn>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
            where T1 : TReturn
            => FromAsync<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(con, sqlBuilder, transaction, buffered);

        /// <summary>
        /// Executes an expression to query data from <typeparamref name="T1"/>.
        /// </summary>
        /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
        /// <typeparam name="T2">The second type parameter.</typeparam>
        /// <typeparam name="T3">The third type parameter.</typeparam>
        /// <typeparam name="T4">The fourth type parameter.</typeparam>
        /// <typeparam name="T5">The fifth type parameter.</typeparam>
        /// <typeparam name="T6">The sixth type parameter.</typeparam>
        /// <typeparam name="T7">The seventh type parameter.</typeparam>
        /// <typeparam name="TReturn">The return type parameter.</typeparam>
        /// <param name="con">The connection to query data from.</param>
        /// <param name="sqlBuilder">A callback to build a <see cref="SqlExpression{TEntity}"/>.</param>
        /// <param name="transaction">Optional transaction for the command.</param>
        /// <param name="buffered">
        /// A value indicating whether the result of the query should be executed directly,
        /// or when the query is materialized (using <c>ToList()</c> for example).
        /// </param>
        /// <returns>The collection of entities returned from the query.</returns>
        public static Task<IEnumerable<TReturn>> FromAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
            this IDbConnection con, Action<SqlExpression<TReturn>> sqlBuilder, IDbTransaction? transaction = null, bool buffered = true)
            where T1 : TReturn
        {
            var map = CreateMapDelegate<T1, T2, T3, T3, T4, T5, T6, TReturn>();
            var sql = FromJoinSql<T1, T2, T3, T4, T5, T6, T7, TReturn>(con, sqlBuilder, out var includeTypes, out var parameters);
            var splitOn = CreateSplitOn(includeTypes);

            return includeTypes.Length switch
            {
                2 => con.QueryAsync(sql, (Func<T1, T2, TReturn>)map, parameters, transaction, buffered, splitOn),
                3 => con.QueryAsync(sql, (Func<T1, T2, T3, TReturn>)map, parameters, transaction, buffered, splitOn),
                4 => con.QueryAsync(sql, (Func<T1, T2, T3, T4, TReturn>)map, parameters, transaction, buffered, splitOn),
                5 => con.QueryAsync(sql, (Func<T1, T2, T3, T4, T5, TReturn>)map, parameters, transaction, buffered, splitOn),
                6 => con.QueryAsync(sql, (Func<T1, T2, T3, T4, T5, T6, TReturn>)map, parameters, transaction, buffered, splitOn),
                7 => con.QueryAsync(sql, (Func<T1, T2, T3, T4, T5, T6, T7, TReturn>)map, parameters, transaction, buffered, splitOn),
                _ => throw new InvalidOperationException($"Invalid amount of include types: {includeTypes.Length}."),
            };
        }

        private static string FromJoinSql<T1, T2, T3, T4, T5, T6, T7, TReturn>(
            IDbConnection con, Action<SqlExpression<TReturn>> sqlBuilder, out Type[] includeTypes, out DynamicParameters? parameters) where T1 : TReturn
        {
            var sqlExpression = CreateSqlExpression<TReturn>(GetSqlBuilder(con));
            sqlBuilder(sqlExpression);

            includeTypes = new[]
            {
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7)
            }
            .Where(t => t != typeof(DontMap))
            .ToArray();
            var joinSql = string.Join("", CreateJoinLines(GetSqlBuilder(con), includeTypes));
            sqlExpression.Join(joinSql);
            return sqlExpression.ToSql(out parameters);
        }
    }
}
