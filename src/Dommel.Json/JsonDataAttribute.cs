using System;

namespace Dommel.Json;

/// <summary>
/// Specifies that a property is persisted as a JSON document.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class JsonDataAttribute : Attribute
{
}
