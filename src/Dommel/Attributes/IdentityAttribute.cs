using System;

namespace Dommel.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Denotes one or more properties that auto-generate/assign value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IdentityAttribute : Attribute
    {
        
    }
}