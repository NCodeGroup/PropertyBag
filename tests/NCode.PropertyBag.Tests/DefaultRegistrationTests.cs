// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;

namespace NCode.PropertyBag.Tests;

public class DefaultRegistrationTests
{
    #region AddPropertyBag Tests

    [Fact]
    public async Task AddPropertyBag_WhenCalled_ResolvesTheDefaultFactory()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPropertyBag();

        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IPropertyBagFactory>();

        Assert.IsType<DefaultPropertyBagFactory>(factory);
    }

    [Fact]
    public async Task AddPropertyBag_WhenCalled_RegistersTheFactoryAsSingleton()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPropertyBag();

        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var first = serviceProvider.GetRequiredService<IPropertyBagFactory>();
        var second = serviceProvider.GetRequiredService<IPropertyBagFactory>();

        Assert.Same(first, second);
    }

    [Fact]
    public void AddPropertyBag_WhenCalled_ReturnsTheSameCollection()
    {
        var serviceCollection = new ServiceCollection();

        var result = serviceCollection.AddPropertyBag();

        Assert.Same(serviceCollection, result);
    }

    [Fact]
    public async Task AddPropertyBag_WhenFactoryAlreadyRegistered_KeepsTheExistingRegistration()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IPropertyBagFactory, CustomFactory>();
        serviceCollection.AddPropertyBag();

        await using var serviceProvider = serviceCollection.BuildServiceProvider();
        var factory = serviceProvider.GetRequiredService<IPropertyBagFactory>();

        Assert.IsType<CustomFactory>(factory);
    }

    [Fact]
    public void AddPropertyBag_WhenCalledTwice_RegistersTheFactoryOnce()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPropertyBag().AddPropertyBag();

        var descriptors = serviceCollection.Where(descriptor =>
            descriptor.ServiceType == typeof(IPropertyBagFactory)
        );

        Assert.Single(descriptors);
    }

    #endregion

    private sealed class CustomFactory : IPropertyBagFactory
    {
        public IPropertyBag Create() => new PropertyBagDictionary();
    }
}
