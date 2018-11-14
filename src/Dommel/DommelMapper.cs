using System;
using System.Runtime.CompilerServices;

namespace Dommel
{
    /// <summary>
    /// Simple CRUD operations for Dapper.
    /// </summary>
    public static partial class DommelMapper
    {
        /// <summary>
        /// The escape character to use for escaping the start of column and table names in queries.
        /// </summary>
        public static char EscapeCharacterStart;

        /// <summary>
        /// The escape character to use for escaping the end of column and table names in queries.
        /// </summary>
        public static char EscapeCharacterEnd;

        /// <summary>
        /// A callback which gets invoked when queries and other information are logged.
        /// </summary>
        public static Action<string> LogReceived;

        private static void LogQuery<T>(string query, [CallerMemberName]string method = null)
            => LogReceived?.Invoke(method != null ? $"{method}<{typeof(T).Name}>: {query}" : query);

        /// <summary>
        /// Clears the query caches.
        /// </summary>
        public static void ClearQueryCache()
        {
            // Keep this in sync with all query caches
            _columnNameCache.Clear();
            _countQueryCache.Clear();
            _deleteAllQueryCache.Clear();
            _deleteQueryCache.Clear();
            _getAllQueryCache.Clear();
            _getByIdsQueryCache.Clear();
            _getQueryCache.Clear();
            _insertQueryCache.Clear();
            _updateQueryCache.Clear();
        }
    }
}
