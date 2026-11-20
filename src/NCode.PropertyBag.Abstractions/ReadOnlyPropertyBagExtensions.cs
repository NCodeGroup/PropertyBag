// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides the <see cref="IReadOnlyPropertyBag"/> extension that names a key after the caller's argument expression.
/// </summary>
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
        ) => bag.TryGetValue(new PropertyBagKey<T>(name ?? string.Empty), out value);
    }
}
