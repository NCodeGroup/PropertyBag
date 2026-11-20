// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/dependency-injection.mdx. The site's build includes each begin-snippet region in the
// page by its key (ADR-1006).
// The Static sample replaces PropertyBagFactory.DefaultFactory, which is process-wide, so the class runs alone, after
// the parallel test classes, and no other test creates a bag through the static factory while it runs.
[CollectionDefinition(nameof(DependencyInjectionSamples), DisableParallelization = true)]
[Collection(nameof(DependencyInjectionSamples))]
public class DependencyInjectionSamples
{
    // begin-snippet: SiteDependencyInjectionInject
    private static readonly PropertyBagKey<string> OrderKey = new("OrderId");

    private sealed class OrderHandler(IPropertyBagFactory factory)
    {
        public IPropertyBag Handle(string id) => factory.Create().SetValue(OrderKey, id);
    }

    // end-snippet

    private static readonly PropertyBagKey<string> TenantKey = new("Tenant");

    private sealed class TenantBagFactory : IPropertyBagFactory
    {
        public IPropertyBag Create()
        {
            IPropertyBag bag = new PropertyBagDictionary();
            return bag.SetValue(TenantKey, "contoso");
        }
    }

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

    [Fact]
    public async Task SiteDependencyInjectionInjectSample_Runs()
    {
        var services = new ServiceCollection();
        services.AddPropertyBag();
        await using ServiceProvider provider = services.BuildServiceProvider();

        var factory = provider.GetRequiredService<IPropertyBagFactory>();
        var handler = new OrderHandler(factory);

        IPropertyBag first = handler.Handle("17");
        IPropertyBag second = handler.Handle("18");

        bool firstFound = first.TryGetValue(OrderKey, out string? firstId);
        Assert.True(firstFound);
        Assert.Equal("17", firstId);
        bool secondFound = second.TryGetValue(OrderKey, out string? secondId);
        Assert.True(secondFound);
        Assert.Equal("18", secondId);
        Assert.NotSame(first, second);
    }

    [Fact]
    public async Task SiteDependencyInjectionCustomFactorySample_Runs()
    {
        var services = new ServiceCollection();

        // begin-snippet: SiteDependencyInjectionCustomFactory
        services.AddSingleton<IPropertyBagFactory, TenantBagFactory>();
        services.AddPropertyBag(); // Keeps TenantBagFactory
        // end-snippet

        await using ServiceProvider provider = services.BuildServiceProvider();
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

        bool plainFound = plain.TryGetValue(TenantKey, out _);
        Assert.False(plainFound);
        bool seededFound = seeded.TryGetValue(TenantKey, out string? tenant);
        Assert.True(seededFound);
        Assert.Equal("contoso", tenant);
        Assert.Same(
            DefaultPropertyBagFactory.Singleton,
            PropertyBagFactory.DefaultFactory
        );
    }
}
