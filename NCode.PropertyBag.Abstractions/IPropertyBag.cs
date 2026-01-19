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
/// Provides a strongly typed, mutable collection of properties that can be accessed by key.
/// </summary>
/// <remarks>
/// <para>
/// This interface extends <see cref="IReadOnlyPropertyBag"/> with methods to modify the property bag,
/// including setting values, removing values, and creating scoped values that are automatically
/// removed when the scope is disposed.
/// </para>
/// <para>
/// All mutating methods return the <see cref="IPropertyBag"/> instance to support fluent method chaining.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var key = new PropertyBagKey&lt;string&gt;("UserName");
/// propertyBag.Set(key, "Alice");
///
/// if (propertyBag.TryGetValue(key, out var userName))
/// {
///     Console.WriteLine(userName); // Output: Alice
/// }
/// </code>
/// </example>
/// <seealso cref="IReadOnlyPropertyBag"/>
/// <seealso cref="IPropertyBagScope"/>
/// <seealso cref="PropertyBagKey{T}"/>
[PublicAPI]
public interface IPropertyBag : IReadOnlyPropertyBag
{
    /// <summary>
    /// Sets a strongly typed value for the specified <paramref name="key"/> in the property bag.
    /// </summary>
    /// <param name="key">The <see cref="PropertyBagKey{T}"/> that identifies the value to set.</param>
    /// <param name="value">
    /// The strongly typed value to associate with the specified key.
    /// This value may be <c>null</c> for reference types and nullable value types.
    /// </param>
    /// <typeparam name="T">The type of the value to set in the property bag.</typeparam>
    /// <returns>The current <see cref="IPropertyBag"/> instance for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// If a value with the same key already exists, it will be overwritten with the new value.
    /// </para>
    /// <para>
    /// This method returns the same <see cref="IPropertyBag"/> instance to allow fluent method chaining,
    /// enabling multiple values to be set in a single expression.
    /// </para>
    /// <para>
    /// Setting a <c>null</c> value explicitly stores <c>null</c> in the property bag. This is different
    /// from removing the key entirely. When <see cref="IReadOnlyPropertyBag.TryGetValue{T}"/> is called
    /// for a key with a stored <c>null</c> value, it returns <c>true</c> with the output value set to <c>null</c>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var nameKey = new PropertyBagKey&lt;string&gt;("Name");
    /// var ageKey = new PropertyBagKey&lt;int&gt;("Age");
    /// var optionalKey = new PropertyBagKey&lt;string?&gt;("Optional");
    ///
    /// propertyBag
    ///     .Set(nameKey, "Alice")
    ///     .Set(ageKey, 30)
    ///     .Set(optionalKey, null); // Explicitly stores null
    /// </code>
    /// </example>
    IPropertyBag Set<T>(PropertyBagKey<T> key, T? value);

    /// <summary>
    /// Removes the value associated with the specified <paramref name="key"/> from the property bag.
    /// </summary>
    /// <param name="key">The <see cref="PropertyBagKey"/> that identifies the value to remove.</param>
    /// <returns>The current <see cref="IPropertyBag"/> instance for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// If the specified key does not exist in the property bag, this method has no effect.
    /// </para>
    /// <para>
    /// This method returns the same <see cref="IPropertyBag"/> instance to allow fluent method chaining.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var key = new PropertyBagKey&lt;string&gt;("TempValue");
    /// propertyBag.Set(key, "temporary");
    ///
    /// // Later, remove the value
    /// propertyBag.Remove(key);
    /// </code>
    /// </example>
    IPropertyBag Remove(PropertyBagKey key);

    /// <summary>
    /// Temporarily sets a strongly typed value for the specified <paramref name="key"/> in the property bag
    /// that will be automatically removed or restored when the returned <see cref="IPropertyBagScope"/> is disposed.
    /// </summary>
    /// <param name="key">The <see cref="PropertyBagKey{T}"/> that identifies the value to set.</param>
    /// <param name="value">
    /// The strongly typed value to temporarily associate with the specified key.
    /// This value may be <c>null</c> for reference types and nullable value types.
    /// </param>
    /// <typeparam name="T">The type of the value to set in the property bag.</typeparam>
    /// <returns>
    /// An <see cref="IPropertyBagScope"/> instance that, when disposed, will remove the scoped value
    /// or restore the previous value if one existed.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is useful for temporarily overriding a value within a specific scope, such as
    /// within a <c>using</c> block. When the returned scope is disposed, the property bag is
    /// restored to its previous state for that key.
    /// </para>
    /// <para>
    /// If a value already exists for the specified key, disposing the scope will restore the
    /// original value. If no value existed, disposing the scope will remove the key entirely.
    /// </para>
    /// <para>
    /// A <c>null</c> value can be used to temporarily set a key to <c>null</c> within the scope.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var cultureKey = new PropertyBagKey&lt;string?&gt;("Culture");
    /// propertyBag.Set(cultureKey, "en-US");
    ///
    /// using (propertyBag.Scope(cultureKey, "fr-FR"))
    /// {
    ///     // Within this scope, Culture is "fr-FR"
    ///     propertyBag.TryGetValue(cultureKey, out var culture); // culture == "fr-FR"
    /// }
    ///
    /// // After the scope is disposed, Culture is restored to "en-US"
    /// propertyBag.TryGetValue(cultureKey, out var restored); // restored == "en-US"
    /// </code>
    /// </example>
    /// <seealso cref="IPropertyBagScope"/>
    IPropertyBagScope Scope<T>(PropertyBagKey<T> key, T? value);
}
