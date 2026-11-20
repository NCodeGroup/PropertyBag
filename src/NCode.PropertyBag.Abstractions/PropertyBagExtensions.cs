// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides the <see cref="IPropertyBag"/> extensions that write values: one that names a key after the
/// caller's argument expression, and the <c>TryAdd</c>, <c>GetOrAdd</c> and <c>TryRemove</c> helpers.
/// </summary>
/// <remarks>
/// <para>
/// This class uses C# 14 extension members to provide convenient methods that automatically infer
/// the property key name from the caller's argument expression using <see cref="CallerArgumentExpressionAttribute"/>.
/// </para>
/// <para>
/// These extension methods simplify common scenarios where the key name matches the variable name,
/// reducing boilerplate code when working with property bags.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Instead of creating a key explicitly:
/// var userNameKey = new PropertyBagKey&lt;string&gt;("userName");
/// propertyBag.SetValue(userNameKey, userName);
///
/// // You can use the extension method which infers the key name:
/// propertyBag.SetValue(userName); // Key name is automatically "userName"
/// </code>
/// </example>
/// <seealso cref="IPropertyBag"/>
/// <seealso cref="ReadOnlyPropertyBagExtensions"/>
/// <seealso cref="PropertyBagKey{T}"/>
[PublicAPI]
public static class PropertyBagExtensions
{
    /// <summary>
    /// Extension block for <see cref="IPropertyBag"/> providing convenient methods for setting values.
    /// </summary>
    /// <param name="bag">The <see cref="IPropertyBag"/> instance to extend.</param>
    extension(IPropertyBag bag)
    {
        /// <summary>
        /// Sets a strongly typed value in the property bag, automatically inferring the key name
        /// from the caller's argument expression.
        /// </summary>
        /// <param name="value">
        /// The strongly typed value to store in the property bag.
        /// This value may be <c>null</c> for reference types and nullable value types.
        /// </param>
        /// <param name="name">
        /// The name of the key. This parameter is automatically populated by the compiler using
        /// <see cref="CallerArgumentExpressionAttribute"/> based on the expression passed to <paramref name="value"/>.
        /// Typically, you should not provide this parameter explicitly.
        /// </param>
        /// <typeparam name="T">The type of the value to store in the property bag.</typeparam>
        /// <returns>The current <see cref="IPropertyBag"/> instance for method chaining.</returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        /// <remarks>
        /// <para>
        /// This method provides a convenient way to set values without explicitly creating a
        /// <see cref="PropertyBagKey{T}"/>. The key name is inferred from the variable or expression
        /// passed as the <paramref name="value"/> argument.
        /// </para>
        /// <para>
        /// If the value already exists for the inferred key, it will be overwritten.
        /// </para>
        /// <para>
        /// Setting a <c>null</c> value explicitly stores <c>null</c> in the property bag. This is different
        /// from removing the key entirely. When <see cref="IReadOnlyPropertyBag.TryGetValue{T}"/> is called
        /// for a key with a stored <c>null</c> value, it returns <c>true</c> with the output value set to <c>null</c>.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var connectionString = "Server=localhost;Database=test";
        /// var timeout = TimeSpan.FromSeconds(30);
        ///
        /// // Keys are inferred as "connectionString" and "timeout"
        /// propertyBag
        ///     .SetValue(connectionString)
        ///     .SetValue(timeout);
        /// </code>
        /// </example>
        public IPropertyBag SetValue<T>(
            T? value,
            [CallerArgumentExpression(nameof(value))] string? name = null
        )
        {
            ArgumentNullException.ThrowIfNull(bag);
            return bag.SetValue(new PropertyBagKey<T>(name ?? string.Empty), value);
        }

