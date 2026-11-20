// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag;

/// <summary>
/// Provides the typed cast that every property bag applies to a stored value.
/// </summary>
internal static class PropertyBagValue
{
    /// <summary>
    /// Casts a stored value to <typeparamref name="T"/>, treating <c>null</c> as a present value.
    /// </summary>
    /// <typeparam name="T">The type to cast the stored value to.</typeparam>
    /// <param name="stored">The value found in the bag's storage.</param>
    /// <param name="value">
    /// When this method returns, contains the stored value as <typeparamref name="T"/>, or <c>default</c>
    /// when <paramref name="stored"/> is <c>null</c> or is not a <typeparamref name="T"/>.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="stored"/> is <c>null</c> or a <typeparamref name="T"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryCast<T>(object? stored, out T? value)
    {
        if (stored is null)
        {
            value = default;
            return true;
        }

        if (stored is T typedValue)
        {
            value = typedValue;
            return true;
        }

        value = default;
        return false;
    }
}
