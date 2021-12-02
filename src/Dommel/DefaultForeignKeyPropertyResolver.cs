using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Dommel;

/// <summary>
/// Implements the <see cref="IForeignKeyPropertyResolver"/> interface.
/// </summary>
public class DefaultForeignKeyPropertyResolver : IForeignKeyPropertyResolver
{
    /// <summary>
    /// Resolves the foreign key property for the specified source type and including type
    /// by using <paramref name="includingType"/> + Id as property name.
    /// </summary>
    /// <param name="sourceType">The source type which should contain the foreign key property.</param>
    /// <param name="includingType">The type of the foreign key relation.</param>
    /// <param name="foreignKeyRelation">The foreign key relationship type.</param>
    /// <returns>The foreign key property for <paramref name="sourceType"/> and <paramref name="includingType"/>.</returns>
    public virtual PropertyInfo ResolveForeignKeyProperty(Type sourceType, Type includingType, out ForeignKeyRelation foreignKeyRelation)
    {
        var oneToOneFk = ResolveOneToOne(sourceType, includingType);
        if (oneToOneFk != null)
        {
            foreignKeyRelation = ForeignKeyRelation.OneToOne;
            return oneToOneFk;
        }

        var oneToManyFk = ResolveOneToMany(sourceType, includingType);
        if (oneToManyFk != null)
        {
            foreignKeyRelation = ForeignKeyRelation.OneToMany;
            return oneToManyFk;
        }

        throw new InvalidOperationException(
            $"Could not resolve foreign key property. Source type '{sourceType.FullName}'; including type: '{includingType.FullName}'.");
    }

    private static PropertyInfo? ResolveOneToOne(Type sourceType, Type includingType)
    {
        // Look for the foreign key on the source type by making an educated guess about the property name.
        var foreignKeyName = includingType.Name + "Id";
        var foreignKeyProperty = sourceType.GetProperty(foreignKeyName);
        if (foreignKeyProperty != null)
        {
            return foreignKeyProperty;
        }

        // Determine if the source type contains a navigation property to the including type.
        var navigationProperty = sourceType.GetProperties().FirstOrDefault(p => p.PropertyType == includingType);
        if (navigationProperty != null)
        {
            // Resolve the foreign key property from the attribute.
            var fkAttr = navigationProperty.GetCustomAttribute<ForeignKeyAttribute>();
            if (fkAttr != null)
            {
                return sourceType.GetProperty(fkAttr.Name);
            }
        }

        return null;
    }

    private static PropertyInfo? ResolveOneToMany(Type sourceType, Type includingType)
    {
        // Look for the foreign key on the including type by making an educated guess about the property name.
        var foreignKeyName = sourceType.Name + "Id";
        var foreignKeyProperty = includingType.GetProperty(foreignKeyName);
        if (foreignKeyProperty != null)
        {
            return foreignKeyProperty;
        }

        var collectionType = typeof(IEnumerable<>).MakeGenericType(includingType);
        var navigationProperty = sourceType.GetProperties().FirstOrDefault(p => collectionType.IsAssignableFrom(p.PropertyType));
        if (navigationProperty != null)
        {
            // Resolve the foreign key property from the attribute.
            var fkAttr = navigationProperty.GetCustomAttribute<ForeignKeyAttribute>();
            if (fkAttr != null)
            {
                return includingType.GetProperty(fkAttr.Name);
            }
        }

        return null;
    }
}
