// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

[Collection(StaticFactoryCollection.Name)]
public class PropertyBagFactoryTests
{
    #region DefaultFactory Property Tests

    [Fact]
    public void DefaultFactory_WhenNotSet_ReturnsDefaultSingleton()
    {
        var factory = PropertyBagFactory.DefaultFactory;

        Assert.Same(DefaultPropertyBagFactory.Singleton, factory);
    }

    [Fact]
    public void DefaultFactory_WhenSet_ReturnsCustomFactory()
    {
        var customFactory = new DefaultPropertyBagFactory();

        try
        {
            PropertyBagFactory.DefaultFactory = customFactory;

            var factory = PropertyBagFactory.DefaultFactory;

            Assert.Same(customFactory, factory);
        }
        finally
        {
            PropertyBagFactory.DefaultFactory = null!;
        }
    }

    [Fact]
    public void DefaultFactory_WhenResetToNull_ReturnsDefaultSingleton()
    {
        PropertyBagFactory.DefaultFactory = new DefaultPropertyBagFactory();
        PropertyBagFactory.DefaultFactory = null!;

        var factory = PropertyBagFactory.DefaultFactory;

        Assert.Same(DefaultPropertyBagFactory.Singleton, factory);
    }

    #endregion

    #region Create Tests

    [Fact]
    public void Create_WhenCalledTwice_ReturnsDistinctEmptyBags()
    {
        var first = PropertyBagFactory.Create();
        var second = PropertyBagFactory.Create();

        var firstHasValue = first.TryGetValue(new PropertyBagKey<string>("Key"), out _);

        Assert.NotSame(first, second);
        Assert.False(firstHasValue);
    }

    [Fact]
    public void Create_WhenDefaultFactoryIsCustom_ReturnsTheCustomFactorysBag()
    {
        var bag = new DefaultPropertyBag();

        try
        {
            PropertyBagFactory.DefaultFactory = new FixedFactory(bag);

            var created = PropertyBagFactory.Create();

            Assert.Same(bag, created);
        }
        finally
        {
            PropertyBagFactory.DefaultFactory = null!;
        }
    }

    #endregion

    private sealed class FixedFactory(IPropertyBag bag) : IPropertyBagFactory
    {
        public IPropertyBag Create() => bag;
    }
}
