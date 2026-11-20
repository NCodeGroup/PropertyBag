// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides a strongly typed, read-only view of a collection of properties that can be accessed by key.
/// </summary>
/// <remarks>
/// <para>
/// This interface defines the read-only operations for accessing properties in a property bag.
/// It serves as the base interface for <see cref="IPropertyBag"/>, which adds mutating operations.
/// </para>
/// <para>
/// Use this interface when you need to pass a property bag to code that should only read values
/// without modifying them, following the principle of least privilege.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// void ProcessSettings(IReadOnlyPropertyBag settings)
/// {
///     var timeoutKey = new PropertyBagKey&lt;TimeSpan&gt;("Timeout");
///     if (settings.TryGetValue(timeoutKey, out var timeout))
///     {
///         // Use the timeout value
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="IPropertyBag"/>
/// <seealso cref="PropertyBagKey{T}"/>
[PublicAPI]
public interface IReadOnlyPropertyBag
{
    /// <summary>
    /// Creates a new mutable <see cref="IPropertyBag"/> instance that holds the key-value pairs of the current property bag.
    /// </summary>
    /// <returns>
    /// A new <see cref="IPropertyBag"/> instance containing all the key-value pairs from the current property bag.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The returned property bag is independent of the original; modifications to the clone
    /// will not affect the original, and vice versa.
    /// </para>
    /// <para>
    /// How the values are copied is up to the implementation. A value that is a reference type may be
    /// shared by the original and the clone, so a change to it shows through both. The default implementation
    /// in the NCode.PropertyBag package clones each value that implements <see cref="ICloneable"/> and copies
    /// every other value by reference.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// IReadOnlyPropertyBag original = GetPropertyBag();
    /// IPropertyBag clone = original.Clone();
    ///
    /// // Modifications to the clone do not affect the original
    /// var key = new PropertyBagKey&lt;string&gt;("NewKey");
    /// clone.SetValue(key, "NewValue");
    /// </code>
    /// </example>
    IPropertyBag Clone();

    /// <summary>
    /// Attempts to retrieve a strongly typed value associated with the specified key from the property bag.
    /// </summary>
    /// <param name="key">The <see cref="PropertyBagKey{T}"/> that identifies the value to retrieve.</param>
    /// <param name="value">
    /// When this method returns, contains the strongly typed value associated with the specified key
    /// if the key is found; otherwise, the default value for type <typeparamref name="T"/>.
    /// This parameter is passed uninitialized.
    /// Note that the value may be <c>null</c> even when the method returns <c>true</c>,
    /// if <c>null</c> was explicitly stored for the key.
    /// </param>
    /// <typeparam name="T">The type of the value to retrieve from the property bag.</typeparam>
    /// <returns>
    /// <c>true</c> if the property bag contains a value with the specified key; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method provides a safe way to retrieve values without throwing exceptions when a key
    /// is not found. Use this method when the presence of a key is uncertain.
    /// </para>
    /// <para>
    /// The output <paramref name="value"/> may be <c>null</c> in two scenarios:
    /// <list type="bullet">
    /// <item><description>When the method returns <c>false</c> (key not found), the value is set to <c>default(T)</c>.</description></item>
    /// <item><description>When the method returns <c>true</c> (key found), but <c>null</c> was explicitly stored as the value.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var userNameKey = new PropertyBagKey&lt;string?&gt;("UserName");
    ///
    /// if (propertyBag.TryGetValue(userNameKey, out var userName))
    /// {
    ///     // Key exists, but userName could be null if null was stored
    ///     Console.WriteLine($"User: {userName ?? "(not set)"}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("User name key not found");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="PropertyBagExtensions"/>
    bool TryGetValue<T>(PropertyBagKey<T> key, out T? value);
}
