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

using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Represents a non-generic key used to identify values in a <see cref="IPropertyBag"/>.
/// </summary>
/// <remarks>
/// <para>
/// This non-generic struct is primarily used internally and for operations that require
/// type-erased key handling, such as the <see cref="IPropertyBag.Remove"/> method.
/// </para>
/// <para>
/// For most use cases, prefer using the generic <see cref="PropertyBagKey{T}"/> which provides
/// compile-time type safety for the associated value.
/// </para>
/// <para>
/// Two keys are considered equal if they have the same <see cref="Type"/> and <see cref="Name"/>.
/// </para>
/// </remarks>
/// <seealso cref="PropertyBagKey{T}"/>
/// <seealso cref="IPropertyBag"/>
/// <remarks>
/// Initializes a new instance of the <see cref="PropertyBagKey"/> struct with the specified type and name.
/// </remarks>
/// <param name="type">The <see cref="System.Type"/> of the value associated with this key.</param>
/// <param name="name">The unique name that identifies this key within the specified type.</param>
/// <example>
/// <code>
/// var key = new PropertyBagKey(typeof(string), "UserName");
/// </code>
/// </example>
[PublicAPI]
public readonly struct PropertyBagKey(Type type, string name) : IEquatable<PropertyBagKey>
{
    /// <summary>
    /// Gets the <see cref="System.Type"/> of the value associated with this key.
    /// </summary>
    /// <value>The type of value that this key represents in the property bag.</value>
    public Type Type { get; } = type;

    /// <summary>
    /// Gets the unique name that identifies this key within a given type.
    /// </summary>
    /// <value>The string name used to distinguish keys of the same type.</value>
    /// <remarks>
    /// The name, combined with the <see cref="Type"/>, forms the unique identity of the key.
    /// Multiple keys can share the same name as long as they have different types.
    /// </remarks>
    public string Name { get; } = name;

    /// <inheritdoc/>
    public bool Equals(PropertyBagKey other) =>
        Type == other.Type && Name == other.Name;

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is PropertyBagKey other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(Type, Name);

    /// <summary>
    /// Determines whether two <see cref="PropertyBagKey"/> instances are equal.
    /// </summary>
    /// <param name="left">The first key to compare.</param>
    /// <param name="right">The second key to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same
    /// <see cref="Type"/> and <see cref="Name"/>; otherwise, <c>false</c>.</returns>
    public static bool operator ==(PropertyBagKey left, PropertyBagKey right) =>
        left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PropertyBagKey"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first key to compare.</param>
    /// <param name="right">The second key to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have different
    /// <see cref="Type"/> or <see cref="Name"/> values; otherwise, <c>false</c>.</returns>
    public static bool operator !=(PropertyBagKey left, PropertyBagKey right) =>
        !(left == right);
}

/// <summary>
/// Represents a strongly typed key used to store and retrieve values of type <typeparamref name="T"/>
/// in a <see cref="IPropertyBag"/>.
/// </summary>
/// <typeparam name="T">The type of the value associated with this key.</typeparam>
/// <remarks>
/// <para>
/// This generic struct provides compile-time type safety when working with property bags.
/// The type parameter <typeparamref name="T"/> ensures that values stored and retrieved
/// using this key are of the correct type.
/// </para>
/// <para>
/// Keys are identified by a combination of the type <typeparamref name="T"/> and the
/// <see cref="Name"/> property. Two keys are considered equal if they have the same
/// type parameter and name.
/// </para>
/// <para>
/// This struct can be implicitly converted to the non-generic <see cref="PropertyBagKey"/>
/// for use with type-erased operations.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Define keys as static readonly fields for reuse
/// public static readonly PropertyBagKey&lt;string&gt; UserNameKey = new("UserName");
/// public static readonly PropertyBagKey&lt;int&gt; UserAgeKey = new("UserAge");
///
/// // Use keys to store and retrieve values
/// propertyBag.Set(UserNameKey, "Alice");
/// propertyBag.Set(UserAgeKey, 30);
///
/// if (propertyBag.TryGetValue(UserNameKey, out var name))
/// {
///     Console.WriteLine(name); // Output: Alice
/// }
/// </code>
/// </example>
/// <seealso cref="PropertyBagKey"/>
/// <seealso cref="IPropertyBag"/>
/// <remarks>
/// Initializes a new instance of the <see cref="PropertyBagKey{T}"/> struct with the specified name.
/// </remarks>
/// <param name="name">The unique name that identifies this key within the type <typeparamref name="T"/>.</param>
/// <example>
/// <code>
/// var connectionStringKey = new PropertyBagKey&lt;string&gt;("ConnectionString");
/// var timeoutKey = new PropertyBagKey&lt;TimeSpan&gt;("Timeout");
/// </code>
/// </example>
[PublicAPI]
public readonly struct PropertyBagKey<T>(string name) : IEquatable<PropertyBagKey<T>>
{
    /// <summary>
    /// Gets the <see cref="System.Type"/> of the value associated with this key.
    /// </summary>
    /// <value>The type <typeparamref name="T"/> that this key represents, obtained via <c>typeof(T)</c>.</value>
    public Type Type => typeof(T);

    /// <summary>
    /// Gets the unique name that identifies this key within the type <typeparamref name="T"/>.
    /// </summary>
    /// <value>The string name used to distinguish keys of the same type.</value>
    /// <remarks>
    /// The name, combined with the type <typeparamref name="T"/>, forms the unique identity of the key.
    /// Multiple keys can share the same name as long as they have different type parameters.
    /// </remarks>
    public string Name { get; } = name;

    /// <inheritdoc/>
    public bool Equals(PropertyBagKey<T> other) =>
        Type == other.Type && Name == other.Name;

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is PropertyBagKey<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(Type, Name);

    /// <summary>
    /// Determines whether two <see cref="PropertyBagKey{T}"/> instances are equal.
    /// </summary>
    /// <param name="left">The first key to compare.</param>
    /// <param name="right">The second key to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have the same
    /// <see cref="Name"/>; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Since both operands share the same type parameter <typeparamref name="T"/>, only the
    /// <see cref="Name"/> property needs to be compared for equality.
    /// </remarks>
    public static bool operator ==(PropertyBagKey<T> left, PropertyBagKey<T> right) =>
        left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PropertyBagKey{T}"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first key to compare.</param>
    /// <param name="right">The second key to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> have different
    /// <see cref="Name"/> values; otherwise, <c>false</c>.</returns>
    public static bool operator !=(PropertyBagKey<T> left, PropertyBagKey<T> right) =>
        !(left == right);

    /// <summary>
    /// Implicitly converts a <see cref="PropertyBagKey{T}"/> to a non-generic <see cref="PropertyBagKey"/>.
    /// </summary>
    /// <param name="key">The generic key to convert.</param>
    /// <returns>A new <see cref="PropertyBagKey"/> with the same <see cref="Type"/> and <see cref="Name"/>.</returns>
    /// <remarks>
    /// This conversion allows generic keys to be used with APIs that accept the non-generic
    /// <see cref="PropertyBagKey"/>, such as <see cref="IPropertyBag.Remove"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var typedKey = new PropertyBagKey&lt;string&gt;("UserName");
    /// PropertyBagKey untypedKey = typedKey; // Implicit conversion
    /// propertyBag.Remove(untypedKey);
    /// </code>
    /// </example>
    public static implicit operator PropertyBagKey(PropertyBagKey<T> key) =>
        new(key.Type, key.Name);
}
