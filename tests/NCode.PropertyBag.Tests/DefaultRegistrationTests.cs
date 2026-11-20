// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;

namespace NCode.PropertyBag.Tests;

public class DefaultRegistrationTests
{
    [Fact]
    public void AddPropertyBag_RegistersTheDefaultFactoryAsSingleton()
    {
        var services = new TestServiceCollection();

        services.AddPropertyBag();

        var descriptor = Assert.Single(services);
        Assert.Equal(typeof(IPropertyBagFactory), descriptor.ServiceType);
        Assert.Equal(typeof(DefaultPropertyBagFactory), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddPropertyBag_ReturnsTheSameCollection()
    {
        IServiceCollection services = new TestServiceCollection();

        Assert.Same(services, services.AddPropertyBag());
    }

    [Fact]
    public void AddPropertyBag_DoesNotOverwriteAnExistingRegistration()
    {
        var services = new TestServiceCollection();
        services.AddSingleton<IPropertyBagFactory, CustomFactory>();

        services.AddPropertyBag();

        var descriptor = Assert.Single(services);
        Assert.Equal(typeof(CustomFactory), descriptor.ImplementationType);
    }

    [Fact]
    public void AddPropertyBag_CalledTwice_RegistersOnce()
    {
        var services = new TestServiceCollection();

        services.AddPropertyBag().AddPropertyBag();

        Assert.Single(services);
    }

    private sealed class CustomFactory : IPropertyBagFactory
    {
        public IPropertyBag Create() => new DefaultPropertyBag();
    }

    private sealed class TestServiceCollection : List<ServiceDescriptor>, IServiceCollection;
}
