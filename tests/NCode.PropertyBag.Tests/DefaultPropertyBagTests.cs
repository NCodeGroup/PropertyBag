// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

using System.Collections;

public class DefaultPropertyBagTests
{
    #region SetValue Tests

    [Fact]
    public void SetValue_WithStringValue_StoresValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        propertyBag.SetValue(key, "TestValue");

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void SetValue_WithIntValue_StoresValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<int>("TestKey");

        propertyBag.SetValue(key, 42);

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Equal(42, value);
    }

    [Fact]
    public void SetValue_WithNullValue_StoresNull()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string?>("TestKey");

        propertyBag.SetValue(key, null);

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Null(value);
    }

    [Fact]
    public void SetValue_OverwritesExistingValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "OriginalValue");

        propertyBag.SetValue(key, "NewValue");

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Equal("NewValue", value);
    }

    [Fact]
    public void SetValue_ReturnsSameInstance_ForMethodChaining()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var result = propertyBag.SetValue(key, "TestValue");

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void SetValue_MethodChaining_SetsMultipleValues()
    {
        var propertyBag = new DefaultPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<int>("Key2");
        var key3 = new PropertyBagKey<bool>("Key3");

        propertyBag.SetValue(key1, "Value1").SetValue(key2, 42).SetValue(key3, true);

        Assert.True(propertyBag.TryGetValue(key1, out var value1));
        Assert.Equal("Value1", value1);
        Assert.True(propertyBag.TryGetValue(key2, out var value2));
        Assert.Equal(42, value2);
        Assert.True(propertyBag.TryGetValue(key3, out var value3));
        Assert.True(value3);
    }

    #endregion

    #region TryGetValue Tests

    [Fact]
    public void TryGetValue_WhenKeyExists_ReturnsTrueAndValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        var result = propertyBag.TryGetValue(key, out var value);

        Assert.True(result);
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void TryGetValue_WhenKeyDoesNotExist_ReturnsFalseAndDefault()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("NonExistentKey");

        var result = propertyBag.TryGetValue(key, out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void TryGetValue_WhenKeyDoesNotExist_ReturnsDefaultForValueType()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<int>("NonExistentKey");

        var result = propertyBag.TryGetValue(key, out var value);

        Assert.False(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryGetValue_WithSameNameButDifferentType_ReturnsFalse()
    {
        var propertyBag = new DefaultPropertyBag();
        var stringKey = new PropertyBagKey<string>("Key");
        var intKey = new PropertyBagKey<int>("Key");
        propertyBag.SetValue(stringKey, "StringValue");

        var result = propertyBag.TryGetValue(intKey, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryGetValue_WhenEmptyPropertyBag_ReturnsFalse()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var result = propertyBag.TryGetValue(key, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryGetValue_WhenStoredValueIsNotOfRequestedType_ReturnsFalse()
    {
        var propertyBag = new DefaultPropertyBag();
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
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        propertyBag.Remove(key);

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void Remove_WhenKeyDoesNotExist_DoesNotThrow()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("NonExistentKey");

        var exception = Record.Exception(() => propertyBag.Remove(key));

        Assert.Null(exception);
    }

    [Fact]
    public void Remove_ReturnsSameInstance_ForMethodChaining()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var result = propertyBag.Remove(key);

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void Remove_OnlyRemovesSpecifiedKey()
    {
        var propertyBag = new DefaultPropertyBag();
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

    #region Clone Tests

    [Fact]
    public void Clone_ReturnsNewInstance()
    {
        var propertyBag = new DefaultPropertyBag();

        var clone = propertyBag.Clone();

        Assert.NotSame(propertyBag, clone);
    }

    [Fact]
    public void Clone_ReturnsIPropertyBag()
    {
        var propertyBag = new DefaultPropertyBag();

        var clone = propertyBag.Clone();

        Assert.IsAssignableFrom<IPropertyBag>(clone);
    }

    [Fact]
    public void Clone_CopiesAllValues()
    {
        var propertyBag = new DefaultPropertyBag();
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
        var propertyBag = new DefaultPropertyBag();
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
    public void Clone_WhenEmpty_ReturnsEmptyPropertyBag()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var clone = propertyBag.Clone();

        Assert.False(clone.TryGetValue(key, out _));
    }

    [Fact]
    public void Clone_DeepClonesCloneableValues()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<CloneableValue>("TestKey");
        var originalValue = new CloneableValue { Data = "Original" };
        propertyBag.SetValue(key, originalValue);

        var clone = propertyBag.Clone();

        Assert.True(clone.TryGetValue(key, out var clonedValue));
        Assert.NotSame(originalValue, clonedValue);
        Assert.Equal("Original", clonedValue!.Data);
    }

    [Fact]
    public void Clone_ShallowCopiesNonCloneableValues()
    {
        var propertyBag = new DefaultPropertyBag();
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
        ICloneable propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        ((DefaultPropertyBag)propertyBag).SetValue(key, "TestValue");

        var clone = propertyBag.Clone();

        Assert.IsType<DefaultPropertyBag>(clone);
        Assert.True(((DefaultPropertyBag)clone).TryGetValue(key, out var value));
        Assert.Equal("TestValue", value);
    }

    #endregion

    #region Scope Tests

    [Fact]
    public void Scope_ReturnsIPropertyBagScope()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var scope = propertyBag.Scope(key, "TestValue");

        Assert.IsAssignableFrom<IPropertyBagScope>(scope);
    }

    [Fact]
    public void Scope_SetsValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        using (propertyBag.Scope(key, "TestValue"))
        {
            Assert.True(propertyBag.TryGetValue(key, out var value));
            Assert.Equal("TestValue", value);
        }
    }

    [Fact]
    public void Scope_RemovesValueOnDispose()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        using (propertyBag.Scope(key, "TestValue"))
        {
            Assert.True(propertyBag.TryGetValue(key, out _));
        }

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void Scope_WithNullValue_SetsNull()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string?>("TestKey");

        using (propertyBag.Scope(key, null))
        {
            Assert.True(propertyBag.TryGetValue(key, out var value));
            Assert.Null(value);
        }

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;
        Assert.False(dict.ContainsKey(key));
    }

    [Fact]
    public void Scope_WhenKeyHadValue_RestoresPreviousValueOnDispose()
    {
        var propertyBag = new DefaultPropertyBag();
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

    #endregion

    #region IReadOnlyDictionary Tests

    [Fact]
    public void Count_WhenEmpty_ReturnsZero()
    {
        IReadOnlyDictionary<PropertyBagKey, object?> propertyBag = new DefaultPropertyBag();

        Assert.Empty(propertyBag);
    }

    [Fact]
    public void Count_AfterAddingItems_ReturnsCorrectCount()
    {
        var propertyBag = new DefaultPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<int>("Key2");
        propertyBag.SetValue(key1, "Value1");
        propertyBag.SetValue(key2, 42);

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;

        Assert.Equal(2, dict.Count);
    }

    [Fact]
    public void Keys_WhenEmpty_ReturnsEmptyCollection()
    {
        IReadOnlyDictionary<PropertyBagKey, object?> propertyBag = new DefaultPropertyBag();

        Assert.Empty(propertyBag.Keys);
    }

    [Fact]
    public void Keys_ReturnsAllKeys()
    {
        var propertyBag = new DefaultPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<int>("Key2");
        propertyBag.SetValue(key1, "Value1");
        propertyBag.SetValue(key2, 42);

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;

        Assert.Equal(2, dict.Keys.Count());
        Assert.Contains((PropertyBagKey)key1, dict.Keys);
        Assert.Contains((PropertyBagKey)key2, dict.Keys);
    }

    [Fact]
    public void Values_WhenEmpty_ReturnsEmptyCollection()
    {
        IReadOnlyDictionary<PropertyBagKey, object?> propertyBag = new DefaultPropertyBag();

        Assert.Empty(propertyBag.Values);
    }

    [Fact]
    public void Values_ReturnsAllValues()
    {
        var propertyBag = new DefaultPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<int>("Key2");
        propertyBag.SetValue(key1, "Value1");
        propertyBag.SetValue(key2, 42);

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;

        Assert.Equal(2, dict.Values.Count());
        Assert.Contains("Value1", dict.Values);
        Assert.Contains(42, dict.Values);
    }

    [Fact]
    public void Indexer_WhenKeyExists_ReturnsValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;

        Assert.Equal("TestValue", dict[key]);
    }

    [Fact]
    public void Indexer_WhenKeyDoesNotExist_ReturnsNull()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("NonExistentKey");

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;

        Assert.Null(dict[key]);
    }

    [Fact]
    public void ContainsKey_WhenKeyExists_ReturnsTrue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;

        Assert.True(dict.ContainsKey(key));
    }

    [Fact]
    public void ContainsKey_WhenKeyDoesNotExist_ReturnsFalse()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("NonExistentKey");

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;

        Assert.False(dict.ContainsKey(key));
    }

    [Fact]
    public void ContainsKey_WhenEmpty_ReturnsFalse()
    {
        IReadOnlyDictionary<PropertyBagKey, object?> propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        Assert.False(propertyBag.ContainsKey(key));
    }

    [Fact]
    public void DictionaryTryGetValue_WhenKeyExists_ReturnsTrueAndValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;
        var result = dict.TryGetValue(key, out var value);

        Assert.True(result);
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void DictionaryTryGetValue_WhenKeyDoesNotExist_ReturnsFalse()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("NonExistentKey");

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;
        var result = dict.TryGetValue(key, out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void GetEnumerator_WhenEmpty_ReturnsEmptyEnumerator()
    {
        IReadOnlyDictionary<PropertyBagKey, object?> propertyBag = new DefaultPropertyBag();

        var items = propertyBag.ToList();

        Assert.Empty(items);
    }

    [Fact]
    public void GetEnumerator_ReturnsAllItems()
    {
        var propertyBag = new DefaultPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<int>("Key2");
        propertyBag.SetValue(key1, "Value1");
        propertyBag.SetValue(key2, 42);

        IReadOnlyDictionary<PropertyBagKey, object?> dict = propertyBag;
        var items = dict.ToList();

        Assert.Equal(2, items.Count);
    }

    [Fact]
    public void NonGenericGetEnumerator_ReturnsEnumerator()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        IEnumerable enumerable = propertyBag;
        var enumerator = enumerable.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.NotNull(enumerator.Current);
    }

    [Fact]
    public void Count_AfterAddingItems_CountsThroughIReadOnlyCollection()
    {
        var propertyBag = new DefaultPropertyBag();
        propertyBag.SetValue(new PropertyBagKey<string>("Key1"), "Value1");
        propertyBag.SetValue(new PropertyBagKey<int>("Key2"), 42);

        IReadOnlyCollection<KeyValuePair<PropertyBagKey, object?>> collection = propertyBag;

        var count = collection.Count;

        Assert.Equal(2, count);
    }

    [Fact]
    public void Count_WhenEmpty_IsZeroThroughIReadOnlyCollection()
    {
        IReadOnlyCollection<KeyValuePair<PropertyBagKey, object?>> collection =
            new DefaultPropertyBag();

        // Reads the Count member itself: Assert.Empty would enumerate instead of calling it.
        var count = collection.Count;

        Assert.Equal(0, count);
    }

    [Fact]
    public void GetEnumerator_WhenNotEmpty_EnumeratesEveryEntry()
    {
        var propertyBag = new DefaultPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<int>("Key2");
        propertyBag.SetValue(key1, "Value1").SetValue(key2, 42);
        IEnumerable<KeyValuePair<PropertyBagKey, object?>> typed = propertyBag;

        var entries = typed.ToArray();

        Assert.Equal(2, entries.Length);
        Assert.Contains(entries, pair => pair.Key == key1 && (string?)pair.Value == "Value1");
        Assert.Contains(entries, pair => pair.Key == key2 && (int?)pair.Value == 42);
    }

    [Fact]
    public void GetEnumerator_WhenEmpty_EnumeratesNothing()
    {
        IEnumerable<KeyValuePair<PropertyBagKey, object?>> typed = new DefaultPropertyBag();

        using var enumerator = typed.GetEnumerator();
        var hasEntry = enumerator.MoveNext();

        Assert.False(hasEntry);
    }

    [Fact]
    public void NonGenericGetEnumerator_WhenEmpty_EnumeratesNothing()
    {
        IEnumerable untyped = new DefaultPropertyBag();

        var enumerator = untyped.GetEnumerator();
        var hasEntry = enumerator.MoveNext();

        Assert.False(hasEntry);
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
