#region Copyright Preamble

//
//    Copyright @ 2023 NCode Group
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

#endregion

using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace NCode.PropertyBag;

/// <summary>
/// Provides the default implementation of the <see cref="IPropertyBag"/> interface using a dictionary-based storage.
/// </summary>
/// <remarks>
/// <para>
/// This class implements <see cref="IPropertyBag"/> and also provides <see cref="IReadOnlyDictionary{TKey,TValue}"/>
/// and <see cref="ICloneable"/> interfaces for additional flexibility.
/// </para>
/// <para>
/// The internal storage is lazily initialized, meaning no memory is allocated for the dictionary until
/// the first value is set. This makes empty property bags very lightweight.
/// </para>
/// <para>
/// When cloning, values that implement <see cref="ICloneable"/> are deep-cloned; otherwise, the reference
/// is copied (shallow copy for reference types).
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var propertyBag = new DefaultPropertyBag();
/// var key = new PropertyBagKey&lt;string&gt;("UserName");
///
/// propertyBag.Set(key, "Alice");
///
/// if (propertyBag.TryGetValue(key, out var userName))
/// {
///     Console.WriteLine(userName); // Output: Alice
/// }
/// </code>
/// </example>
/// <seealso cref="IPropertyBag"/>
/// <seealso cref="IReadOnlyPropertyBag"/>
/// <seealso cref="DefaultPropertyBagFactory"/>
public class DefaultPropertyBag : IPropertyBag, IReadOnlyDictionary<PropertyBagKey, object?>, ICloneable
{
    /// <summary>
    /// The underlying dictionary storage, or <c>null</c> if no values have been set yet.
    /// </summary>
    private Dictionary<PropertyBagKey, object?>? ItemsOrNull { get; set; }

    /// <summary>
    /// Gets the underlying dictionary, initializing it if necessary.
    /// </summary>
    private Dictionary<PropertyBagKey, object?> Items => ItemsOrNull ??= new Dictionary<PropertyBagKey, object?>();

    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// Creates a new <see cref="DefaultPropertyBag"/> instance containing copies of all key-value pairs.
    /// </para>
    /// <para>
    /// Values that implement <see cref="ICloneable"/> are deep-cloned by calling their
    /// <see cref="ICloneable.Clone"/> method. All other values are copied by reference (shallow copy).
    /// </para>
    /// <para>
    /// If the current property bag is empty, an empty property bag is returned without allocating
    /// internal storage.
    /// </para>
    /// </remarks>
    public IPropertyBag Clone()
    {
        var newBag = new DefaultPropertyBag();

        var items = ItemsOrNull;
        if (items is not { Count: > 0 })
            return newBag;

        var newItems = items.Select(item =>
            item.Value is ICloneable cloneable
                ? new KeyValuePair<PropertyBagKey, object?>(item.Key, cloneable.Clone())
                : item);

        newBag.ItemsOrNull = new Dictionary<PropertyBagKey, object?>(newItems);

        return newBag;
    }

    /// <summary>
    /// Creates a shallow copy of this property bag.
    /// </summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <remarks>
    /// This explicit interface implementation calls <see cref="Clone"/> and returns the result as <see cref="object"/>.
    /// </remarks>
    object ICloneable.Clone() => Clone();

    /// <inheritdoc />
    /// <remarks>
    /// If the key already exists, its value is overwritten with the new value.
    /// </remarks>
    public IPropertyBag Set<T>(PropertyBagKey<T> key, T value)
    {
        Items[key] = value;
        return this;
    }

    /// <inheritdoc />
    /// <remarks>
    /// If the key does not exist in the property bag, this method has no effect.
    /// </remarks>
    public IPropertyBag Remove(PropertyBagKey key)
    {
        Items.Remove(key);
        return this;
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method sets the value using <see cref="Set{T}"/> and returns a <see cref="DefaultPropertyBagScope"/>
    /// that will restore the previous state when disposed.
    /// </remarks>
    public IPropertyBagScope Scope<T>(PropertyBagKey<T> key, T value)
    {
        Set(key, value);
        return new DefaultPropertyBagScope(this, key);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This method performs a type check on the retrieved value. If the stored value is not of type
    /// <typeparamref name="T"/>, this method returns <c>false</c>.
    /// </remarks>
    public bool TryGetValue<T>(PropertyBagKey<T> key, [MaybeNullWhen(false)] out T value)
    {
        if (TryGetBase(key, out var baseValue) && baseValue is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to retrieve a value from the internal dictionary without type checking.
    /// </summary>
    /// <param name="key">The key to look up in the dictionary.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key if the key is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.
    /// </param>
    /// <returns><c>true</c> if the key was found in the dictionary; otherwise, <c>false</c>.</returns>
    private bool TryGetBase(PropertyBagKey key, out object? value)
    {
        if (ItemsOrNull?.TryGetValue(key, out var baseValue) ?? false)
        {
            value = baseValue;
            return true;
        }

        value = default;
        return false;
    }

    #region IReadOnlyDictionary Implementation

    /// <summary>
    /// Returns an enumerator that iterates through the property bag's key-value pairs.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator() =>
        (ItemsOrNull ?? Enumerable.Empty<KeyValuePair<PropertyBagKey, object?>>()).GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the property bag's key-value pairs.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    IEnumerator<KeyValuePair<PropertyBagKey, object?>> IEnumerable<KeyValuePair<PropertyBagKey, object?>>.
        GetEnumerator() =>
        (ItemsOrNull ?? Enumerable.Empty<KeyValuePair<PropertyBagKey, object?>>()).GetEnumerator();

    /// <summary>
    /// Gets the number of key-value pairs in the property bag.
    /// </summary>
    int IReadOnlyCollection<KeyValuePair<PropertyBagKey, object?>>.Count =>
        ItemsOrNull?.Count ?? 0;

    /// <summary>
    /// Gets a collection containing the keys in the property bag.
    /// </summary>
    IEnumerable<PropertyBagKey> IReadOnlyDictionary<PropertyBagKey, object?>.Keys =>
        ItemsOrNull?.Keys ?? Enumerable.Empty<PropertyBagKey>();

    /// <summary>
    /// Gets a collection containing the values in the property bag.
    /// </summary>
    IEnumerable<object?> IReadOnlyDictionary<PropertyBagKey, object?>.Values =>
        ItemsOrNull?.Values ?? Enumerable.Empty<object?>();

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or <c>null</c> if the key is not found.</returns>
    object? IReadOnlyDictionary<PropertyBagKey, object?>.this[PropertyBagKey key] =>
        TryGetBase(key, out var value) ? value : default;

    /// <summary>
    /// Determines whether the property bag contains an element with the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the property bag.</param>
    /// <returns><c>true</c> if the property bag contains an element with the key; otherwise, <c>false</c>.</returns>
    bool IReadOnlyDictionary<PropertyBagKey, object?>.ContainsKey(PropertyBagKey key) =>
        ItemsOrNull?.ContainsKey(key) ?? false;

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key if the key is found;
    /// otherwise, <c>null</c>. This parameter is passed uninitialized.
    /// </param>
    /// <returns><c>true</c> if the property bag contains an element with the specified key; otherwise, <c>false</c>.</returns>
    bool IReadOnlyDictionary<PropertyBagKey, object?>.TryGetValue(PropertyBagKey key, out object? value) =>
        TryGetBase(key, out value);

    #endregion
}
