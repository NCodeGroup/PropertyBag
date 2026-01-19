namespace NCode.PropertyBag.Tests;

public class DefaultPropertyBagScopeTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidParameters_SetsProperties()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var scope = new DefaultPropertyBagScope(propertyBag, key);

        Assert.Same(propertyBag, scope.PropertyBag);
        Assert.Equal(key, scope.Key);
    }

    [Fact]
    public void Constructor_WithDifferentKeyTypes_SetsCorrectKey()
    {
        var propertyBag = new DefaultPropertyBag();
        var intKey = new PropertyBagKey<int>("IntKey");

        var scope = new DefaultPropertyBagScope(propertyBag, intKey);

        Assert.Equal(typeof(int), scope.Key.Type);
        Assert.Equal("IntKey", scope.Key.Name);
    }

    #endregion

    #region PropertyBag Property Tests

    [Fact]
    public void PropertyBag_ReturnsConstructorParameter()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var scope = new DefaultPropertyBagScope(propertyBag, key);

        Assert.Same(propertyBag, scope.PropertyBag);
    }

    #endregion

    #region Key Property Tests

    [Fact]
    public void Key_ReturnsConstructorParameter()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");

        var scope = new DefaultPropertyBagScope(propertyBag, key);

        Assert.Equal(key, scope.Key);
    }

    [Fact]
    public void Key_PreservesTypeAndName()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<DateTime>("DateKey");

        var scope = new DefaultPropertyBagScope(propertyBag, key);

        Assert.Equal(typeof(DateTime), scope.Key.Type);
        Assert.Equal("DateKey", scope.Key.Name);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_RemovesValueFromPropertyBag()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.Set(key, "TestValue");

        var scope = new DefaultPropertyBagScope(propertyBag, key);

        Assert.True(propertyBag.TryGetValue(key, out _));

        scope.Dispose();

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void Dispose_WhenKeyDoesNotExist_DoesNotThrow()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("NonExistentKey");

        var scope = new DefaultPropertyBagScope(propertyBag, key);

        var exception = Record.Exception(() => scope.Dispose());

        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.Set(key, "TestValue");

        var scope = new DefaultPropertyBagScope(propertyBag, key);

        var exception = Record.Exception(() =>
        {
            scope.Dispose();
            scope.Dispose();
            scope.Dispose();
        });

        Assert.Null(exception);
    }

    [Fact]
    public void Dispose_OnlyRemovesSpecifiedKey()
    {
        var propertyBag = new DefaultPropertyBag();
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<string>("Key2");
        propertyBag.Set(key1, "Value1");
        propertyBag.Set(key2, "Value2");

        var scope = new DefaultPropertyBagScope(propertyBag, key1);

        scope.Dispose();

        Assert.False(propertyBag.TryGetValue(key1, out _));
        Assert.True(propertyBag.TryGetValue(key2, out var value2));
        Assert.Equal("Value2", value2);
    }

    #endregion

    #region Using Statement Tests

    [Fact]
    public void UsingStatement_RemovesValueAfterBlock()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.Set(key, "TestValue");

        using (new DefaultPropertyBagScope(propertyBag, key))
        {
            Assert.True(propertyBag.TryGetValue(key, out var value));
            Assert.Equal("TestValue", value);
        }

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void UsingStatement_WithScopeMethod_TemporarilyOverridesValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("Culture");
        propertyBag.Set(key, "en-US");

        using (propertyBag.Scope(key, "fr-FR"))
        {
            Assert.True(propertyBag.TryGetValue(key, out var value));
            Assert.Equal("fr-FR", value);
        }

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void UsingStatement_WhenExceptionThrown_StillRemovesValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("TestKey");
        propertyBag.Set(key, "TestValue");

        try
        {
            using (new DefaultPropertyBagScope(propertyBag, key))
            {
                throw new InvalidOperationException("Test exception");
            }
        }
        catch (InvalidOperationException)
        {
            // Expected
        }

        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void NestedScopes_RemoveInCorrectOrder()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<int>("Counter");
        propertyBag.Set(key, 1);

        using (propertyBag.Scope(key, 2))
        {
            Assert.True(propertyBag.TryGetValue(key, out var value1));
            Assert.Equal(2, value1);

            using (propertyBag.Scope(key, 3))
            {
                Assert.True(propertyBag.TryGetValue(key, out var value2));
                Assert.Equal(3, value2);
            }

            // After inner scope disposes, value is removed (not restored to 2)
            Assert.False(propertyBag.TryGetValue(key, out _));
        }

        // After outer scope disposes, still no value
        Assert.False(propertyBag.TryGetValue(key, out _));
    }

    [Fact]
    public void Scope_WithDifferentTypes_WorksIndependently()
    {
        var propertyBag = new DefaultPropertyBag();
        var stringKey = new PropertyBagKey<string>("Value");
        var intKey = new PropertyBagKey<int>("Value");

        propertyBag.Set(stringKey, "StringValue");
        propertyBag.Set(intKey, 42);

        using (new DefaultPropertyBagScope(propertyBag, stringKey))
        {
            Assert.True(propertyBag.TryGetValue(stringKey, out _));
            Assert.True(propertyBag.TryGetValue(intKey, out _));
        }

        // Only string key should be removed
        Assert.False(propertyBag.TryGetValue(stringKey, out _));
        Assert.True(propertyBag.TryGetValue(intKey, out var intValue));
        Assert.Equal(42, intValue);
    }

    #endregion
}
