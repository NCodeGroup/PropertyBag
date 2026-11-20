// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Concurrent;

namespace NCode.PropertyBag.Tests;

public class ConcurrentPropertyBagDictionaryTests
{
    #region SetValue Tests

    [Fact]
    public void SetValue_WithStringValue_StoresValue()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");

        propertyBag.SetValue(key, "TestValue");

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void SetValue_WithNullValue_StoresNull()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string?>("TestKey");

        propertyBag.SetValue(key, null);

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Null(value);
    }

    [Fact]
    public void SetValue_OverwritesExistingValue()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "OriginalValue");

        propertyBag.SetValue(key, "NewValue");

        Assert.True(propertyBag.TryGetValue(key, out var value));
        Assert.Equal("NewValue", value);
    }

    [Fact]
    public void SetValue_ReturnsSameInstance_ForMethodChaining()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");

        var result = propertyBag.SetValue(key, "TestValue");

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void SetValue_FromManyThreads_StoresEveryValue()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();

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
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("NonExistentKey");

        var result = propertyBag.TryGetValue(key, out var value);

        Assert.False(result);
        Assert.Null(value);
    }

    [Fact]
    public void TryGetValue_WithSameNameButDifferentType_ReturnsFalse()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var stringKey = new PropertyBagKey<string>("Key");
        var intKey = new PropertyBagKey<int>("Key");
        propertyBag.SetValue(stringKey, "StringValue");

        var result = propertyBag.TryGetValue(intKey, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryGetValue_WhenStoredValueIsNotOfRequestedType_ReturnsFalse()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");

        propertyBag.Remove(key);

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void Remove_WhenKeyDoesNotExist_DoesNotThrow()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("NonExistentKey");

        var exception = Record.Exception(() => propertyBag.Remove(key));

        Assert.Null(exception);
    }

    [Fact]
    public void Remove_ReturnsSameInstance_ForMethodChaining()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");

        var result = propertyBag.Remove(key);

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void Remove_OnlyRemovesSpecifiedKey()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
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

    #region TryAdd Tests

    [Fact]
    public void TryAdd_WhenKeyDoesNotExist_StoresValueAndReturnsTrue()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");

        Assert.True(bag.TryAdd(key, "First"));

        Assert.True(bag.TryGetValue(key, out var value));
        Assert.Equal("First", value);
    }

    [Fact]
    public void TryAdd_WhenKeyExists_KeepsValueAndReturnsFalse()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string?>("TestKey");
        bag.SetValue(key, null);

        Assert.False(bag.TryAdd(key, "Second"));

        Assert.True(bag.TryGetValue(key, out var value));
        Assert.Null(value);
    }

    #endregion

    #region TryRemove Tests

    [Fact]
    public void TryRemove_WhenKeyExists_RemovesAndReturnsValue()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        bag.SetValue(key, "Value");

        Assert.True(bag.TryRemove(key, out var value));

        Assert.Equal("Value", value);
        Assert.False(bag.TryGetValue(key, out _));
    }

    [Fact]
    public void TryRemove_WhenKeyDoesNotExist_ReturnsFalseAndDefault()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");

        Assert.False(bag.TryRemove(key, out var value));

        Assert.Null(value);
    }

    #endregion

    #region GetOrAdd Tests

    [Fact]
    public void GetOrAdd_WithNullFactory_Throws()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");

        Assert.Throws<ArgumentNullException>(() => bag.GetOrAdd(key, null!));
    }

    [Fact]
    public void GetOrAdd_WhenKeyExists_ReturnsValueWithoutCallingFactory()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        bag.SetValue(key, "Existing");

        var result = bag.GetOrAdd(key, _ =>
        {
            Assert.Fail("The factory must not run.");
            return "Created";
        });

        Assert.Equal("Existing", result);
    }

    [Fact]
    public void GetOrAdd_WhenKeyDoesNotExist_StoresAndReturnsCreatedValue()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");

        var result = bag.GetOrAdd(key, k => k.Name + "!");

        Assert.Equal("TestKey!", result);
        Assert.True(bag.TryGetValue(key, out var value));
        Assert.Equal("TestKey!", value);
    }

    [Fact]
    public void GetOrAdd_WhenAnotherThreadStoresDuringFactory_ReturnsStoredValue()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<object>("TestKey");
        var other = new object();

        var result = bag.GetOrAdd(key, k =>
        {
            bag.SetValue(k, other);
            return new object();
        });

        Assert.Same(other, result);
        Assert.True(bag.TryGetValue(key, out var value));
        Assert.Same(other, value);
    }

    [Fact]
    public void GetOrAdd_FromManyThreads_ReturnsOneValue()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<object>("TestKey");
        var results = new ConcurrentBag<object>();

        Parallel.For(0, 100, _ => results.Add(bag.GetOrAdd(key, _ => new object())!));

        Assert.Single(results.Distinct(ReferenceEqualityComparer.Instance));
    }

    #endregion

    #region Scope Tests

    [Fact]
    public void Scope_SetsValue()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
    public void Clone_ReturnsNewConcurrentPropertyBagDictionary()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();

        var clone = propertyBag.Clone();

        Assert.NotSame(propertyBag, clone);
        Assert.IsType<ConcurrentPropertyBagDictionary>(clone);
    }

    [Fact]
    public void Clone_CopiesAllValues()
    {
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
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
        var propertyBag = new ConcurrentPropertyBagDictionary();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.SetValue(key, "TestValue");
        ICloneable cloneable = propertyBag;

        var clone = cloneable.Clone();

        var concurrentClone = Assert.IsType<ConcurrentPropertyBagDictionary>(clone);
        Assert.NotSame(propertyBag, concurrentClone);
        Assert.True(concurrentClone.TryGetValue(key, out var value));
        Assert.Equal("TestValue", value);
    }

    #endregion

    #region IReadOnlyDictionary Tests

    private static ConcurrentPropertyBagDictionary CreateBag(
        out PropertyBagKey<string> key1,
        out PropertyBagKey<int> key2
    )
    {
        key1 = new PropertyBagKey<string>("Key1");
        key2 = new PropertyBagKey<int>("Key2");
        var bag = new ConcurrentPropertyBagDictionary();
        bag.SetValue(key1, "Value1");
        bag.SetValue(key2, 42);
        return bag;
    }

    [Fact]
    public void Count_ReturnsNumberOfEntries()
    {
        var bag = CreateBag(out _, out _);
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        Assert.Equal(2, dict.Count);
    }

    [Fact]
    public void Keys_ReturnsAllKeys()
    {
        var bag = CreateBag(out var key1, out var key2);
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        Assert.Equivalent(new PropertyBagKey[] { key1, key2 }, dict.Keys.ToArray());
    }

    [Fact]
    public void Values_ReturnsAllValues()
    {
        var bag = CreateBag(out _, out _);
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        Assert.Equivalent(new object?[] { "Value1", 42 }, dict.Values.ToArray());
    }

    [Fact]
    public void Indexer_WithPresentKey_ReturnsValue()
    {
        var bag = CreateBag(out var key1, out _);
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        Assert.Equal("Value1", dict[key1]);
    }

    [Fact]
    public void Indexer_WithMissingKey_ReturnsNull()
    {
        var bag = CreateBag(out _, out _);
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        Assert.Null(dict[new PropertyBagKey<string>("Missing")]);
    }

    [Fact]
    public void ContainsKey_ReportsPresenceOfKey()
    {
        var bag = CreateBag(out var key1, out _);
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        Assert.True(dict.ContainsKey(key1));
        Assert.False(dict.ContainsKey(new PropertyBagKey<string>("Missing")));
    }

    [Fact]
    public void DictionaryTryGetValue_ReportsPresenceOfKey()
    {
        var bag = CreateBag(out var key1, out _);
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        Assert.True(dict.TryGetValue(key1, out var value));
        Assert.Equal("Value1", value);
        Assert.False(dict.TryGetValue(new PropertyBagKey<string>("Missing"), out var missing));
        Assert.Null(missing);
    }

    [Fact]
    public void GenericEnumerator_EnumeratesAllEntries()
    {
        var bag = CreateBag(out var key1, out var key2);
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        var pairs = dict.ToDictionary(pair => pair.Key, pair => pair.Value);

        Assert.Equal(2, pairs.Count);
        Assert.Equal("Value1", pairs[key1]);
        Assert.Equal(42, pairs[key2]);
    }

    [Fact]
    public void NonGenericEnumerator_EnumeratesAllEntries()
    {
        var bag = CreateBag(out _, out _);
        System.Collections.IEnumerable enumerable = bag;

        var count = 0;
        foreach (var _ in enumerable)
        {
            count++;
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task Enumerate_WhileAnotherThreadSets_DoesNotThrow()
    {
        var bag = new ConcurrentPropertyBagDictionary();
        IReadOnlyDictionary<PropertyBagKey, object?> dict = bag;

        var writer = Task.Run(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                bag.SetValue(new PropertyBagKey<int>($"Key{i}"), i);
            }
        });

        while (!writer.IsCompleted)
        {
            foreach (var pair in dict)
            {
                Assert.IsType<int>(pair.Value);
            }
        }

        await writer;

        Assert.Equal(1000, dict.Count);
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
