using System;
using System.Collections.Generic;
using System.Text;

namespace Dommel
{

    /// <summary>
    /// Provides access to default resolver implementations.
    /// </summary>
    public static class DefaultResolvers
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
