using System;

namespace Dommel;

/// <summary>
/// Specifies that a property should be ignored.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class IgnoreAttribute : Attribute
{
}
