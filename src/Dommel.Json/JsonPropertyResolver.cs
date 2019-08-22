using System;
using System.Collections.Generic;
using System.Linq;

namespace Dommel.Json
{
    internal class JsonPropertyResolver : DommelMapper.DefaultPropertyResolver
    {
        public JsonPropertyResolver(IReadOnlyCollection<Type> jsonTypes)
        {
            JsonTypes = jsonTypes;
        }

        public IReadOnlyCollection<Type> JsonTypes { get; }

        // Append the given types to the base set of types Dommel considers
        // primitive so they will be used in insert and update queries.
        protected override HashSet<Type> PrimitiveTypes =>
            new HashSet<Type>(base.PrimitiveTypes.Concat(JsonTypes));
    }
}
