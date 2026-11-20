// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using JetBrains.Annotations;

namespace NCode.PropertyBag;

/// <summary>
/// Defines a factory for creating <see cref="IPropertyBag"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// This interface enables dependency injection and abstraction of property bag creation,
/// allowing different implementations to be substituted as needed.
/// </para>
/// <para>
/// The default implementation is <see cref="DefaultPropertyBagFactory"/>, which creates
/// <see cref="PropertyBagDictionary"/> instances.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Inject the factory via dependency injection
/// public class MyService
/// {
///     private readonly IPropertyBagFactory _factory;
///
///     public MyService(IPropertyBagFactory factory)
///     {
///         _factory = factory;
///     }
///
///     public IPropertyBag CreateContext()
///     {
///         return _factory.Create();
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="DefaultPropertyBagFactory"/>
/// <seealso cref="PropertyBagFactory"/>
/// <seealso cref="IPropertyBag"/>
[PublicAPI]
public interface IPropertyBagFactory
{
    /// <summary>
    /// Creates a new instance of an <see cref="IPropertyBag"/>.
    /// </summary>
    /// <returns>A new, empty <see cref="IPropertyBag"/> instance ready for use.</returns>
    /// <remarks>
    /// Each call to this method returns a new, independent property bag instance.
    /// </remarks>
    IPropertyBag Create();
}

/// <summary>
/// Provides static convenience methods for creating <see cref="IPropertyBag"/> instances
/// without requiring an injected factory.
/// </summary>
/// <remarks>
/// <para>
/// This static class wraps an <see cref="IPropertyBagFactory"/> instance and provides
/// a simple API for creating property bags in scenarios where dependency injection
/// is not available or not desired.
/// </para>
/// <para>
/// The <see cref="DefaultFactory"/> property controls which factory implementation is used.
/// By default, it uses <see cref="DefaultPropertyBagFactory.Singleton"/>, but this can be
/// replaced at runtime to customize property bag creation globally.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Simple usage with default factory
/// IPropertyBag bag = PropertyBagFactory.Create();
///
/// // Customize the default factory
/// PropertyBagFactory.DefaultFactory = new CustomPropertyBagFactory();
/// IPropertyBag customBag = PropertyBagFactory.Create();
/// </code>
/// </example>
/// <seealso cref="IPropertyBagFactory"/>
/// <seealso cref="DefaultPropertyBagFactory"/>
/// <seealso cref="IPropertyBag"/>
[PublicAPI]
public static class PropertyBagFactory
{
    /// <summary>
    /// Gets or sets the default <see cref="IPropertyBagFactory"/> instance used by the static <see cref="Create"/> method.
    /// </summary>
    /// <value>
    /// The factory instance used to create property bags. If not explicitly set, returns
    /// <see cref="DefaultPropertyBagFactory.Singleton"/>.
    /// </value>
    /// <remarks>
    /// <para>
    /// This property uses a field-backed property pattern. When the backing field is <c>null</c>,
    /// the getter returns <see cref="DefaultPropertyBagFactory.Singleton"/> as the default.
    /// </para>
    /// <para>
    /// Setting this property allows you to globally customize property bag creation for all
    /// consumers of the static <see cref="Create"/> method.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Use a custom factory implementation
    /// PropertyBagFactory.DefaultFactory = new CustomPropertyBagFactory();
    ///
    /// // Reset to default behavior
    /// PropertyBagFactory.DefaultFactory = null!;
    /// </code>
    /// </example>
    public static IPropertyBagFactory DefaultFactory
    {
        get => field ?? DefaultPropertyBagFactory.Singleton;
        set;
    }

    /// <summary>
    /// Creates a new <see cref="IPropertyBag"/> instance using the <see cref="DefaultFactory"/>.
    /// </summary>
    /// <returns>A new, empty <see cref="IPropertyBag"/> instance ready for use.</returns>
    /// <remarks>
    /// This method delegates to <see cref="IPropertyBagFactory.Create"/> on the <see cref="DefaultFactory"/> instance.
    /// </remarks>
    /// <example>
    /// <code>
    /// IPropertyBag bag = PropertyBagFactory.Create();
    /// var key = new PropertyBagKey&lt;string&gt;("UserName");
    /// bag.Set(key, "Alice");
    /// </code>
    /// </example>
    public static IPropertyBag Create() => DefaultFactory.Create();
}
