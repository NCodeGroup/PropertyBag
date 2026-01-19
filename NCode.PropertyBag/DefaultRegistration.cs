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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NCode.PropertyBag;

/// <summary>
/// Provides extension methods for registering PropertyBag services with the dependency injection container.
/// </summary>
/// <remarks>
/// <para>
/// This class uses C# 14 extension types to provide extension methods on <see cref="IServiceCollection"/>
/// for convenient service registration.
/// </para>
/// <para>
/// The registration uses <see cref="ServiceCollectionDescriptorExtensions.TryAddSingleton{TService,TImplementation}(IServiceCollection)"/>
/// to avoid overwriting existing registrations, allowing consumers to provide their own implementations if desired.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // In Startup.cs or Program.cs
/// services.AddPropertyBag();
///
/// // Then inject IPropertyBagFactory where needed
/// public class MyService
/// {
///     private readonly IPropertyBagFactory _factory;
///
///     public MyService(IPropertyBagFactory factory)
///     {
///         _factory = factory;
///     }
/// }
/// </code>
/// </example>
/// <seealso cref="IPropertyBagFactory"/>
/// <seealso cref="DefaultPropertyBagFactory"/>
[PublicAPI]
public static class DefaultRegistration
{
    /// <summary>
    /// Extension block for <see cref="IServiceCollection"/> providing PropertyBag service registration methods.
    /// </summary>
    /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add services to.</param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the default PropertyBag services with the dependency injection container.
        /// </summary>
        /// <returns>The <see cref="IServiceCollection"/> instance for method chaining.</returns>
        /// <remarks>
        /// <para>
        /// This method registers <see cref="DefaultPropertyBagFactory"/> as the implementation for
        /// <see cref="IPropertyBagFactory"/> with a singleton lifetime.
        /// </para>
        /// <para>
        /// The registration uses <c>TryAddSingleton</c>, which means it will not overwrite an existing
        /// registration for <see cref="IPropertyBagFactory"/>. This allows you to register a custom
        /// implementation before calling this method if needed.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// // Basic registration
        /// services.AddPropertyBag();
        ///
        /// // Or with custom factory (must be registered before AddPropertyBag)
        /// services.AddSingleton&lt;IPropertyBagFactory, CustomPropertyBagFactory&gt;();
        /// services.AddPropertyBag(); // Will not overwrite the custom registration
        /// </code>
        /// </example>
        [PublicAPI]
        public IServiceCollection AddPropertyBag()
        {
            serviceCollection.TryAddSingleton<IPropertyBagFactory, DefaultPropertyBagFactory>();

            return serviceCollection;
        }
    }
}
