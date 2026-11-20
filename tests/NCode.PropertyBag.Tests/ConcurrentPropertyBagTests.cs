// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

public class ConcurrentPropertyBagTests
{
    #region SetValue Tests

    [Fact]
    public void SetValue_WithStringValue_StoresValue()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        propertyBag.SetValue(key, "TestValue");

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void SetValue_WithNullValue_StoresNull()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string?>("TestKey");

        propertyBag.SetValue(key, null);

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Null(value);
    }

    [Fact]
    public void SetValue_OverwritesExistingValue()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "OriginalValue");

        propertyBag.SetValue(key, "NewValue");

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Equal("NewValue", value);
    }

    [Fact]
    public void SetValue_ReturnsSameInstance_ForMethodChaining()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var result = propertyBag.SetValue(key, "TestValue");

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void SetValue_FromManyThreads_StoresEveryValue()
    {
        var propertyBag = new ConcurrentPropertyBag();

        Parallel.For(0, 1000, i => propertyBag.SetValue(new PropertyBagKey<int>($"k{i}"), i));

        for (var i = 0; i < 1000; i++)
        {
            Assert.True(propertyBag.TryGetValue(new PropertyBagKey<int>($"k{i}"), out var value));
            Assert.Equal(i, value);
        }
    }

    #endregion

    #region TryGetValue Tests

    [Fact]
    public void TryGetValue_WhenKeyDoesNotExist_ReturnsFalseAndDefault()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("NonExistentKey");

        var result = propertyBag.TryGetValue(key, out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void TryGetValue_WithSameNameButDifferentType_ReturnsFalse()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var stringKey = new PropertyBagKey<string>("Key");
        var intKey = new PropertyBagKey<int>("Key");
        propertyBag.SetValue(stringKey, "StringValue");

        var result = propertyBag.TryGetValue(intKey, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryGetValue_WhenStoredValueIsNotOfRequestedType_ReturnsFalse()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<WrongTypeClone>("TestKey");
        propertyBag.SetValue(key, new WrongTypeClone());

        // Clone deep-copies an ICloneable value, and this one's Clone returns another type.
        var clone = propertyBag.Clone();

        Assert.False(clone.TryGetValue(key, out var value));
        Assert.Null(value);
    }

    #endregion

    #region Remove Tests

    [Fact]
    public void Remove_WhenKeyExists_RemovesValue()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        propertyBag.Remove(key);

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void Remove_WhenKeyDoesNotExist_DoesNotThrow()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("NonExistentKey");

        var exception = Record.Exception(() => propertyBag.Remove(key));

        Assert.Null(exception);
    }

    [Fact]
    public void Remove_ReturnsSameInstance_ForMethodChaining()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var result = propertyBag.Remove(key);

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void Remove_OnlyRemovesSpecifiedKey()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<string>("Key2");
        propertyBag.SetValue(key1, "Value1");
        propertyBag.SetValue(key2, "Value2");

        propertyBag.Remove(key1);

        Assert.False(propertyBag.TryGetValue(key1, out _));
        Assert.True(propertyBag.TryGetValue(key2, out var value2));
        Assert.Equal("Value2", value2);
    }

    #endregion

    #region Scope Tests

    [Fact]
    public void Scope_SetsValue()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        using (propertyBag.Scope(key, "TestValue"))
        {
            Assert.True(propertyBag.TryGetValue(key, out var value));
            Assert.Equal("TestValue", value);
        }
    }

    [Fact]
    public void Scope_WhenKeyHadNoValue_RemovesValueOnDispose()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        using (propertyBag.Scope(key, "TestValue"))
        {
            Assert.True(propertyBag.TryGetValue(key, out _));
        }

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void Scope_WhenKeyHadValue_RestoresPreviousValueOnDispose()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "Outer");

        using (propertyBag.Scope(key, "Inner"))
        {
            Assert.True(propertyBag.TryGetValue(key, out var inner));
            Assert.Equal("Inner", inner);
        }

        Assert.True(propertyBag.TryGetValue(key, out var restored));
        Assert.Equal("Outer", restored);
    }

    [Fact]
    public void Scope_WhenAnotherWriteHappensInside_RestoresCapturedValue()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "Outer");

        using (propertyBag.Scope(key, "Inner"))
        {
            propertyBag.SetValue(key, "Other");
        }

        Assert.True(propertyBag.TryGetValue(key, out var restored));
        Assert.Equal("Outer", restored);
    }

    [Fact]
    public void Scope_DisposedTwice_RestoresOnce()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "Outer");
        var scope = propertyBag.Scope(key, "Inner");

        scope.Dispose();
        propertyBag.SetValue(key, "Later");
        scope.Dispose();

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Equal("Later", value);
    }

    #endregion

    #region Clone Tests

    [Fact]
    public void Clone_ReturnsNewConcurrentPropertyBag()
    {
        var propertyBag = new ConcurrentPropertyBag();

        var clone = propertyBag.Clone();

        Assert.NotSame(propertyBag, clone);
        Assert.IsType<ConcurrentPropertyBag>(clone);
    }

    [Fact]
    public void Clone_CopiesAllValues()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<int>("Key2");
        propertyBag.SetValue(key1, "Value1");
        propertyBag.SetValue(key2, 42);

        var clone = propertyBag.Clone();

        Assert.True(clone.TryGetValue(key1, out var value1));
        Assert.Equal("Value1", value1);
        Assert.True(clone.TryGetValue(key2, out var value2));
        Assert.Equal(42, value2);
    }

    [Fact]
    public void Clone_IsIndependentFromOriginal()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "OriginalValue");

        var clone = propertyBag.Clone();
        clone.SetValue(key, "ClonedValue");

        Assert.True(propertyBag.TryGetValue(key, out var originalValue));
        Assert.Equal("OriginalValue", originalValue);
        Assert.True(clone.TryGetValue(key, out var clonedValue));
        Assert.Equal("ClonedValue", clonedValue);
    }

    [Fact]
    public void Clone_DeepClonesCloneableValues()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<CloneableValue>("TestKey");
        var originalValue = new CloneableValue { Data = "Original" };
        propertyBag.SetValue(key, originalValue);

        var clone = propertyBag.Clone();

        Assert.True(clone.TryGetValue(key, out var clonedValue));
        Assert.NotSame(originalValue, clonedValue);
        Assert.Equal("Original", clonedValue?.Data);
    }

    [Fact]
    public void Clone_ShallowCopiesNonCloneableValues()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<NonCloneableValue>("TestKey");
        var originalValue = new NonCloneableValue { Data = "Original" };
        propertyBag.SetValue(key, originalValue);

        var clone = propertyBag.Clone();

        Assert.True(clone.TryGetValue(key, out var clonedValue));
        Assert.Same(originalValue, clonedValue);
    }

    [Fact]
    public void ICloneable_Clone_ReturnsClone()
    {
        var propertyBag = new ConcurrentPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");
        ICloneable cloneable = propertyBag;

        var clone = cloneable.Clone();

        var concurrentClone = Assert.IsType<ConcurrentPropertyBag>(clone);
        Assert.NotSame(propertyBag, concurrentClone);
        Assert.True(concurrentClone.TryGetValue(key, out var value));
        Assert.Equal("TestValue", value);
    }

    #endregion

    #region Helper Classes

    private sealed class WrongTypeClone : ICloneable
    {
        public object Clone() => "not a WrongTypeClone";
    }

    private sealed class CloneableValue : ICloneable
    {
        public string? Data { get; set; }

        public object Clone() => new CloneableValue { Data = Data };
    }

    private sealed class NonCloneableValue
    {
        public string? Data { get; set; }
    }

    #endregion
}
