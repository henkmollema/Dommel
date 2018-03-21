using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dommel
{
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
        public static async Task<TReturn> GetAsync<T1, T2, TReturn>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, DontMap, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, CreateMap<T1, T2, TReturn>(), id)).FirstOrDefault();
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
        public static async Task<TReturn> GetAsync<T1, T2, T3, TReturn>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, T3, DontMap, DontMap, DontMap, DontMap, TReturn>(connection, CreateMap<T1, T2, T3, TReturn>(), id)).FirstOrDefault();
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
        public static async Task<TReturn> GetAsync<T1, T2, T3, T4, TReturn>(this IDbConnection connection, object id, IDbTransaction transaction = null) where TReturn : class
        {
            return (await MultiMapAsync<T1, T2, T3, T4, DontMap, DontMap, DontMap, TReturn>(connection, CreateMap<T1, T2, T3, T4, TReturn>(), id)).FirstOrDefault();
        }

        private static Func<T1, T2, TReturn> CreateMap<T1, T2, TReturn>()
        {
            var result = default(T1);
            return (target, t2) =>
            {
                if (result == null)
                {
                    result = target;
                }

                var targetProperties = typeof(T1).GetProperties();
                MapValueToTarget(targetProperties, result, t2);

                return (TReturn)(object)result;
            };
        }

        private static Func<T1, T2, T3, TReturn> CreateMap<T1, T2, T3, TReturn>()
        {
            var result = default(T1);
            return (target, t2, t3) =>
            {
                if (result == null)
                {
                    result = target;
                }

                var targetProperties = typeof(T1).GetProperties();
                MapValueToTarget(targetProperties, result, t2);
                MapValueToTarget(targetProperties, result, t3);

                return (TReturn)(object)result;
            };
        }

        private static Func<T1, T2, T3, T4, TReturn> CreateMap<T1, T2, T3, T4, TReturn>()
        {
            var result = default(T1);
            return (target, t2, t3, t4) =>
            {
                if (result == null)
                {
                    result = target;
                }

                var targetProperties = typeof(T1).GetProperties();
                MapValueToTarget(targetProperties, result, t2);
                MapValueToTarget(targetProperties, result, t3);
                MapValueToTarget(targetProperties, result, t4);

                return (TReturn)(object)result;
            };
        }

        private static void MapValueToTarget<T>(PropertyInfo[] props, object target, T instance)
        {
            var instanceType = typeof(T);
            var prop = props.FirstOrDefault(p => p.PropertyType == instanceType);
            if (prop != null)
            {
                prop.SetValue(target, instance);
                return;
            }

            var collectionType = typeof(ICollection<T>);
            prop = props.FirstOrDefault(p => collectionType.IsAssignableFrom(p.PropertyType));
            if (prop != null)
            {
                var value = prop.GetValue(target);
                if (value == null)
                {
                    var list = new List<T> { instance };
                    prop.SetValue(target, list);
                }
                else if (value != null && value is ICollection<T> collection)
                {
                    collection.Add(instance);
                }
            }
        }
    }
}
