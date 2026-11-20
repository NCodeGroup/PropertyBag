// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides an <see cref="IPropertyBag"/> that keeps its own values and reads through to a parent bag
/// for every key it holds no value for.
/// </summary>
/// <remarks>
/// <para>
/// Writes go to this bag only: the parent is never written. A local value hides the parent's value for
/// the same key, and <see cref="Remove"/> hides the parent's value without touching it.
/// </para>
/// <para>
/// A layered bag reads its parent on every call, so it sees later changes to the parent.
/// </para>
/// <para>
/// A layered bag neither enumerates its values nor locks, because <see cref="IReadOnlyPropertyBag"/>
/// has no enumeration and offers no lock.
/// </para>
/// <para>
/// <see cref="Clone"/> returns a layered bag over the same parent with a copy of the local values.
/// Values that implement <see cref="ICloneable"/> are deep-cloned; otherwise, the reference is copied.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var key = new PropertyBagKey&lt;string&gt;("UserName");
/// propertyBag.SetValue(key, "Alice");
///
/// var child = new LayeredPropertyBag(propertyBag);
/// child.SetValue(key, "Bob");
///
/// Console.WriteLine(child.TryGetValue(key, out var childName) ? childName : null); // Output: Bob
/// Console.WriteLine(propertyBag.TryGetValue(key, out var parentName) ? parentName : null); // Output: Alice
/// </code>
/// </example>
/// <seealso cref="IPropertyBag"/>
/// <seealso cref="IReadOnlyPropertyBag"/>
[PublicAPI]
public sealed class LayeredPropertyBag(IReadOnlyPropertyBag parent)
    : IPropertyBag,
        ICloneable,
        IScopeRestorer
{
    /// <summary>
    /// Gets the bag that this bag reads through to.
    /// </summary>
    /// <value>The parent bag, which this bag never writes.</value>
    /// <exception cref="ArgumentNullException">The parent passed to the constructor is <c>null</c>.</exception>
    public IReadOnlyPropertyBag Parent { get; } =
        parent ?? throw new ArgumentNullException(nameof(parent));

    /// <summary>
    /// The marker that a local entry holds when the key is hidden from the parent.
    /// </summary>
    private static object Removed { get; } = new();

    /// <summary>
    /// The local entries, or <c>null</c> if nothing has been written yet.
    /// </summary>
    private Dictionary<PropertyBagKey, object?>? LocalOrNull { get; set; }

    /// <summary>
    /// Gets the local entries, initializing them if necessary.
    /// </summary>
    private Dictionary<PropertyBagKey, object?> Local => LocalOrNull ??= [];

    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// Creates a new <see cref="LayeredPropertyBag"/> over the same <see cref="Parent"/>, containing
    /// copies of the local entries, hidden keys included.
    /// </para>
    /// <para>
    /// Values that implement <see cref="ICloneable"/> are deep-cloned; all other values are copied by reference.
    /// </para>
    /// </remarks>
    public IPropertyBag Clone()
    {
        var newBag = new LayeredPropertyBag(Parent);

        var local = LocalOrNull;
        if (local is not { Count: > 0 })
            return newBag;

        var newLocal = new Dictionary<PropertyBagKey, object?>(local.Count);
        foreach (var (key, value) in local)
        {
            newLocal[key] = value is ICloneable cloneable ? cloneable.Clone() : value;
        }

        newBag.LocalOrNull = newLocal;
        return newBag;
    }

    /// <summary>
    /// Creates a copy of this property bag, as <see cref="Clone"/> does.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone() => Clone();

    /// <inheritdoc />
    /// <remarks>
    /// The value is stored in this bag and hides the parent's value for the key. The parent is not written.
    /// </remarks>
    public IPropertyBag SetValue<T>(PropertyBagKey<T> key, T? value)
    {
        Local[key] = value;
        return this;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The key reads as absent from this bag, even when the parent holds a value for it.
    /// The parent is not written.
    /// </remarks>
    public IPropertyBag Remove(PropertyBagKey key)
    {
        Local[key] = Removed;
        return this;
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method sets the value in this bag and returns a <see cref="DefaultPropertyBagScope"/> that, when
    /// disposed, puts back the local state the key held before this call: a local value, a hidden key, or
    /// no local entry, which reads through to the parent again.
    /// </remarks>
    public IPropertyBagScope Scope<T>(PropertyBagKey<T> key, T? value)
    {
        var local = Local;
        PropertyBagKey baseKey = key;
        var hadValue = local.TryGetValue(baseKey, out var previousValue);
        local[baseKey] = value;

        return new DefaultPropertyBagScope(this, key, hadValue, previousValue);
    }

    /// <summary>
    /// Puts back the local state that a key held before a scope set its value.
    /// </summary>
    /// <param name="key">The key whose state to put back.</param>
    /// <param name="hadValue">
    /// <c>true</c> if the key held a local entry before the scope set it; otherwise, <c>false</c>.
    /// </param>
    /// <param name="previousValue">
    /// The local entry the key held before the scope set it, when <paramref name="hadValue"/> is <c>true</c>.
    /// </param>
    void IScopeRestorer.Restore(PropertyBagKey key, bool hadValue, object? previousValue)
    {
        if (hadValue)
        {
            Local[key] = previousValue;
        }
        else
        {
            Local.Remove(key);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// A local value, or a hidden key, answers without asking the parent. Otherwise the parent answers.
    /// </remarks>
    public bool TryGetValue<T>(PropertyBagKey<T> key, out T? value)
    {
        if (LocalOrNull is { } local && local.TryGetValue(key, out var stored))
        {
            if (ReferenceEquals(stored, Removed))
            {
                value = default;
                return false;
            }

            return PropertyBagValue.TryCast(stored, out value);
        }

        return Parent.TryGetValue(key, out value);
    }
}
