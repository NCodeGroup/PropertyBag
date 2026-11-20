// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag;

/// <summary>
/// Defines the contract that a writable property bag implements so that a <see cref="DefaultPropertyBagScope"/>
/// can put back the state a key held before the scope set its value.
/// </summary>
internal interface IScopeRestorer : IPropertyBag
{
    /// <summary>
    /// Puts back the state that a key held before a scope set its value.
    /// </summary>
    /// <param name="key">The key whose state to put back.</param>
    /// <param name="hadValue">
    /// <c>true</c> if the key held a value before the scope set it; otherwise, <c>false</c>.
    /// </param>
    /// <param name="previousValue">
    /// The value the key held before the scope set it, when <paramref name="hadValue"/> is <c>true</c>.
    /// </param>
    void Restore(PropertyBagKey key, bool hadValue, object? previousValue);
}
