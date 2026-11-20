// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

[Collection(StaticFactoryCollection.Name)]
public class PropertyBagFactoryTests
{
    [Fact]
    public void DefaultFactory_WhenNotSet_IsTheDefaultSingleton()
    {
        Assert.Same(DefaultPropertyBagFactory.Singleton, PropertyBagFactory.DefaultFactory);
    }

    [Fact]
    public void DefaultFactory_WhenSet_ReturnsTheCustomFactory()
    {
        var customFactory = new DefaultPropertyBagFactory();

        try
        {
            PropertyBagFactory.DefaultFactory = customFactory;

            Assert.Same(customFactory, PropertyBagFactory.DefaultFactory);
        }
        finally
        {
            PropertyBagFactory.DefaultFactory = null!;
        }
    }

    [Fact]
    public void DefaultFactory_WhenResetToNull_FallsBackToTheDefaultSingleton()
    {
        PropertyBagFactory.DefaultFactory = new DefaultPropertyBagFactory();
        PropertyBagFactory.DefaultFactory = null!;

        Assert.Same(DefaultPropertyBagFactory.Singleton, PropertyBagFactory.DefaultFactory);
    }

    [Fact]
    public void Create_ReturnsANewEmptyBag()
    {
        var first = PropertyBagFactory.Create();
        var second = PropertyBagFactory.Create();

        Assert.NotSame(first, second);
        Assert.False(first.TryGetValue(new PropertyBagKey<string>("Key"), out _));
    }

    [Fact]
    public void Create_UsesTheCustomFactory()
    {
        var bag = new DefaultPropertyBag();

        try
        {
            PropertyBagFactory.DefaultFactory = new FixedFactory(bag);

            Assert.Same(bag, PropertyBagFactory.Create());
        }
        finally
        {
            PropertyBagFactory.DefaultFactory = null!;
        }
    }

    private sealed class FixedFactory(IPropertyBag bag) : IPropertyBagFactory
    {
        public IPropertyBag Create() => bag;
    }
}
