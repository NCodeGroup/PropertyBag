// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides extension methods that create layered and read-only views of an <see cref="IReadOnlyPropertyBag"/>.
/// </summary>
/// <seealso cref="LayeredPropertyBag"/>
/// <seealso cref="ReadOnlyPropertyBag"/>
[PublicAPI]
public static class PropertyBagViewExtensions
{
    /// <summary>
    /// Extension block for <see cref="IReadOnlyPropertyBag"/> providing view-creating methods.
    /// </summary>
    /// <param name="propertyBag">The <see cref="IReadOnlyPropertyBag"/> to create a view of.</param>
    extension(IReadOnlyPropertyBag propertyBag)
    {
        /// <summary>
        /// Creates an empty writable <see cref="LayeredPropertyBag"/> whose reads fall back to this property bag.
        /// </summary>
        /// <returns>A new writable <see cref="IPropertyBag"/> layered over this property bag.</returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        [PublicAPI]
        public IPropertyBag CreateChild()
        {
            ArgumentNullException.ThrowIfNull(propertyBag);

            return new LayeredPropertyBag(propertyBag);
        }

        /// <summary>
        /// Returns a read-only view of this property bag.
        /// </summary>
        /// <returns>
        /// This property bag when it is already a <see cref="ReadOnlyPropertyBag"/>; otherwise a new
        /// <see cref="ReadOnlyPropertyBag"/> wrapping it.
        /// </returns>
        /// <exception cref="ArgumentNullException">The property bag is <c>null</c>.</exception>
        [PublicAPI]
        public IReadOnlyPropertyBag AsReadOnly()
        {
            ArgumentNullException.ThrowIfNull(propertyBag);

            return propertyBag as ReadOnlyPropertyBag ?? new ReadOnlyPropertyBag(propertyBag);
        }
    }
}
