namespace NCode.PropertyBag.Tests;

public class PropertyBagKeyGenericTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithName_SetsNameProperty()
    {
        var key = new PropertyBagKey<string>("TestKey");

        Assert.Equal("TestKey", key.Name);
    }

    [Fact]
    public void Constructor_WithName_SetsTypeProperty()
    {
        var key = new PropertyBagKey<string>("TestKey");

        Assert.Equal(typeof(string), key.Type);
    }

    [Fact]
    public void Constructor_WithEmptyName_SetsEmptyName()
    {
        var key = new PropertyBagKey<string>(string.Empty);

        Assert.Equal(string.Empty, key.Name);
    }

    #endregion

    #region Type Property Tests

    [Fact]
    public void Type_ReturnsTypeOfT_ForString()
    {
        var key = new PropertyBagKey<string>("TestKey");

        Assert.Equal(typeof(string), key.Type);
    }

    [Fact]
    public void Type_ReturnsTypeOfT_ForInt()
    {
        var key = new PropertyBagKey<int>("TestKey");

        Assert.Equal(typeof(int), key.Type);
    }

    [Fact]
    public void Type_ReturnsTypeOfT_ForDateTime()
    {
        var key = new PropertyBagKey<DateTime>("TestKey");

        Assert.Equal(typeof(DateTime), key.Type);
    }

    [Fact]
    public void Type_ReturnsTypeOfT_ForCustomClass()
    {
        var key = new PropertyBagKey<TestClass>("TestKey");

        Assert.Equal(typeof(TestClass), key.Type);
    }

    #endregion

    #region Name Property Tests

    [Fact]
    public void Name_ReturnsConstructorName()
    {
        var key = new PropertyBagKey<string>("MyKey");

        Assert.Equal("MyKey", key.Name);
    }

    #endregion

    #region Equals Tests

    [Fact]
    public void Equals_WithSameName_ReturnsTrue()
    {
        var key1 = new PropertyBagKey<string>("TestKey");
        var key2 = new PropertyBagKey<string>("TestKey");

        Assert.True(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithDifferentName_ReturnsFalse()
    {
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<string>("Key2");

        Assert.False(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithObject_WhenSame_ReturnsTrue()
    {
        var key1 = new PropertyBagKey<string>("TestKey");
        object key2 = new PropertyBagKey<string>("TestKey");

        Assert.True(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithObject_WhenDifferentName_ReturnsFalse()
    {
        var key1 = new PropertyBagKey<string>("Key1");
        object key2 = new PropertyBagKey<string>("Key2");

        Assert.False(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var key = new PropertyBagKey<string>("TestKey");

        Assert.False(key.Equals(null));
    }

    [Fact]
    public void Equals_WithDifferentGenericType_ReturnsFalse()
    {
        var key1 = new PropertyBagKey<string>("TestKey");
        object key2 = new PropertyBagKey<int>("TestKey");

        Assert.False(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithNonPropertyBagKeyObject_ReturnsFalse()
    {
        var key = new PropertyBagKey<string>("TestKey");

        Assert.False(key.Equals("TestKey"));
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    public void GetHashCode_WithSameName_ReturnsSameHashCode()
    {
        var key1 = new PropertyBagKey<string>("TestKey");
        var key2 = new PropertyBagKey<string>("TestKey");

        Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentName_ReturnsDifferentHashCode()
    {
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<string>("Key2");

        Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentType_ReturnsDifferentHashCode()
    {
        var key1 = new PropertyBagKey<string>("TestKey");
        var key2 = new PropertyBagKey<int>("TestKey");

        Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
    }

    #endregion

    #region Equality Operator Tests

    [Fact]
    public void EqualityOperator_WhenSameName_ReturnsTrue()
    {
        var key1 = new PropertyBagKey<string>("TestKey");
        var key2 = new PropertyBagKey<string>("TestKey");

        Assert.True(key1 == key2);
    }

    [Fact]
    public void EqualityOperator_WhenDifferentName_ReturnsFalse()
    {
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<string>("Key2");

        Assert.False(key1 == key2);
    }

    [Fact]
    public void InequalityOperator_WhenDifferentName_ReturnsTrue()
    {
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<string>("Key2");

        Assert.True(key1 != key2);
    }

    [Fact]
    public void InequalityOperator_WhenSameName_ReturnsFalse()
    {
        var key1 = new PropertyBagKey<string>("TestKey");
        var key2 = new PropertyBagKey<string>("TestKey");

        Assert.False(key1 != key2);
    }

    #endregion

    #region Implicit Conversion Tests

    [Fact]
    public void ImplicitConversion_ToNonGeneric_PreservesType()
    {
        var genericKey = new PropertyBagKey<string>("TestKey");

        PropertyBagKey nonGenericKey = genericKey;

        Assert.Equal(typeof(string), nonGenericKey.Type);
    }

    [Fact]
    public void ImplicitConversion_ToNonGeneric_PreservesName()
    {
        var genericKey = new PropertyBagKey<string>("TestKey");

        PropertyBagKey nonGenericKey = genericKey;

        Assert.Equal("TestKey", nonGenericKey.Name);
    }

    [Fact]
    public void ImplicitConversion_CreatesEquivalentNonGenericKey()
    {
        var genericKey = new PropertyBagKey<int>("Counter");

        PropertyBagKey nonGenericKey = genericKey;
        var expectedNonGeneric = new PropertyBagKey(typeof(int), "Counter");

        Assert.Equal(expectedNonGeneric, nonGenericKey);
    }

    [Fact]
    public void ImplicitConversion_DifferentGenericTypes_CreateDifferentNonGenericKeys()
    {
        var stringKey = new PropertyBagKey<string>("Key");
        var intKey = new PropertyBagKey<int>("Key");

        PropertyBagKey nonGenericString = stringKey;
        PropertyBagKey nonGenericInt = intKey;

        Assert.NotEqual(nonGenericString, nonGenericInt);
    }

    #endregion

    #region Dictionary Usage Tests

    [Fact]
    public void GenericKey_CanBeUsedAsDictionaryKey()
    {
        var key1 = new PropertyBagKey<string>("Key1");
        var key2 = new PropertyBagKey<string>("Key2");
        var dict = new Dictionary<PropertyBagKey<string>, string>
        {
            [key1] = "Value1",
            [key2] = "Value2"
        };

        Assert.Equal("Value1", dict[key1]);
        Assert.Equal("Value2", dict[key2]);
    }

    [Fact]
    public void GenericKey_SameName_OverwritesInDictionary()
    {
        var key1 = new PropertyBagKey<string>("TestKey");
        var key2 = new PropertyBagKey<string>("TestKey");
        var dict = new Dictionary<PropertyBagKey<string>, string> { [key1] = "Original" };

        dict[key2] = "Updated";

        Assert.Single(dict);
        Assert.Equal("Updated", dict[key1]);
    }

    #endregion

    #region Cross-Type Comparison Tests

    [Fact]
    public void SameNameDifferentTypes_AreNotEqual()
    {
        var stringKey = new PropertyBagKey<string>("Key");
        var intKey = new PropertyBagKey<int>("Key");

        // They can't be directly compared with == due to different types,
        // but when converted to non-generic they should not be equal
        PropertyBagKey nonGenericString = stringKey;
        PropertyBagKey nonGenericInt = intKey;

        Assert.NotEqual(nonGenericString, nonGenericInt);
    }

    [Fact]
    public void SameNameSameType_AreEqual()
    {
        var key1 = new PropertyBagKey<string>("Key");
        var key2 = new PropertyBagKey<string>("Key");

        Assert.Equal(key1, key2);
        Assert.True(key1 == key2);
    }

    #endregion

    #region Helper Classes

    private class TestClass
    {
        public string? Value { get; set; }
    }

    #endregion
}
