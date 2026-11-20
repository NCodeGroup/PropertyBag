// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Provides the default implementation of the <see cref="IPropertyBagFactory"/> interface
/// that creates <see cref="PropertyBagDictionary"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// This factory class provides a simple way to create new <see cref="PropertyBagDictionary"/> instances.
/// It follows the factory pattern to allow for dependency injection and testability.
/// </para>
/// <para>
/// A singleton instance is available via the <see cref="Singleton"/> property, which is also used
/// as the default factory by <see cref="PropertyBagFactory.DefaultFactory"/>.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Using the singleton instance
/// IPropertyBag bag1 = DefaultPropertyBagFactory.Singleton.Create();
///
/// // Using a new factory instance
/// var factory = new DefaultPropertyBagFactory();
/// IPropertyBag bag2 = factory.Create();
/// </code>
/// </example>
/// <seealso cref="IPropertyBagFactory"/>
/// <seealso cref="PropertyBagDictionary"/>
/// <seealso cref="PropertyBagFactory"/>
[PublicAPI]
public sealed class DefaultPropertyBagFactory : IPropertyBagFactory
{
    /// <summary>
    /// Gets or sets the singleton instance of the <see cref="DefaultPropertyBagFactory"/> class.
    /// </summary>
    /// <value>
    /// A shared <see cref="IPropertyBagFactory"/> instance that can be used application-wide.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property provides a convenient singleton instance for scenarios where dependency injection
    /// is not available or not desired. The singleton can be replaced at runtime if needed.
    /// </para>
    /// <para>
    /// This instance is used as the default factory by <see cref="PropertyBagFactory.DefaultFactory"/>
    /// when no custom factory has been configured.
    /// </para>
    /// </remarks>
    public static IPropertyBagFactory Singleton { get; set; } = new DefaultPropertyBagFactory();

    /// <inheritdoc />
    /// <returns>A new <see cref="PropertyBagDictionary"/> instance with no initial values.</returns>
    public IPropertyBag Create() => new PropertyBagDictionary();
}
