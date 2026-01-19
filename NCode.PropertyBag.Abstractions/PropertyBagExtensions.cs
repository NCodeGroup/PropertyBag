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

using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides extension methods for the <see cref="IPropertyBag"/> and <see cref="IReadOnlyPropertyBag"/> interfaces.
/// </summary>
/// <remarks>
/// <para>
/// This class uses C# 14 extension types to provide convenient methods that automatically infer
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
/// propertyBag.Set(userNameKey, userName);
///
/// // You can use the extension method which infers the key name:
/// propertyBag.Set(userName); // Key name is automatically "userName"
/// </code>
/// </example>
/// <seealso cref="IPropertyBag"/>
/// <seealso cref="IReadOnlyPropertyBag"/>
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
        ///     .Set(connectionString)
        ///     .Set(timeout);
        /// </code>
        /// </example>
        public IPropertyBag Set<T>(
            T? value,
            [CallerArgumentExpression(nameof(value))]
            string? name = null
        ) =>
            bag.Set(new PropertyBagKey<T>(name ?? string.Empty), value);
    }

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
        /// if (propertyBag.TryGet(out userName))
        /// {
        ///     // Key exists, but userName could be null if null was stored
        ///     Console.WriteLine($"User: {userName ?? "(not set)"}");
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="IReadOnlyPropertyBag.TryGetValue{T}"/>
        public bool TryGet<T>(
            out T? value,
            [CallerArgumentExpression(nameof(value))]
            string? name = null
        ) =>
            bag.TryGetValue(new PropertyBagKey<T>(name ?? string.Empty), out value);
    }
}
