using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Selects all the entities matching the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static Task<IEnumerable<TReturn>> SelectAsync<T1, T2, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
        CancellationToken cancellationToken = default)
        => MultiMapSelectAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, predicate, map, transaction, buffered, cancellationToken);

    /// <summary>
    /// Selects the first entity matching the specified predicate or a default value.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>
    /// An entity of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static async Task<TReturn?> FirstOrDefaultAsync<T1, T2, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, TReturn> map,
        IDbTransaction? transaction = null)
        where TReturn : class
        => (await SelectAsync(connection, predicate, map, transaction)).FirstOrDefault();

    /// <summary>
    /// Selects all the entities matching the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static Task<IEnumerable<TReturn>> SelectAsync<T1, T2, T3, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
        CancellationToken cancellationToken = default)
        => MultiMapSelectAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, predicate, map, transaction, buffered, cancellationToken);

    /// <summary>
    /// Selects the first entity matching the specified predicate or a default value.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>
    /// An entity of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static async Task<TReturn?> FirstOrDefaultAsync<T1, T2, T3, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, TReturn> map,
        IDbTransaction? transaction = null)
        where TReturn : class
        => (await SelectAsync(connection, predicate, map, transaction)).FirstOrDefault();

    /// <summary>
    /// Selects all the entities matching the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static Task<IEnumerable<TReturn>> SelectAsync<T1, T2, T3, T4, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, T4, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
        CancellationToken cancellationToken = default)
        => MultiMapSelectAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, predicate, map, transaction, buffered, cancellationToken);

    /// <summary>
    /// Selects the first entity matching the specified predicate or a default value.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>
    /// An entity of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static async Task<TReturn?> FirstOrDefaultAsync<T1, T2, T3, T4, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, T4, TReturn> map,
        IDbTransaction? transaction = null)
        where TReturn : class
        => (await SelectAsync(connection, predicate, map, transaction)).FirstOrDefault();

    /// <summary>
    /// Selects all the entities matching the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="T5">The fifth type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static Task<IEnumerable<TReturn>> SelectAsync<T1, T2, T3, T4, T5, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, T4, T5, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
        CancellationToken cancellationToken = default)
        => MultiMapSelectAsync<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, predicate, map, transaction, buffered, cancellationToken);

    /// <summary>
    /// Selects the first entity matching the specified predicate or a default value.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="T5">The fifth type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>
    /// An entity of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static async Task<TReturn?> FirstOrDefaultAsync<T1, T2, T3, T4, T5, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, T4, T5, TReturn> map,
        IDbTransaction? transaction = null)
        where TReturn : class
        => (await SelectAsync(connection, predicate, map, transaction)).FirstOrDefault();

    /// <summary>
    /// Selects all the entities matching the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="T5">The fifth type parameter.</typeparam>
    /// <typeparam name="T6">The sixth type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static Task<IEnumerable<TReturn>> SelectAsync<T1, T2, T3, T4, T5, T6, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, T4, T5, T6, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
        CancellationToken cancellationToken = default)
        => MultiMapSelectAsync<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, predicate, map, transaction, buffered, cancellationToken);

    /// <summary>
    /// Selects the first entity matching the specified predicate or a default value.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="T5">The fifth type parameter.</typeparam>
    /// <typeparam name="T6">The sixth type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>
    /// An entity of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static async Task<TReturn?> FirstOrDefaultAsync<T1, T2, T3, T4, T5, T6, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, T4, T5, T6, TReturn> map,
        IDbTransaction? transaction = null)
        where TReturn : class
        => (await SelectAsync(connection, predicate, map, transaction)).FirstOrDefault();

    /// <summary>
    /// Selects all the entities matching the specified predicate.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="T5">The fifth type parameter.</typeparam>
    /// <typeparam name="T6">The sixth type parameter.</typeparam>
    /// <typeparam name="T7">The seventh type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static Task<IEnumerable<TReturn>> SelectAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
        CancellationToken cancellationToken = default)
        => MultiMapSelectAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, predicate, map, transaction, buffered, cancellationToken);

    /// <summary>
    /// Selects the first entity matching the specified predicate or a default value.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="T5">The fifth type parameter.</typeparam>
    /// <typeparam name="T6">The sixth type parameter.</typeparam>
    /// <typeparam name="T7">The seventh type parameter.</typeparam>
    /// <typeparam name="TReturn">The type of the entity.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="predicate">A predicate to filter the results.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>
    /// An entity of type <typeparamref name="TReturn"/> matching the specified <paramref name="predicate"/>.
    /// </returns>
    public static async Task<TReturn?> FirstOrDefaultAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
        this IDbConnection connection,
        Expression<Func<TReturn, bool>> predicate,
        Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map,
        IDbTransaction? transaction = null)
        where TReturn : class
        => (await SelectAsync(connection, predicate, map, transaction)).FirstOrDefault();

    private static Task<IEnumerable<TReturn>> MultiMapSelectAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
        IDbConnection connection, Expression<Func<TReturn, bool>> predicate, Delegate map, IDbTransaction? transaction = null,
        bool buffered = true, CancellationToken cancellationToken = default)
        => MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
            connection,
            map,
            null,
            transaction,
            buffered,
            parameters => CreateMultiMapSelectWhereSql(connection, predicate, parameters),
            cancellationToken);
}
