using System;
using System.ComponentModel;

namespace Dommel;

/// <summary>
/// Supports building the set-clause of a bulk update by chaining
/// <see cref="SetProperty{TProperty}(Func{TEntity, TProperty}, TProperty)"/> calls.
/// Used as the parameter of the update expression passed to
/// <see cref="DommelMapper.UpdateMultiple{TEntity}"/> and
/// <see cref="DommelMapper.UpdateMultipleAsync{TEntity}"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to update.</typeparam>
public sealed class SetPropertyCalls<TEntity>
{
    private SetPropertyCalls()
    {
    }

    /// <summary>
    /// Specifies that the property selected by <paramref name="propertySelector"/>
    /// should be assigned the constant <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">A property access expression. E.g. <c>p => p.Name</c>.</param>
    /// <param name="value">The value to assign to the property.</param>
    /// <returns>The current <see cref="SetPropertyCalls{TEntity}"/> instance so calls can be chained.</returns>
    public SetPropertyCalls<TEntity> SetProperty<TProperty>(Func<TEntity, TProperty> propertySelector, TProperty value)
        => throw new InvalidOperationException("SetProperty can only be used within an UpdateMultiple expression.");

    /// <summary>
    /// Specifies that the property selected by <paramref name="propertySelector"/>
    /// should be assigned the value produced by <paramref name="valueSelector"/>.
    /// The value selector may reference columns of the entity and use arithmetic,
    /// e.g. <c>p => p.AmountInStock - 1</c>.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="propertySelector">A property access expression. E.g. <c>p => p.Name</c>.</param>
    /// <param name="valueSelector">An expression producing the value to assign. E.g. <c>p => p.AmountInStock - 1</c>.</param>
    /// <returns>The current <see cref="SetPropertyCalls{TEntity}"/> instance so calls can be chained.</returns>
    public SetPropertyCalls<TEntity> SetProperty<TProperty>(Func<TEntity, TProperty> propertySelector, Func<TEntity, TProperty> valueSelector)
        => throw new InvalidOperationException("SetProperty can only be used within an UpdateMultiple expression.");

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string? ToString() => base.ToString();

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj) => base.Equals(obj);

    /// <inheritdoc />
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => base.GetHashCode();
}
