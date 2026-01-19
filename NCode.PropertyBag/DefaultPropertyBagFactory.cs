#region Copyright Preamble

// Copyright @ 2025 NCode Group
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
/// Provides the default implementation of the <see cref="IPropertyBagFactory"/> interface
/// that creates <see cref="DefaultPropertyBag"/> instances.
/// </summary>
/// <remarks>
/// <para>
/// This factory class provides a simple way to create new <see cref="DefaultPropertyBag"/> instances.
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
/// <seealso cref="DefaultPropertyBag"/>
/// <seealso cref="PropertyBagFactory"/>
[PublicAPI]
public class DefaultPropertyBagFactory : IPropertyBagFactory
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
    /// <returns>A new <see cref="DefaultPropertyBag"/> instance with no initial values.</returns>
    public IPropertyBag Create() => new DefaultPropertyBag();
}
