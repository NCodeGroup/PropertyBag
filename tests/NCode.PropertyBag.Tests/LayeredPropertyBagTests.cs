// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

public class LayeredPropertyBagTests
{
    private sealed class Cell(int number) : ICloneable
    {
        public int Number { get; } = number;

        public object Clone() => new Cell(Number);
    }

    private sealed class WrongTypeClone : ICloneable
    {
        public object Clone() => "not a WrongTypeClone";
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_WithNullParent_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new LayeredPropertyBag(null!));
    }

    [Fact]
    public void Parent_ReturnsTheParent()
    {
        var parent = new PropertyBagDictionary();

        var child = new LayeredPropertyBag(parent);

        Assert.Same(parent, child.Parent);
    }

    #endregion

    #region TryGetValue Tests

    [Fact]
    public void TryGetValue_WithParentOnlyValue_ReadsThrough()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);

        Assert.True(child.TryGetValue(key, out var value));
        Assert.Equal("Parent", value);
    }

    [Fact]
    public void TryGetValue_WithNoValueAnywhere_ReturnsFalse()
    {
        var child = new LayeredPropertyBag(new PropertyBagDictionary());
        var key = new PropertyBagKey<string>("TestKey");

        Assert.False(child.TryGetValue(key, out var value));
        Assert.Null(value);
    }

    [Fact]
    public void TryGetValue_WithLaterParentChange_SeesTheChange()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        var child = new LayeredPropertyBag(parent);
        Assert.False(child.TryGetValue(key, out _));

        parent.SetValue(key, "Later");

        Assert.True(child.TryGetValue(key, out var value));
        Assert.Equal("Later", value);
    }

    [Fact]
    public void TryGetValue_WhenLocalValueIsNotOfRequestedType_ReturnsFalseWithoutReadingParent()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<WrongTypeClone>("TestKey");
        parent.SetValue(key, new WrongTypeClone());
        var child = new LayeredPropertyBag(parent);
        child.SetValue(key, new WrongTypeClone());
        var clone = child.Clone();
        var parentFound = parent.TryGetValue(key, out _);
        Assert.True(parentFound);

        var found = clone.TryGetValue(key, out var value);

        Assert.False(found);
        Assert.Null(value);
    }

    #endregion

    #region SetValue Tests

    [Fact]
    public void SetValue_HidesParentValueAndLeavesParentUnchanged()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);

        var result = child.SetValue(key, "Child");

        Assert.Same(child, result);
        Assert.True(child.TryGetValue(key, out var childValue));
        Assert.Equal("Child", childValue);
        Assert.True(parent.TryGetValue(key, out var parentValue));
        Assert.Equal("Parent", parentValue);
    }

    [Fact]
    public void SetValue_WithNull_StoresNullAndHidesParent()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string?>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);

        child.SetValue(key, null);

        Assert.True(child.TryGetValue(key, out var value));
        Assert.Null(value);
    }

    #endregion

    #region Remove Tests

    [Fact]
    public void Remove_HidesParentValueWhileParentStillHoldsIt()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);

        var result = child.Remove(key);

        Assert.Same(child, result);
        Assert.False(child.TryGetValue(key, out var value));
        Assert.Null(value);
        Assert.True(parent.TryGetValue(key, out var parentValue));
        Assert.Equal("Parent", parentValue);
    }

    [Fact]
    public void SetValue_AfterRemove_ShowsTheValueAgain()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);
        child.Remove(key);

        child.SetValue(key, "Child");

        Assert.True(child.TryGetValue(key, out var value));
        Assert.Equal("Child", value);
    }

    #endregion

    #region Scope Tests

    [Fact]
    public void Scope_OverParentOnlyKey_RestoresFallThrough()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);

        using (child.Scope(key, "Scoped"))
        {
            Assert.True(child.TryGetValue(key, out var scoped));
            Assert.Equal("Scoped", scoped);
        }

        Assert.True(child.TryGetValue(key, out var value));
        Assert.Equal("Parent", value);

        parent.SetValue(key, "Changed");
        Assert.True(child.TryGetValue(key, out var changed));
        Assert.Equal("Changed", changed);
    }

    [Fact]
    public void Scope_OverRemovedKey_RestoresTheRemoval()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);
        child.Remove(key);

        using (child.Scope(key, "Scoped"))
        {
            Assert.True(child.TryGetValue(key, out var scoped));
            Assert.Equal("Scoped", scoped);
        }

        Assert.False(child.TryGetValue(key, out _));
    }

    [Fact]
    public void Scope_OverLocalValue_RestoresTheLocalValue()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        var child = new LayeredPropertyBag(parent);
        child.SetValue(key, "Local");

        using (child.Scope(key, "Scoped")) { }

        Assert.True(child.TryGetValue(key, out var value));
        Assert.Equal("Local", value);
    }

    #endregion

    #region Clone Tests

    [Fact]
    public void Clone_SharesParentAndCopiesLocalEntriesIndependently()
    {
        var parent = new PropertyBagDictionary();
        var parentKey = new PropertyBagKey<string>("ParentKey");
        var localKey = new PropertyBagKey<string>("LocalKey");
        parent.SetValue(parentKey, "Parent");
        var child = new LayeredPropertyBag(parent);
        child.SetValue(localKey, "Local");

        var clone = Assert.IsType<LayeredPropertyBag>(child.Clone());

        Assert.NotSame(child, clone);
        Assert.Same(parent, clone.Parent);
        Assert.True(clone.TryGetValue(localKey, out var local));
        Assert.Equal("Local", local);
        Assert.True(clone.TryGetValue(parentKey, out var fromParent));
        Assert.Equal("Parent", fromParent);

        clone.SetValue(localKey, "Changed");
        Assert.True(child.TryGetValue(localKey, out var original));
        Assert.Equal("Local", original);
    }

    [Fact]
    public void Clone_CopiesTombstone()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);
        child.Remove(key);

        var clone = child.Clone();

        Assert.False(clone.TryGetValue(key, out _));

        clone.SetValue(key, "Clone");
        Assert.False(child.TryGetValue(key, out _));
    }

    [Fact]
    public void Clone_WithNeverWrittenChild_ReturnsEmptyChildOverSameParent()
    {
        var parent = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        parent.SetValue(key, "Parent");
        var child = new LayeredPropertyBag(parent);

        var clone = Assert.IsType<LayeredPropertyBag>(child.Clone());

        Assert.NotSame(child, clone);
        Assert.Same(parent, clone.Parent);
        Assert.True(clone.TryGetValue(key, out var value));
        Assert.Equal("Parent", value);
    }

    [Fact]
    public void Clone_WithCloneableLocalValue_ClonesTheValue()
    {
        var child = new LayeredPropertyBag(new PropertyBagDictionary());
        var key = new PropertyBagKey<Cell>("TestKey");
        var cell = new Cell(7);
        child.SetValue(key, cell);

        var clone = child.Clone();

        Assert.True(clone.TryGetValue(key, out var cloned));
        Assert.NotSame(cell, cloned);
        Assert.Equal(7, cloned?.Number);
    }

    [Fact]
    public void ICloneableClone_ReturnsLayeredCopy()
    {
        var parent = new PropertyBagDictionary();
        var child = new LayeredPropertyBag(parent);
        var key = new PropertyBagKey<string>("TestKey");
        child.SetValue(key, "Local");

        var clone = Assert.IsType<LayeredPropertyBag>(((ICloneable)child).Clone());

        Assert.NotSame(child, clone);
        Assert.Same(parent, clone.Parent);
        Assert.True(clone.TryGetValue(key, out var value));
        Assert.Equal("Local", value);
    }

    #endregion
}
