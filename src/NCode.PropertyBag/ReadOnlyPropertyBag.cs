// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides a read-only view of another <see cref="IReadOnlyPropertyBag"/>.
/// </summary>
/// <remarks>
/// <para>
/// The view implements only <see cref="IReadOnlyPropertyBag"/>, so a caller cannot cast it to
/// <see cref="IPropertyBag"/> to change the wrapped bag.
/// </para>
/// <para>
/// The view reads through to the wrapped bag, so it shows every later change to that bag.
/// </para>
/// <para>
/// <see cref="Clone"/> returns the wrapped bag's clone, which is a writable, independent copy.
/// </para>
/// </remarks>
/// <seealso cref="IReadOnlyPropertyBag"/>
[PublicAPI]
public sealed class ReadOnlyPropertyBag(IReadOnlyPropertyBag propertyBag) : IReadOnlyPropertyBag
{
    private IReadOnlyPropertyBag Inner { get; } =
        propertyBag ?? throw new ArgumentNullException(nameof(propertyBag));

    /// <inheritdoc />
    public IPropertyBag Clone() => Inner.Clone();

    /// <inheritdoc />
    public bool TryGetValue<T>(PropertyBagKey<T> key, out T? value) =>
        Inner.TryGetValue(key, out value);
}
