// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

public class ReadOnlyPropertyBagTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullBag_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ReadOnlyPropertyBag(null!));
    }

    #endregion

    #region TryGetValue Tests

    [Fact]
    public void TryGetValue_WithStoredValue_Delegates()
    {
        var bag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        bag.SetValue(key, "Value");
        var view = new ReadOnlyPropertyBag(bag);

        Assert.True(view.TryGetValue(key, out var value));
        Assert.Equal("Value", value);
    }

    [Fact]
    public void TryGetValue_WithNoValue_ReturnsFalse()
    {
        var view = new ReadOnlyPropertyBag(new PropertyBagDictionary());
        var key = new PropertyBagKey<string>("TestKey");

        Assert.False(view.TryGetValue(key, out var value));
        Assert.Null(value);
    }

    [Fact]
    public void TryGetValue_WithLaterChange_SeesTheChange()
    {
        var bag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        var view = new ReadOnlyPropertyBag(bag);

        bag.SetValue(key, "Later");

        Assert.True(view.TryGetValue(key, out var value));
        Assert.Equal("Later", value);
    }

    #endregion

    #region Type Tests

    [Fact]
    public void View_IsNotAssignableToPropertyBag()
    {
        var view = new ReadOnlyPropertyBag(new PropertyBagDictionary());

        Assert.IsNotAssignableFrom<IPropertyBag>(view);
    }

    #endregion

    #region Clone Tests

    [Fact]
    public void Clone_ReturnsWritableIndependentCopy()
    {
        var bag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        bag.SetValue(key, "Original");
        var view = new ReadOnlyPropertyBag(bag);

        var clone = view.Clone();
        clone.SetValue(key, "Changed");

        Assert.True(clone.TryGetValue(key, out var cloneValue));
        Assert.Equal("Changed", cloneValue);
        Assert.True(view.TryGetValue(key, out var viewValue));
        Assert.Equal("Original", viewValue);
    }

    #endregion
}
