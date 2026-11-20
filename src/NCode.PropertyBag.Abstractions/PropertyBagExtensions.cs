// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides the <see cref="IPropertyBag"/> extension that names a key after the caller's argument expression.
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
    }
}
