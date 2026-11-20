// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag;

/// <summary>
/// Provides the default implementation of the <see cref="IPropertyBagScope"/> interface
/// that restores or removes a value in a property bag when disposed.
/// </summary>
/// <remarks>
/// <para>
/// A scope created by <see cref="PropertyBagDictionary.Scope{T}"/>,
/// <see cref="ConcurrentPropertyBagDictionary.Scope{T}"/> or <see cref="LayeredPropertyBag.Scope{T}"/> restores the
/// value its key held in that bag when the scope was created, or removes the key when it held none, when the scope
/// is disposed. For a <see cref="LayeredPropertyBag"/>, that is the bag's own entry for the key, so a key it held no
/// entry for reads through to the parent again.
/// </para>
/// <para>
/// A scope built with the public constructor removes the value associated with the <see cref="Key"/>
/// from the <see cref="PropertyBag"/> when it is disposed.
/// </para>
/// <para>
/// <see cref="Dispose"/> acts only on its first call.
/// </para>
/// <para>
/// This class is typically used within a <c>using</c> statement to ensure proper cleanup:
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var key = new PropertyBagKey&lt;string&gt;("TempValue");
///
/// using (new DefaultPropertyBagScope(propertyBag, key))
/// {
///     // The value exists in the property bag
///     propertyBag.TryGetValue(key, out var value);
/// }
/// // After disposal, the value has been removed
/// </code>
/// </example>
/// <param name="propertyBag">The <see cref="IPropertyBag"/> instance that contains the scoped value.</param>
/// <param name="key">
/// The <see cref="PropertyBagKey"/> that identifies the scoped value that disposal restores or removes.
/// </param>
/// <seealso cref="IPropertyBagScope"/>
/// <seealso cref="IPropertyBag.Scope{T}"/>
/// <seealso cref="PropertyBagDictionary"/>
public sealed class DefaultPropertyBagScope(IPropertyBag propertyBag, PropertyBagKey key)
    : IPropertyBagScope
{
    /// <summary>
    /// Initializes a new scope whose <see cref="Dispose"/> hands the captured state back to the bag
    /// instead of removing the key.
    /// </summary>
    /// <param name="propertyBag">The <see cref="IScopeRestorer"/> instance that contains the scoped value.</param>
    /// <param name="key">The <see cref="PropertyBagKey"/> that identifies the scoped value.</param>
    /// <param name="hadPreviousValue">
    /// <c>true</c> if the key held a value when the scope was created; otherwise, <c>false</c>.
    /// </param>
    /// <param name="previousValue">
    /// The value the key held when the scope was created, when <paramref name="hadPreviousValue"/> is <c>true</c>.
    /// </param>
    internal DefaultPropertyBagScope(
        IScopeRestorer propertyBag,
        PropertyBagKey key,
        bool hadPreviousValue,
        object? previousValue
    )
        : this(propertyBag, key)
    {
        RestoreTarget = propertyBag;
        HadPreviousValue = hadPreviousValue;
        PreviousValue = previousValue;
    }

    /// <inheritdoc />
    /// <value>The property bag instance that was passed to the constructor.</value>
    public IPropertyBag PropertyBag { get; } = propertyBag;

    /// <inheritdoc />
    /// <value>
    /// The key that was passed to the constructor, identifying the value that disposal restores or removes.
    /// </value>
    public PropertyBagKey Key { get; } = key;

    /// <summary>
    /// Gets the bag that <see cref="Dispose"/> hands the captured state back to,
    /// or <c>null</c> for a scope that removes its key.
    /// </summary>
    private IScopeRestorer? RestoreTarget { get; }

    /// <summary>
    /// Gets a value that indicates whether the key held a value when the scope was created.
    /// </summary>
    private bool HadPreviousValue { get; }

    /// <summary>
    /// Gets the value the key held when the scope was created, when <see cref="HadPreviousValue"/> is <c>true</c>.
    /// </summary>
    private object? PreviousValue { get; }

    /// <summary>
    /// Gets or sets a value that indicates whether <see cref="Dispose"/> has already acted.
    /// </summary>
    private bool IsDisposed { get; set; }

    /// <summary>
    /// Restores or removes the scoped value in the property bag and suppresses finalization.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For a scope created by <see cref="PropertyBagDictionary.Scope{T}"/>,
    /// <see cref="ConcurrentPropertyBagDictionary.Scope{T}"/> or <see cref="LayeredPropertyBag.Scope{T}"/>, this method
    /// restores the value that <see cref="Key"/> held in that bag when the scope was created, or removes the key when it
    /// held none.
    /// For a scope built with the public constructor, it removes the value associated with <see cref="Key"/>
    /// from the <see cref="PropertyBag"/> by calling <see cref="IPropertyBag.Remove"/>.
    /// </para>
    /// <para>
    /// This method calls <see cref="GC.SuppressFinalize"/> to prevent the finalizer from running,
    /// following the standard dispose pattern.
    /// </para>
    /// <para>
    /// This method acts only on its first call; subsequent calls have no effect.
    /// </para>
    /// </remarks>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        if (RestoreTarget is null)
        {
            PropertyBag.Remove(Key);
        }
        else
        {
            RestoreTarget.Restore(Key, HadPreviousValue, PreviousValue);
        }

        GC.SuppressFinalize(this);
    }
}
