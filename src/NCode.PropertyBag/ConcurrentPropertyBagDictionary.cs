// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using System.Collections;
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
/// The <see cref="IReadOnlyDictionary{TKey,TValue}"/> view takes its lock per call. Enumerating it, or
/// reading its keys or values, walks a snapshot taken under the lock, so a write by another thread
/// during the walk neither throws nor appears in it.
/// </para>
/// <para>
/// When cloning, values that implement <see cref="ICloneable"/> are deep-cloned; otherwise, the reference
/// is copied (shallow copy for reference types).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var shared = new ConcurrentPropertyBagDictionary();
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
public sealed class ConcurrentPropertyBagDictionary
    : IPropertyBag,
        ICloneable,
        IScopeRestorer,
        IReadOnlyDictionary<PropertyBagKey, object?>
{
    /// <summary>
    /// Initializes a new, empty <see cref="ConcurrentPropertyBagDictionary"/>.
    /// </summary>
    public ConcurrentPropertyBagDictionary()
    {
        Items = new Dictionary<PropertyBagKey, object?>();
    }

    /// <summary>
    /// Initializes a new <see cref="ConcurrentPropertyBagDictionary"/> that owns the specified dictionary.
    /// </summary>
    /// <param name="items">The dictionary that holds the entries of the new bag.</param>
    private ConcurrentPropertyBagDictionary(Dictionary<PropertyBagKey, object?> items)
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

    /// <summary>
    /// Stores the value for the key only when the bag holds no value for it, in one atomic step.
    /// </summary>
    /// <param name="key">The key of the value to add.</param>
    /// <param name="value">The value to store.</param>
    /// <typeparam name="T">The type of the value to store.</typeparam>
    /// <returns>
    /// <c>true</c> if the value was stored; <c>false</c> if the bag already held a value for
    /// <paramref name="key"/>, which is left unchanged.
    /// </returns>
    /// <remarks>
    /// A stored <c>null</c> counts as a value, so the key is not added again.
    /// </remarks>
    public bool TryAdd<T>(PropertyBagKey<T> key, T? value)
    {
        lock (Gate)
        {
            return Items.TryAdd(key, value);
        }
    }

    /// <summary>
    /// Retrieves the value stored for the key, or creates it with a factory and stores it.
    /// </summary>
    /// <param name="key">The key of the value to retrieve or add.</param>
    /// <param name="valueFactory">The function that creates the value when the bag holds none.</param>
    /// <typeparam name="T">The type of the value to retrieve or store.</typeparam>
    /// <returns>
    /// The value stored for <paramref name="key"/>, which may be <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="valueFactory"/> is <c>null</c>.</exception>
    /// <remarks>
    /// The factory runs outside the lock, so it may run more than once when several threads add the same
    /// key at once. Only one result is stored, and every caller gets that one.
    /// </remarks>
    public T? GetOrAdd<T>(PropertyBagKey<T> key, Func<PropertyBagKey<T>, T?> valueFactory)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);

        if (TryGetValue<T>(key, out var existing))
        {
            return existing;
        }

        var created = valueFactory(key);

        lock (Gate)
        {
            if (
                Items.TryGetValue(key, out var stored)
                && PropertyBagValue.TryCast(stored, out T? winner)
            )
            {
                return winner;
            }

            Items[key] = created;
            return created;
        }
    }

    /// <summary>
    /// Removes the value stored for the key and returns it, in one atomic step.
    /// </summary>
    /// <param name="key">The key of the value to remove.</param>
    /// <param name="value">
    /// When this method returns, contains the removed value if the key was found; otherwise, the default
    /// value for type <typeparamref name="T"/>.
    /// </param>
    /// <typeparam name="T">The type of the value to remove.</typeparam>
    /// <returns>
    /// <c>true</c> if the bag held a value for <paramref name="key"/> and it was removed; otherwise,
    /// <c>false</c>.
    /// </returns>
    public bool TryRemove<T>(PropertyBagKey<T> key, out T? value)
    {
        bool removed;
        object? stored;

        lock (Gate)
        {
            removed = Items.Remove(key, out stored);
        }

        if (removed)
        {
            PropertyBagValue.TryCast(stored, out value);
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// Creates a new <see cref="ConcurrentPropertyBagDictionary"/> instance containing copies of all key-value pairs.
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

        return new ConcurrentPropertyBagDictionary(newItems);
    }

    /// <summary>
    /// Creates a copy of this property bag, as <see cref="Clone"/> does.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <remarks>
    /// This explicit interface implementation calls <see cref="Clone"/> and returns the result as <see cref="object"/>.
    /// </remarks>
    object ICloneable.Clone() => Clone();

    #region IReadOnlyDictionary Implementation

    /// <summary>
    /// Returns an enumerator over a snapshot of the key-value pairs, taken under the lock.
    /// </summary>
    /// <returns>An enumerator for the snapshot.</returns>
    IEnumerator IEnumerable.GetEnumerator() => Snapshot().GetEnumerator();

    /// <summary>
    /// Returns an enumerator over a snapshot of the key-value pairs, taken under the lock.
    /// </summary>
    /// <returns>An enumerator for the snapshot.</returns>
    IEnumerator<KeyValuePair<PropertyBagKey, object?>> IEnumerable<
        KeyValuePair<PropertyBagKey, object?>
    >.GetEnumerator() =>
        ((IEnumerable<KeyValuePair<PropertyBagKey, object?>>)Snapshot()).GetEnumerator();

    /// <summary>
    /// Copies the key-value pairs under the lock.
    /// </summary>
    /// <returns>A snapshot of the entries.</returns>
    private KeyValuePair<PropertyBagKey, object?>[] Snapshot()
    {
        lock (Gate)
        {
            return Items.ToArray();
        }
    }

    /// <summary>
    /// Gets the number of key-value pairs in the property bag.
    /// </summary>
    int IReadOnlyCollection<KeyValuePair<PropertyBagKey, object?>>.Count
    {
        get
        {
            lock (Gate)
            {
                return Items.Count;
            }
        }
    }

    /// <summary>
    /// Gets a snapshot of the keys in the property bag.
    /// </summary>
    IEnumerable<PropertyBagKey> IReadOnlyDictionary<PropertyBagKey, object?>.Keys
    {
        get
        {
            lock (Gate)
            {
                return Items.Keys.ToArray();
            }
        }
    }

    /// <summary>
    /// Gets a snapshot of the values in the property bag.
    /// </summary>
    IEnumerable<object?> IReadOnlyDictionary<PropertyBagKey, object?>.Values
    {
        get
        {
            lock (Gate)
            {
                return Items.Values.ToArray();
            }
        }
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or <c>null</c> if the key is not found.</returns>
    object? IReadOnlyDictionary<PropertyBagKey, object?>.this[PropertyBagKey key]
    {
        get
        {
            lock (Gate)
            {
                return Items.GetValueOrDefault(key);
            }
        }
    }

    /// <summary>
    /// Determines whether the property bag contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the property bag.</param>
    /// <returns><c>true</c> if the property bag contains an element with the key; otherwise, <c>false</c>.</returns>
    bool IReadOnlyDictionary<PropertyBagKey, object?>.ContainsKey(PropertyBagKey key)
    {
        lock (Gate)
        {
            return Items.ContainsKey(key);
        }
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key if the key is found;
    /// otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if the property bag contains an element with the specified key; otherwise, <c>false</c>.</returns>
    bool IReadOnlyDictionary<PropertyBagKey, object?>.TryGetValue(
        PropertyBagKey key,
        out object? value
    )
    {
        lock (Gate)
        {
            return Items.TryGetValue(key, out value);
        }
    }

    #endregion
}
