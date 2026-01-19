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

namespace NCode.PropertyBag;

/// <summary>
/// Provides the default implementation of the <see cref="IPropertyBagScope"/> interface
/// that removes a value from a property bag when disposed.
/// </summary>
/// <remarks>
/// <para>
/// This class is created by <see cref="DefaultPropertyBag.Scope{T}"/> and provides automatic
/// cleanup of temporarily scoped values when the scope is disposed.
/// </para>
/// <para>
/// When <see cref="Dispose"/> is called, the value associated with the <see cref="Key"/> is
/// removed from the <see cref="PropertyBag"/>.
/// </para>
/// <para>
/// This class is typically used within a <c>using</c> statement to ensure proper cleanup:
/// </para>
/// </remarks>
/// <example>
/// <code>
/// var key = new PropertyBagKey&lt;string&gt;("TempValue");
///
/// using (new DefaultPropertyBagScope(propertyBag, key))
/// {
///     // The value exists in the property bag
///     propertyBag.TryGetValue(key, out var value);
/// }
/// // After disposal, the value has been removed
/// </code>
/// </example>
/// <param name="propertyBag">The <see cref="IPropertyBag"/> instance that contains the scoped value.</param>
/// <param name="key">The <see cref="PropertyBagKey"/> that identifies the scoped value to remove on disposal.</param>
/// <seealso cref="IPropertyBagScope"/>
/// <seealso cref="IPropertyBag.Scope{T}"/>
/// <seealso cref="DefaultPropertyBag"/>
public class DefaultPropertyBagScope(IPropertyBag propertyBag, PropertyBagKey key) : IPropertyBagScope
{
    /// <inheritdoc />
    /// <value>The property bag instance that was passed to the constructor.</value>
    public IPropertyBag PropertyBag { get; } = propertyBag;

    /// <inheritdoc />
    /// <value>The key that was passed to the constructor, identifying the value to remove on disposal.</value>
    public PropertyBagKey Key { get; } = key;

    /// <summary>
    /// Removes the scoped value from the property bag and suppresses finalization.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method removes the value associated with <see cref="Key"/> from the <see cref="PropertyBag"/>
    /// by calling <see cref="IPropertyBag.Remove"/>.
    /// </para>
    /// <para>
    /// This method calls <see cref="GC.SuppressFinalize"/> to prevent the finalizer from running,
    /// following the standard dispose pattern.
    /// </para>
    /// <para>
    /// This method can be called multiple times safely; subsequent calls will have no effect
    /// since the key has already been removed.
    /// </para>
    /// </remarks>
    public void Dispose()
    {
        PropertyBag.Remove(Key);
        GC.SuppressFinalize(this);
    }
}
