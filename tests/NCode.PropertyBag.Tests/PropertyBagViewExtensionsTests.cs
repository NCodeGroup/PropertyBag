// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

public class PropertyBagViewExtensionsTests
{
    #region CreateChild Tests

    [Fact]
    public void CreateChild_WithNullBag_ThrowsArgumentNullException()
    {
        IReadOnlyPropertyBag propertyBag = null!;

        Assert.Throws<ArgumentNullException>(() => propertyBag.CreateChild());
    }

    [Fact]
    public void CreateChild_WithBag_ReturnsLayeredBagOverBag()
    {
        var propertyBag = new PropertyBagDictionary();

        var child = propertyBag.CreateChild();

        var layered = Assert.IsType<LayeredPropertyBag>(child);
        Assert.Same(propertyBag, layered.Parent);
    }

    #endregion

    #region AsReadOnly Tests

    [Fact]
    public void AsReadOnly_WithNullBag_ThrowsArgumentNullException()
    {
        IReadOnlyPropertyBag propertyBag = null!;

        Assert.Throws<ArgumentNullException>(() => propertyBag.AsReadOnly());
    }

    [Fact]
    public void AsReadOnly_WithWritableBag_WrapsBag()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        var readOnly = propertyBag.AsReadOnly();

        Assert.IsType<ReadOnlyPropertyBag>(readOnly);
        Assert.NotSame(propertyBag, readOnly);
        Assert.True(readOnly.TryGetValue(key, out var value));
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void AsReadOnly_WithReadOnlyBag_ReturnsSameInstance()
    {
        var readOnly = new ReadOnlyPropertyBag(new PropertyBagDictionary());

        var result = readOnly.AsReadOnly();

        Assert.Same(readOnly, result);
    }

    #endregion
}
