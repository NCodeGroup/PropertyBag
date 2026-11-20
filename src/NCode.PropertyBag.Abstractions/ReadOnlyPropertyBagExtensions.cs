// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides the <see cref="IReadOnlyPropertyBag"/> extensions that read values: one that names a key after the
/// caller's argument expression, and the <c>GetValue</c>, <c>GetValueOrDefault</c> and <c>ContainsKey</c> helpers.
/// </summary>
/// <remarks>
/// Every helper reads through <see cref="IReadOnlyPropertyBag.TryGetValue{T}"/>, so a stored <c>null</c>
/// counts as a value that is present.
/// </remarks>
[PublicAPI]
public static class ReadOnlyPropertyBagExtensions
{
    /// <summary>
    /// Extension block for <see cref="IReadOnlyPropertyBag"/> providing convenient methods for retrieving values.
    /// </summary>
    /// <param name="bag">The <see cref="IReadOnlyPropertyBag"/> instance to extend.</param>
    extension(IReadOnlyPropertyBag bag)
    {
        /// <summary>
        /// Attempts to retrieve a strongly typed value from the property bag, automatically inferring
        /// the key name from the caller's argument expression.
        /// </summary>
        /// <param name="value">
        /// When this method returns, contains the strongly typed value associated with the inferred key
        /// if the key is found; otherwise, the default value for type <typeparamref name="T"/>.
        /// This parameter is passed uninitialized.
        /// Note that the value may be <c>null</c> even when the method returns <c>true</c>,
        /// if <c>null</c> was explicitly stored for the key.
        /// </param>
        /// <param name="name">
        /// The name of the key. This parameter is automatically populated by the compiler using
        /// <see cref="CallerArgumentExpressionAttribute"/> based on the expression passed to <paramref name="value"/>.
        /// Typically, you should not provide this parameter explicitly.
        /// </param>
        /// <typeparam name="T">The type of the value to retrieve from the property bag.</typeparam>
        /// <returns>
        /// <c>true</c> if the property bag contains a value with the inferred key; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        /// <remarks>
        /// <para>
        /// This method provides a convenient way to retrieve values without explicitly creating a
        /// <see cref="PropertyBagKey{T}"/>. The key name is inferred from the variable passed as
        /// the <paramref name="value"/> argument.
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
        /// // Assuming a value was stored with key "userName"
        /// // Note: declare variable first - inline declaration (out string? x) won't infer the key name correctly
        /// string? userName;
        /// if (propertyBag.TryGetValue(out userName))
        /// {
        ///     // Key exists, but userName could be null if null was stored
        ///     Console.WriteLine($"User: {userName ?? "(not set)"}");
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="IReadOnlyPropertyBag.TryGetValue{T}"/>
        public bool TryGetValue<T>(
            out T? value,
            [CallerArgumentExpression(nameof(value))] string? name = null
        )
        {
            ArgumentNullException.ThrowIfNull(bag);
            return bag.TryGetValue(new PropertyBagKey<T>(name ?? string.Empty), out value);
        }

        /// <summary>
        /// Retrieves a strongly typed value from the property bag, and throws when the bag holds none.
        /// </summary>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <typeparam name="T">The type of the value to retrieve from the property bag.</typeparam>
        /// <returns>
        /// The value associated with <paramref name="key"/>. It may be <c>null</c> when <c>null</c>
        /// was explicitly stored for the key.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        /// <exception cref="KeyNotFoundException">The property bag holds no value for <paramref name="key"/>.</exception>
        /// <remarks>
        /// A stored <c>null</c> counts as a value, so it is returned instead of throwing.
        /// Use <c>GetValueOrDefault</c> or <see cref="IReadOnlyPropertyBag.TryGetValue{T}"/> to avoid the exception.
        /// </remarks>
        /// <example>
        /// <code>
        /// var key = new PropertyBagKey&lt;string&gt;("userName");
        /// propertyBag.SetValue(key, "alice");
        /// var name = propertyBag.GetValue(key);
        /// </code>
        /// </example>
        public T? GetValue<T>(PropertyBagKey<T> key)
        {
            ArgumentNullException.ThrowIfNull(bag);

            if (bag.TryGetValue<T>(key, out var value))
            {
                return value;
            }

            throw new KeyNotFoundException($"The property bag holds no value for the key '{key}'.");
        }

        /// <summary>
        /// Retrieves a strongly typed value from the property bag, or a default value when the bag holds none.
        /// </summary>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <param name="defaultValue">The value to return when the bag holds no value for <paramref name="key"/>.</param>
        /// <typeparam name="T">The type of the value to retrieve from the property bag.</typeparam>
        /// <returns>
        /// The value associated with <paramref name="key"/> when the bag holds one, which may be <c>null</c>
        /// when <c>null</c> was explicitly stored; otherwise, <paramref name="defaultValue"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        /// <remarks>
        /// A stored <c>null</c> counts as a value, so it is returned instead of <paramref name="defaultValue"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// var key = new PropertyBagKey&lt;int&gt;("retries");
        /// var retries = propertyBag.GetValueOrDefault(key, 3);
        /// </code>
        /// </example>
        public T? GetValueOrDefault<T>(PropertyBagKey<T> key, T? defaultValue = default)
        {
            ArgumentNullException.ThrowIfNull(bag);

            return bag.TryGetValue<T>(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Determines whether the property bag holds a value for the specified key.
        /// </summary>
        /// <param name="key">The key to look for.</param>
        /// <typeparam name="T">The type of the value stored for the key.</typeparam>
        /// <returns>
        /// <c>true</c> if the property bag holds a value for <paramref name="key"/>, a stored <c>null</c> included;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        /// <example>
        /// <code>
        /// var key = new PropertyBagKey&lt;string&gt;("userName");
        /// var present = propertyBag.ContainsKey(key);
        /// </code>
        /// </example>
        public bool ContainsKey<T>(PropertyBagKey<T> key)
        {
            ArgumentNullException.ThrowIfNull(bag);

            return bag.TryGetValue<T>(key, out _);
        }
    }
}
