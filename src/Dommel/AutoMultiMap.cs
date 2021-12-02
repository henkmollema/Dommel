using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dommel;

public static partial class DommelMapper
{
    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="id">The id of the entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="id">The id of the entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="id">The id of the entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="id">The id of the entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="id">The id of the entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, T4, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="id">The id of the entity in the database.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, T4, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static TReturn? Get<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, T6, T7, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/> with the specified <paramref name="id"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<TReturn?> GetAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, object id, IDbTransaction? transaction = null)
        where T1 : class, TReturn
        where TReturn : class
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, T6, T7, TReturn>(results),
            id,
            transaction);
        return results.Values.FirstOrDefault();
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="T5">The fifth type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    /// joined with the types specified as type parameters.
    /// </summary>
    /// <typeparam name="T1">The first type parameter. This is the source entity.</typeparam>
    /// <typeparam name="T2">The second type parameter.</typeparam>
    /// <typeparam name="T3">The third type parameter.</typeparam>
    /// <typeparam name="T4">The fourth type parameter.</typeparam>
    /// <typeparam name="T5">The fifth type parameter.</typeparam>
    /// <typeparam name="TReturn">The return type parameter.</typeparam>
    /// <param name="connection">The connection to the database. This can either be open or closed.</param>
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, DontMap, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
    ///
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, T6, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, T6, DontMap, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entities of type <typeparamref name="TReturn"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static IEnumerable<TReturn> GetAll<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = MultiMap<T1, T2, T3, T4, T5, T6, T7, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, T6, T7, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Retrieves the automatically mapped entity of type <typeparamref name="TReturn"/>
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
    /// <param name="transaction">Optional transaction for the command.</param>
    /// <param name="buffered">
    /// A value indicating whether the result of the query should be executed directly,
    /// or when the query is materialized (using <c>ToList()</c> for example).
    /// </param>
    /// <returns>The entity with the corresponding id joined with the specified types.</returns>
    public static async Task<IEnumerable<TReturn>> GetAllAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(this IDbConnection connection, IDbTransaction? transaction = null, bool buffered = true)
        where T1 : class, TReturn
    {
        var results = new Dictionary<int, T1>();
        _ = await MultiMapAsync<T1, T2, T3, T4, T5, T6, T7, TReturn>(
            connection,
            CreateMapDelegate<T1, T2, T3, T4, T5, T6, T7, TReturn>(results),
            id: null,
            transaction,
            buffered);
        return results.Values;
    }

    /// <summary>
    /// Creates a <see cref="Delegate"/> which performs the mapping between the joined tables specified
    /// in the generic parameters. The unique mapped entities of type <typeparamref name="T1"/> are
    /// stored in <paramref name="results"/>.
    /// </summary>
    /// <param name="results">A dictionary where the unique entities are added to.</param>
    internal static Delegate CreateMapDelegate<T1, T2, T3, T4, T5, T6, T7, TReturn>(Dictionary<int, T1> results)
        where T1 : class, TReturn
    {
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

        var targetProperties = typeof(T1).GetProperties();
        var keyProperty = Resolvers.KeyProperties(typeof(T1)).First().Property;

        T1 GetOrAdd(T1 target)
        {
            // Populate the dictionary with cached items keyed by the hash code
            // of the value of the primary key property. This way multi mapping
            // one-to-many relations don't produce multiple instances of the
            // same parent record.
            var id = keyProperty!.GetValue(target)!.GetHashCode();
            if (!results.TryGetValue(id, out var cachedItem))
            {
                cachedItem = target;
                results[id] = cachedItem;
            }
            return cachedItem;
        }

        switch (includeTypes.Length)
        {
            case 2:
                Func<T1, T2, TReturn> f2 = (target, t2) =>
                {
                    target = GetOrAdd(target);
                    MapValueToTarget(targetProperties, target, t2);
                    return target;
                };
                return f2;

            case 3:
                Func<T1, T2, T3, TReturn> f3 = (target, t2, t3) =>
                {
                    target = GetOrAdd(target);
                    MapValueToTarget(targetProperties, target, t2);
                    MapValueToTarget(targetProperties, target, t3);
                    return target;
                };
                return f3;

            case 4:
                Func<T1, T2, T3, T4, TReturn> f4 = (target, t2, t3, t4) =>
                {
                    target = GetOrAdd(target);
                    MapValueToTarget(targetProperties, target, t2);
                    MapValueToTarget(targetProperties, target, t3);
                    MapValueToTarget(targetProperties, target, t4);
                    return target;
                };
                return f4;

            case 5:
                Func<T1, T2, T3, T4, T5, TReturn> f5 = (target, t2, t3, t4, t5) =>
                {
                    target = GetOrAdd(target);
                    MapValueToTarget(targetProperties, target, t2);
                    MapValueToTarget(targetProperties, target, t3);
                    MapValueToTarget(targetProperties, target, t4);
                    MapValueToTarget(targetProperties, target, t5);
                    return target;
                };
                return f5;

            case 6:
                Func<T1, T2, T3, T4, T5, T6, TReturn> f6 = (target, t2, t3, t4, t5, t6) =>
                {
                    target = GetOrAdd(target);
                    MapValueToTarget(targetProperties, target, t2);
                    MapValueToTarget(targetProperties, target, t3);
                    MapValueToTarget(targetProperties, target, t4);
                    MapValueToTarget(targetProperties, target, t5);
                    MapValueToTarget(targetProperties, target, t6);
                    return target;
                };
                return f6;

            default:
                Func<T1, T2, T3, T4, T5, T6, T7, TReturn> f7 = (target, t2, t3, t4, t5, t6, t7) =>
                {
                    target = GetOrAdd(target);
                    MapValueToTarget(targetProperties, target, t2);
                    MapValueToTarget(targetProperties, target, t3);
                    MapValueToTarget(targetProperties, target, t4);
                    MapValueToTarget(targetProperties, target, t5);
                    MapValueToTarget(targetProperties, target, t6);
                    MapValueToTarget(targetProperties, target, t7);
                    return target;
                };
                return f7;
        }
    }

    private static void MapValueToTarget<T>(PropertyInfo[] props, object target, T instance)
    {
        if (instance == null)
        {
            // Nothing to add
            return;
        }

        // Find a property with the same type as the current instance
        var instanceType = typeof(T);
        var prop = props.FirstOrDefault(p => p.PropertyType == instanceType);
        if (prop != null)
        {
            // Assign the instance to the property of the target object
            prop.SetValue(target, instance);
            return;
        }

        // Find a collection type of current instance
        var collectionType = typeof(ICollection<T>);
        prop = props.FirstOrDefault(p => collectionType.IsAssignableFrom(p.PropertyType));
        if (prop != null)
        {
            var value = prop.GetValue(target);
            if (value is null)
            {
                // Create a new list of the type of the instance
                var list = new List<T> { instance };
                prop.SetValue(target, list);
            }
            else if (value is ICollection<T> collection && !collection.Contains(instance))
            {
                // Add the instance type to the existing list
                collection.Add(instance);
            }
        }
    }
}
