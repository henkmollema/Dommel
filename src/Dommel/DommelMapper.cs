using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Dapper;

namespace Dommel
{
    /// <summary>
    /// Simple CRUD operations for Dapper.
    /// </summary>
    public static partial class DommelMapper
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo> ColumnNameCache = new ConcurrentDictionary<string, PropertyInfo>();

        internal static ConcurrentDictionary<QueryCacheKey, string> QueryCache { get; } = new ConcurrentDictionary<QueryCacheKey, string>();

        internal static IPropertyResolver PropertyResolver = new DefaultPropertyResolver();
        internal static IKeyPropertyResolver KeyPropertyResolver = new DefaultKeyPropertyResolver();
        internal static IForeignKeyPropertyResolver ForeignKeyPropertyResolver = new DefaultForeignKeyPropertyResolver();
        internal static ITableNameResolver TableNameResolver = new DefaultTableNameResolver();
        internal static IColumnNameResolver ColumnNameResolver = new DefaultColumnNameResolver();

        internal static readonly Dictionary<string, ISqlBuilder> SqlBuilders = new Dictionary<string, ISqlBuilder>
        {
            ["sqlconnection"] = new SqlServerSqlBuilder(),
            ["sqlceconnection"] = new SqlServerCeSqlBuilder(),
            ["sqliteconnection"] = new SqliteSqlBuilder(),
            ["npgsqlconnection"] = new PostgresSqlBuilder(),
            ["mysqlconnection"] = new MySqlSqlBuilder()
        };

        static DommelMapper()
        {
            // Type mapper for [Column] attribute
            SqlMapper.TypeMapProvider = type => CreateMap(type);

            SqlMapper.ITypeMap CreateMap(Type t) => new CustomPropertyTypeMap(t,
                (type, columnName) =>
                {
                    var cacheKey = type + columnName;
                    if (!ColumnNameCache.TryGetValue(cacheKey, out var propertyInfo))
                    {
                        propertyInfo = type.GetProperties().FirstOrDefault(p => p.GetCustomAttribute<ColumnAttribute>()?.Name == columnName || p.Name == columnName);
                        ColumnNameCache.TryAdd(cacheKey, propertyInfo);
                    }

                    return propertyInfo;
                });
        }

        /// <summary>
        /// A callback which gets invoked when queries and other information are logged.
        /// </summary>
        public static Action<string>? LogReceived;

        private static void LogQuery<T>(string query, [CallerMemberName]string? method = null)
            => LogReceived?.Invoke(method != null ? $"{method}<{typeof(T).Name}>: {query}" : query);

        /// <summary>
        /// Sets the <see cref="IPropertyResolver"/> implementation for resolving key of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IPropertyResolver"/>.</param>
        public static void SetPropertyResolver(IPropertyResolver resolver) => PropertyResolver = resolver;

        /// <summary>
        /// Sets the <see cref="IKeyPropertyResolver"/> implementation for resolving key properties of entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IKeyPropertyResolver"/>.</param>
        public static void SetKeyPropertyResolver(IKeyPropertyResolver resolver) => KeyPropertyResolver = resolver;

        /// <summary>
        /// Sets the <see cref="IForeignKeyPropertyResolver"/> implementation for resolving foreign key properties.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IForeignKeyPropertyResolver"/>.</param>
        public static void SetForeignKeyPropertyResolver(IForeignKeyPropertyResolver resolver) => ForeignKeyPropertyResolver = resolver;

        /// <summary>
        /// Sets the <see cref="ITableNameResolver"/> implementation for resolving table names for entities.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="ITableNameResolver"/>.</param>
        public static void SetTableNameResolver(ITableNameResolver resolver) => TableNameResolver = resolver;

        /// <summary>
        /// Sets the <see cref="IColumnNameResolver"/> implementation for resolving column names.
        /// </summary>
        /// <param name="resolver">An instance of <see cref="IColumnNameResolver"/>.</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver) => ColumnNameResolver = resolver;

        /// <summary>
        /// Adds a custom implementation of <see cref="ISqlBuilder"/>
        /// for the specified ADO.NET connection type.
        /// </summary>
        /// <param name="connectionType">
        /// The ADO.NET conncetion type to use the <paramref name="builder"/> with.
        /// Example: <c>typeof(SqlConnection)</c>.
        /// </param>
        /// <param name="builder">An implementation of the <see cref="ISqlBuilder"/> interface.</param>
        public static void AddSqlBuilder(Type connectionType, ISqlBuilder builder) => AddSqlBuilder(connectionType.Name.ToLower(), builder);

        /// <summary>
        /// Adds a custom implementation of <see cref="ISqlBuilder"/>
        /// for the specified connection name        ///
        /// </summary>
        /// <param name="connectionName">The name of the connection. E.g. "sqlconnection".</param>
        /// <param name="builder">An implementation of the <see cref="ISqlBuilder"/> interface.</param>
        public static void AddSqlBuilder(string connectionName, ISqlBuilder builder) => SqlBuilders[connectionName] = builder;

        /// <summary>
        /// Gets the configured <see cref="ISqlBuilder"/> for the specified <see cref="IDbConnection"/> instance.
        /// </summary>
        /// <param name="connection">The database connection instance.</param>
        /// <returns>The <see cref="ISqlBuilder"/> interface for the specified <see cref="IDbConnection"/> instance.</returns>
        public static ISqlBuilder GetSqlBuilder(IDbConnection connection)
        {
            var connectionTypeName = connection.GetType().Name;
            var builder = SqlBuilders.TryGetValue(connectionTypeName.ToLower(), out var b) ? b : new SqlServerSqlBuilder();
            LogReceived?.Invoke($"Selected SQL Builder '{builder.GetType().Name}' for connection type '{connectionTypeName}'");
            return builder;
        }

        /// <summary>
        /// The factory to create <see cref="SqlExpression{TEntity}"/> or custom instances.
        /// </summary>
        public static Func<Type, ISqlBuilder, object> SqlExpressionFactory = (type, sqlBuilder) =>
        {
            var expr = typeof(SqlExpression<>).MakeGenericType(type);
            return Activator.CreateInstance(expr, sqlBuilder);
        };

        internal static SqlExpression<TEntity> CreateSqlExpression<TEntity>(ISqlBuilder sqlBuilder)
        {
            var expr = SqlExpressionFactory(typeof(TEntity), sqlBuilder);
            return (SqlExpression<TEntity>)expr;
        }
    }
}
