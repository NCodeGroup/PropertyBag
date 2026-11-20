#region Copyright Preamble

// Copyright @ 2024 NCode Group
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
/// Represents a disposable scope that temporarily associates a value with a key in an <see cref="IPropertyBag"/>,
/// automatically restoring the previous state when the scope is disposed.
/// </summary>
/// <remarks>
/// <para>
/// This interface is returned by <see cref="IPropertyBag.Scope{T}"/> and provides a mechanism for
/// temporarily overriding values in a property bag within a defined scope, such as a <c>using</c> block.
/// </para>
/// <para>
/// When disposed, the scope will either restore the previous value that existed before the scope was created,
/// or remove the key entirely if no value existed previously.
/// </para>
/// <para>
/// This pattern is useful for scenarios such as:
/// <list type="bullet">
///   <item><description>Temporarily changing configuration values during a specific operation</description></item>
///   <item><description>Setting context-specific values that should not persist beyond a method call</description></item>
///   <item><description>Implementing ambient context patterns with automatic cleanup</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var cultureKey = new PropertyBagKey&lt;string&gt;("Culture");
/// propertyBag.Set(cultureKey, "en-US");
///
/// using (IPropertyBagScope scope = propertyBag.Scope(cultureKey, "fr-FR"))
/// {
///     // Within this scope, Culture is "fr-FR"
///     Console.WriteLine(scope.Key.Name); // Output: Culture
///     Console.WriteLine(scope.PropertyBag == propertyBag); // Output: True
/// }
/// // After disposal, Culture is automatically restored to "en-US"
/// </code>
/// </example>
/// <seealso cref="IPropertyBag"/>
/// <seealso cref="IPropertyBag.Scope{T}"/>
/// <seealso cref="PropertyBagKey"/>
[PublicAPI]
public interface IPropertyBagScope : IDisposable
{
    /// <summary>
    /// Gets the <see cref="IPropertyBag"/> instance that this scope is associated with.
    /// </summary>
    /// <value>The property bag containing the scoped value.</value>
    /// <remarks>
    /// This property provides access to the underlying property bag, allowing you to perform
    /// additional operations within the scope if needed.
    /// </remarks>
    IPropertyBag PropertyBag { get; }

    /// <summary>
    /// Gets the <see cref="PropertyBagKey"/> that identifies the scoped value.
    /// </summary>
    /// <value>The non-generic key representing the type and name of the scoped value.</value>
    /// <remarks>
    /// <para>
    /// This property returns the non-generic <see cref="PropertyBagKey"/> which contains both
    /// the <see cref="PropertyBagKey.Type"/> and <see cref="PropertyBagKey.Name"/> of the scoped value.
    /// </para>
    /// <para>
    /// When the scope is disposed, this key is used to either restore the previous value
    /// or remove the entry from the property bag.
    /// </para>
    /// </remarks>
    PropertyBagKey Key { get; }
}
