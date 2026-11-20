// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides an <see cref="IPropertyBag"/> implementation that is safe to use from several threads at once.
/// </summary>
/// <remarks>
/// <para>
/// Every member of this class is safe to call from several threads at once. The bag holds one dictionary
/// behind a lock, so each call is atomic.
/// </para>
/// <para>
/// A scope returned by <see cref="Scope{T}"/> restores the value the key held when the scope was created,
/// or removes the key when it held none, even if another thread wrote the key since.
/// </para>
/// <para>
/// The lock guards the bag, not the values stored in it: a stored object is shared between threads
/// as it is.
/// </para>
/// <para>
/// When cloning, values that implement <see cref="ICloneable"/> are deep-cloned; otherwise, the reference
/// is copied (shallow copy for reference types).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var shared = new ConcurrentPropertyBag();
/// var key = new PropertyBagKey&lt;string&gt;("UserName");
///
/// Parallel.For(0, 10, _ => shared.SetValue(key, "Alice"));
///
/// if (shared.TryGetValue(key, out var userName))
/// {
///     Console.WriteLine(userName); // Output: Alice
/// }
/// </code>
/// </example>
/// <seealso cref="IPropertyBag"/>
/// <seealso cref="PropertyBagDictionary"/>
[PublicAPI]
public sealed class ConcurrentPropertyBag : IPropertyBag, ICloneable, IScopeRestorer
{
    /// <summary>
    /// Initializes a new, empty <see cref="ConcurrentPropertyBag"/>.
    /// </summary>
    public ConcurrentPropertyBag()
    {
        Items = new Dictionary<PropertyBagKey, object?>();
    }

    /// <summary>
    /// Initializes a new <see cref="ConcurrentPropertyBag"/> that owns the specified dictionary.
    /// </summary>
    /// <param name="items">The dictionary that holds the entries of the new bag.</param>
    private ConcurrentPropertyBag(Dictionary<PropertyBagKey, object?> items)
    {
        Items = items;
    }

    /// <summary>
    /// Gets the lock that guards <see cref="Items"/>.
    /// </summary>
    private Lock Gate { get; } = new();

    /// <summary>
    /// Gets the underlying dictionary storage. Every access holds <see cref="Gate"/>.
    /// </summary>
    private Dictionary<PropertyBagKey, object?> Items { get; }

    /// <inheritdoc />
    /// <remarks>
    /// If the key already exists, its value is overwritten with the new value.
    /// </remarks>
    public IPropertyBag SetValue<T>(PropertyBagKey<T> key, T? value)
    {
        lock (Gate)
        {
            Items[key] = value;
        }

        return this;
    }

    /// <inheritdoc />
    /// <remarks>
    /// If the key does not exist in the property bag, this method has no effect.
    /// </remarks>
    public IPropertyBag Remove(PropertyBagKey key)
    {
        lock (Gate)
        {
            Items.Remove(key);
        }

        return this;
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method captures the key's state and sets the value in one atomic step, and returns a scope that,
    /// when disposed, restores the captured value, or removes the key when it held none. The scope restores
    /// the value captured when it was created, even if another thread wrote the key since.
    /// </remarks>
    public IPropertyBagScope Scope<T>(PropertyBagKey<T> key, T? value)
    {
        bool hadValue;
        object? previousValue;

        lock (Gate)
        {
            PropertyBagKey baseKey = key;
            hadValue = Items.TryGetValue(baseKey, out previousValue);
            Items[baseKey] = value;
        }

        return new DefaultPropertyBagScope(this, key, hadValue, previousValue);
    }

    /// <summary>
    /// Puts back the state that a key held before a scope set its value.
    /// </summary>
    /// <param name="key">The key whose state to put back.</param>
    /// <param name="hadValue">
    /// <c>true</c> if the key held a value before the scope set it; otherwise, <c>false</c>.
    /// </param>
    /// <param name="previousValue">
    /// The value the key held before the scope set it, when <paramref name="hadValue"/> is <c>true</c>.
    /// </param>
    void IScopeRestorer.Restore(PropertyBagKey key, bool hadValue, object? previousValue)
    {
        lock (Gate)
        {
            if (hadValue)
            {
                Items[key] = previousValue;
            }
            else
            {
                Items.Remove(key);
            }
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method performs a type check on the retrieved value. If the stored value is not of type
    /// <typeparamref name="T"/> (and is not <c>null</c>), this method returns <c>false</c>.
    /// If the stored value is <c>null</c>, this method returns <c>true</c> with <paramref name="value"/> set to <c>null</c>.
    /// </remarks>
    public bool TryGetValue<T>(PropertyBagKey<T> key, out T? value)
    {
        bool found;
        object? stored;

        lock (Gate)
        {
            found = Items.TryGetValue(key, out stored);
        }

        if (found)
        {
            return PropertyBagValue.TryCast(stored, out value);
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// Creates a new <see cref="ConcurrentPropertyBag"/> instance containing copies of all key-value pairs.
    /// The entries are copied under the lock, so the clone holds one consistent state of this bag.
    /// </para>
    /// <para>
    /// Values that implement <see cref="ICloneable"/> are deep-cloned by calling their
    /// <see cref="ICloneable.Clone"/> method, outside the lock. All other values are copied by reference
    /// (shallow copy).
    /// </para>
    /// </remarks>
    public IPropertyBag Clone()
    {
        KeyValuePair<PropertyBagKey, object?>[] snapshot;

        lock (Gate)
        {
            snapshot = Items.ToArray();
        }

        var newItems = new Dictionary<PropertyBagKey, object?>(snapshot.Length);
        foreach (var (key, value) in snapshot)
        {
            newItems[key] = value is ICloneable cloneable ? cloneable.Clone() : value;
        }

        return new ConcurrentPropertyBag(newItems);
    }

    /// <summary>
    /// Creates a copy of this property bag, as <see cref="Clone"/> does.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <remarks>
    /// This explicit interface implementation calls <see cref="Clone"/> and returns the result as <see cref="object"/>.
    /// </remarks>
    object ICloneable.Clone() => Clone();
}
