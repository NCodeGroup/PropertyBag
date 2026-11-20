// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/dependency-injection.mdx. The site's build includes each begin-snippet region in the page by its key (ADR-1004).
// The Static sample replaces PropertyBagFactory.DefaultFactory, which is process-wide, so the class runs alone, after the parallel test classes, and no other test creates a bag through the static factory while it runs.
[CollectionDefinition(nameof(DependencyInjectionSamples), DisableParallelization = true)]
[Collection(nameof(DependencyInjectionSamples))]
public class DependencyInjectionSamples
{
    [Fact]
    public void SiteDependencyInjectionRegisterSample_Runs()
    {
        // begin-snippet: SiteDependencyInjectionRegister
        var services = new ServiceCollection();

        services.AddPropertyBag();
        // end-snippet

        ServiceDescriptor descriptor = Assert.Single(services, d =>
            d.ServiceType == typeof(IPropertyBagFactory)
        );
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.Equal(typeof(DefaultPropertyBagFactory), descriptor.ImplementationType);
    }

    // begin-snippet: SiteDependencyInjectionInject
    private static readonly PropertyBagKey<string> OrderIdKey = new("OrderId");

    private sealed class OrderHandler(IPropertyBagFactory factory)
    {
        public IPropertyBag Handle(string orderId) =>
            factory.Create().Set(OrderIdKey, orderId);
    }

    // end-snippet

    [Fact]
    public void SiteDependencyInjectionInjectSample_Runs()
    {
        var services = new ServiceCollection();
        services.AddPropertyBag();
        using ServiceProvider provider = services.BuildServiceProvider();

        var handler = new OrderHandler(
            provider.GetRequiredService<IPropertyBagFactory>()
        );

        IPropertyBag first = handler.Handle("17");
        IPropertyBag second = handler.Handle("18");

        Assert.True(first.TryGetValue(OrderIdKey, out string? firstId));
        Assert.Equal("17", firstId);
        Assert.True(second.TryGetValue(OrderIdKey, out string? secondId));
        Assert.Equal("18", secondId);
        Assert.NotSame(first, second);
    }

    private static readonly PropertyBagKey<string> TenantKey = new("Tenant");

    private sealed class TenantBagFactory : IPropertyBagFactory
    {
        public IPropertyBag Create()
        {
            IPropertyBag bag = new DefaultPropertyBag();
            return bag.Set(TenantKey, "contoso");
        }
    }

    [Fact]
    public void SiteDependencyInjectionCustomFactorySample_Runs()
    {
        var services = new ServiceCollection();

        // begin-snippet: SiteDependencyInjectionCustomFactory
        services.AddSingleton<IPropertyBagFactory, TenantBagFactory>();
        services.AddPropertyBag(); // Keeps TenantBagFactory
        // end-snippet

        using ServiceProvider provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IPropertyBagFactory>();

        Assert.IsType<TenantBagFactory>(factory);
    }

    [Fact]
    public void SiteDependencyInjectionStaticSample_Runs()
    {
        // begin-snippet: SiteDependencyInjectionStatic
        // Empty: DefaultPropertyBagFactory.Singleton made it
        IPropertyBag plain = PropertyBagFactory.Create();

        IPropertyBag seeded;
        try
        {
            PropertyBagFactory.DefaultFactory = new TenantBagFactory();
            seeded = PropertyBagFactory.Create(); // Holds "contoso"
        }
        finally
        {
            PropertyBagFactory.DefaultFactory = DefaultPropertyBagFactory.Singleton;
        }
        // end-snippet

        Assert.False(plain.TryGetValue(TenantKey, out _));
        Assert.True(seeded.TryGetValue(TenantKey, out string? tenant));
        Assert.Equal("contoso", tenant);
        Assert.Same(
            DefaultPropertyBagFactory.Singleton,
            PropertyBagFactory.DefaultFactory
        );
    }
}