        /// <summary>
        /// Sets a strongly typed value in the property bag only when the bag holds no value for the key.
        /// </summary>
        /// <param name="key">The key of the value to add.</param>
        /// <param name="value">
        /// The value to store when the key is missing. This value may be <c>null</c> for reference types
        /// and nullable value types.
        /// </param>
        /// <typeparam name="T">The type of the value to store in the property bag.</typeparam>
        /// <returns>
        /// <c>true</c> if the value was stored; <c>false</c> if the bag already held a value for
        /// <paramref name="key"/>, which is left unchanged.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        /// <remarks>
        /// <para>
        /// A stored <c>null</c> counts as a value, so the key is not added again.
        /// </para>
        /// <para>
        /// This method composes <see cref="IReadOnlyPropertyBag.TryGetValue{T}"/> and
        /// <see cref="IPropertyBag.SetValue{T}"/>, so it is not atomic. <c>ConcurrentPropertyBag</c> has
        /// an atomic version.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var key = new PropertyBagKey&lt;string&gt;("userName");
        /// var added = propertyBag.TryAdd(key, "alice");
        /// </code>
        /// </example>
        public bool TryAdd<T>(PropertyBagKey<T> key, T? value)
        {
            ArgumentNullException.ThrowIfNull(bag);

            if (bag.TryGetValue<T>(key, out _))
            {
                return false;
            }

            bag.SetValue(key, value);
            return true;
        }

        /// <summary>
        /// Retrieves the value stored for the key, or creates it with a factory and stores it.
        /// </summary>
        /// <param name="key">The key of the value to retrieve or add.</param>
        /// <param name="valueFactory">The function that creates the value when the bag holds none.</param>
        /// <typeparam name="T">The type of the value to retrieve or store.</typeparam>
        /// <returns>
        /// The value already stored for <paramref name="key"/>, which may be <c>null</c>; otherwise, the value
        /// that <paramref name="valueFactory"/> created and this method stored.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The property bag or <paramref name="valueFactory"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        /// <para>
        /// The factory is not called when the bag already holds a value, a stored <c>null</c> included.
        /// </para>
        /// <para>
        /// This method composes <see cref="IReadOnlyPropertyBag.TryGetValue{T}"/> and
        /// <see cref="IPropertyBag.SetValue{T}"/>, so it is not atomic. <c>ConcurrentPropertyBag</c> has
        /// an atomic version.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var key = new PropertyBagKey&lt;string&gt;("userName");
        /// var name = propertyBag.GetOrAdd(key, k => k.Name.ToUpperInvariant());
        /// </code>
        /// </example>
        public T? GetOrAdd<T>(PropertyBagKey<T> key, Func<PropertyBagKey<T>, T?> valueFactory)
        {
            ArgumentNullException.ThrowIfNull(bag);
            ArgumentNullException.ThrowIfNull(valueFactory);

            if (bag.TryGetValue<T>(key, out var existing))
            {
                return existing;
            }

            var value = valueFactory(key);
            bag.SetValue(key, value);
            return value;
        }

        /// <summary>
        /// Removes the value stored for the key and returns it.
        /// </summary>
        /// <param name="key">The key of the value to remove.</param>
        /// <param name="value">
        /// When this method returns, contains the removed value if the key was found; otherwise, the default
        /// value for type <typeparamref name="T"/>.
        /// </param>
        /// <typeparam name="T">The type of the value to remove from the property bag.</typeparam>
        /// <returns>
        /// <c>true</c> if the bag held a value for <paramref name="key"/> and it was removed; otherwise,
        /// <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        /// <remarks>
        /// This method composes <see cref="IReadOnlyPropertyBag.TryGetValue{T}"/> and
        /// <see cref="IPropertyBag.Remove"/>, so it is not atomic. <c>ConcurrentPropertyBag</c> has
        /// an atomic version.
        /// </remarks>
        /// <example>
        /// <code>
        /// var key = new PropertyBagKey&lt;string&gt;("userName");
        /// if (propertyBag.TryRemove(key, out var removed))
        /// {
        ///     Console.WriteLine($"Removed {removed}");
        /// }
        /// </code>
        /// </example>
        public bool TryRemove<T>(PropertyBagKey<T> key, out T? value)
        {
            ArgumentNullException.ThrowIfNull(bag);

            if (bag.TryGetValue(key, out value))
            {
                bag.Remove(key);
                return true;
            }

            return false;
        }
    }
}
