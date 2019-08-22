using System;
using System.Reflection;
using Dapper;

namespace Dommel.Json
{
    /// <summary>
    /// Options for Dommel JSON support.
    /// </summary>
    public class DommelJsonOptions
    {
        /// <summary>
        /// Gets or sets the set of assemblies to scan for 
        /// entities with [<see cref="JsonDataAttribute"/>] properties.
        /// </summary>
        public Assembly[] EntityAssemblies { get; set; }

        /// <summary>
        /// Gets or sets the Dapper type handler being used to handle JSON objects.
        /// </summary>
        public Func<SqlMapper.ITypeHandler> JsonTypeHandler { get; set; }
    }
}
