using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Dommel;

public static partial class DommelMapper
{
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, TReturn>(this IDbConnection connection, object id, Func<T1, T2, TReturn> map, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where TReturn : class
        => MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id, transaction).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, TReturn>(this IDbConnection connection, object id, Func<T1, T2, TReturn> map, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where TReturn : class
        => (await MultiMapAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id, transaction, cancellationToken: cancellationToken)).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, TReturn> map, IDbTransaction? transaction = null) where TReturn : class
        => MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id, transaction).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, TReturn> map, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where TReturn : class
        => (await MultiMapAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id, transaction, cancellationToken: cancellationToken)).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, T4, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, T4, TReturn> map, IDbTransaction? transaction = null) where TReturn : class
        => MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, id, transaction).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, T4, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, T4, TReturn> map, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where TReturn : class
        => (await MultiMapAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, id, transaction, cancellationToken: cancellationToken)).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, T4, T5, TReturn> map,
        IDbTransaction? transaction = null) where TReturn : class
        => MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, id, transaction).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, T4, T5, TReturn> map, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where TReturn : class
        => (await MultiMapAsync<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, id, transaction, cancellationToken: cancellationToken)).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, T4, T5, T6, TReturn> map, IDbTransaction? transaction = null) where TReturn : class
        => MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, id, transaction).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, T4, T5, T6, TReturn>(
        this IDbConnection connection,
        object id,
        Func<T1, T2, T3, T4, T5, T6, TReturn> map,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default) => (await MultiMapAsync<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, id, transaction, cancellationToken: cancellationToken)).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map, IDbTransaction? transaction = null) where TReturn : class
        => MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, id, transaction).FirstOrDefault();

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, object id, Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map, IDbTransaction? transaction = null, CancellationToken cancellationToken = default) where TReturn : class
        => (await MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, id, transaction, cancellationToken: cancellationToken)).FirstOrDefault();

    /// <summary>
    /// Retrieves all the entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true) => MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id: null, transaction, buffered);

    /// <summary>
    /// Retrieves all the entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="map">The mapping to perform on the entities in the result set.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
          CancellationToken cancellationToken = default) => MultiMapAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id: null, transaction, buffered, cancellationToken: cancellationToken);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true) => MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id: null, transaction, buffered);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
          CancellationToken cancellationToken = default) => MultiMapAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, map, id: null, transaction, buffered, cancellationToken: cancellationToken);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, T4, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true) => MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, id: null, transaction, buffered);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, T4, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
          CancellationToken cancellationToken = default) => MultiMapAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, map, id: null, transaction, buffered, cancellationToken: cancellationToken);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, T4, T5, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true) => MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, id: null, transaction, buffered);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, T4, T5, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
         CancellationToken cancellationToken = default) => MultiMapAsync<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(connection, map, id: null, transaction, buffered, cancellationToken: cancellationToken);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, T4, T5, T6, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true) => MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, id: null, transaction, buffered);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, T6, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, T4, T5, T6, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
        CancellationToken cancellationToken = default) => MultiMapAsync<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(connection, map, id: null, transaction, buffered, cancellationToken: cancellationToken);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, T7, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true) => MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, id: null, transaction, buffered);

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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <param name="cancellationToken">Optional cancellation token for the command.</param>
    /// <returns>
    /// A collection of entities of type <typeparamref name="TReturn"/>
    /// joined with the specified type types.
    /// </returns>
    public static Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
        this IDbConnection connection,
        Func<T1, T2, T3, T4, T5, T6, T7, TReturn> map,
        IDbTransaction? transaction = null,
        bool buffered = true,
        CancellationToken cancellationToken = default) => MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(connection, map, id: null, transaction, buffered, cancellationToken: cancellationToken);

    private static IEnumerable<TReturn> MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(
        IDbConnection connection, Delegate map, object? id, IDbTransaction? transaction = null, bool buffered = true, Func<DynamicParameters, string>? appendSql = null)
    {
        var resultType = typeof(TReturn);
        var includeTypes = new[]
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

        var sql = BuildMultiMapQuery(GetSqlBuilder(connection), resultType, includeTypes, id, out var parameters);
        if (appendSql != null)
        {
            parameters ??= new DynamicParameters();
            sql += appendSql(parameters);
        }
        LogQuery<TReturn>(sql);

        var splitOn = CreateSplitOn(includeTypes);
        return includeTypes.Length switch
        {
            2 => connection.Query(sql, (Func<T1, T2, TReturn>)map, parameters, transaction, buffered, splitOn),
            3 => connection.Query(sql, (Func<T1, T2, T3, TReturn>)map, parameters, transaction, buffered, splitOn),
            4 => connection.Query(sql, (Func<T1, T2, T3, T4, TReturn>)map, parameters, transaction, buffered, splitOn),
            5 => connection.Query(sql, (Func<T1, T2, T3, T4, T5, TReturn>)map, parameters, transaction, buffered, splitOn),
            6 => connection.Query(sql, (Func<T1, T2, T3, T4, T5, T6, TReturn>)map, parameters, transaction, buffered, splitOn),
            7 => connection.Query(sql, (Func<T1, T2, T3, T4, T5, T6, T7, TReturn>)map, parameters, transaction, buffered, splitOn),
            _ => throw new InvalidOperationException($"Invalid amount of include types: {includeTypes.Length}."),
        };
    }

    private static Task<IEnumerable<TReturn>> MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
        IDbConnection connection, Delegate map, object? id, IDbTransaction? transaction = null, bool buffered = true, Func<DynamicParameters, string>? appendSql = null, CancellationToken cancellationToken = default)
    {
        var resultType = typeof(TReturn);
        var includeTypes = new[]
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

        var sql = BuildMultiMapQuery(GetSqlBuilder(connection), resultType, includeTypes, id, out var parameters);
        if (appendSql != null)
        {
            parameters ??= new DynamicParameters();
            sql += appendSql(parameters);
        }
        LogQuery<TReturn>(sql);
        var splitOn = CreateSplitOn(includeTypes);

        var commandDefinition = new CommandDefinition(sql, parameters, transaction, flags: buffered ? CommandFlags.Buffered : CommandFlags.None, cancellationToken: cancellationToken);
        return includeTypes.Length switch
        {
            2 => connection.QueryAsync(commandDefinition, (Func<T1, T2, TReturn>)map, splitOn),
            3 => connection.QueryAsync(commandDefinition, (Func<T1, T2, T3, TReturn>)map, splitOn),
            4 => connection.QueryAsync(commandDefinition, (Func<T1, T2, T3, T4, TReturn>)map, splitOn),
            5 => connection.QueryAsync(commandDefinition, (Func<T1, T2, T3, T4, T5, TReturn>)map, splitOn),
            6 => connection.QueryAsync(commandDefinition, (Func<T1, T2, T3, T4, T5, T6, TReturn>)map, splitOn),
            7 => connection.QueryAsync(commandDefinition, (Func<T1, T2, T3, T4, T5, T6, T7, TReturn>)map, splitOn),
            _ => throw new InvalidOperationException($"Invalid amount of include types: {includeTypes.Length}."),
        };
    }

    internal static string CreateSplitOn(Type[] includeTypes)
    {
        // Create a splitOn parameter from the key properties of the included types
        // We use the column name resolver directly rather than via the Resolvers class
        // because Dapper needs an un-quoted column identifier.
        // E.g. FooId rather than [FooId] for SQL server, etc.
        return string.Join(",", includeTypes
            .Select(t => Resolvers.KeyProperties(t).First())
            .Select(p => ColumnNameResolver.ResolveColumnName(p.Property)));
    }

    internal static string BuildMultiMapQuery(ISqlBuilder sqlBuilder, Type resultType, Type[] includeTypes, object? id, out DynamicParameters? parameters)
    {
        var resultTableName = Resolvers.Table(resultType, sqlBuilder);
        var resultTableKeyColumnName = Resolvers.Column(Resolvers.KeyProperties(resultType).Single().Property, sqlBuilder);
        var sql = $"select * from {resultTableName}";

        // Determine the table to join with.
        var sourceType = includeTypes[0];
        for (var i = 1; i < includeTypes.Length; i++)
        {
            // Determine the table name of the joined table.
            var includeType = includeTypes[i];
            var foreignKeyTableName = Resolvers.Table(includeType, sqlBuilder);

            // Determine the foreign key and the relationship type.
            var foreignKeyProperty = Resolvers.ForeignKeyProperty(sourceType, includeType, out var relation);
            var foreignKeyPropertyName = Resolvers.Column(foreignKeyProperty, sqlBuilder);

            if (relation == ForeignKeyRelation.OneToOne)
            {
                // Determine the primary key of the foreign key table.
                var foreignKeyTableKeyColumName = Resolvers.Column(Resolvers.KeyProperties(includeType).Single().Property, sqlBuilder);
                sql += $" left join {foreignKeyTableName} on {foreignKeyPropertyName} = {foreignKeyTableKeyColumName}";
            }
            else if (relation == ForeignKeyRelation.OneToMany)
            {
                // Determine the primary key of the source table.
                var sourceKeyColumnName = Resolvers.Column(Resolvers.KeyProperties(sourceType).Single().Property, sqlBuilder);
                sql += $" left join {foreignKeyTableName} on {sourceKeyColumnName} = {foreignKeyPropertyName}";
            }
        }

        parameters = null;
        if (id != null)
        {
            sql += $" where {resultTableKeyColumnName} = {sqlBuilder.PrefixParameter("Id")}";

            parameters = new DynamicParameters();
            parameters.Add("Id", id);
        }

        return sql;
    }

    internal class DontMap
    {
    }
}
