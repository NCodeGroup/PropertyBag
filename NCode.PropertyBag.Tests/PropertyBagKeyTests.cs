namespace NCode.PropertyBag.Tests;

public class PropertyBagKeyTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithTypeAndName_SetsProperties()
    {
        var key = new PropertyBagKey(typeof(string), "TestKey");

        Assert.Equal(typeof(string), key.Type);
        Assert.Equal("TestKey", key.Name);
    }

    [Fact]
    public void Constructor_WithDifferentTypes_SetsCorrectType()
    {
        var intKey = new PropertyBagKey(typeof(int), "Key");
        var dateTimeKey = new PropertyBagKey(typeof(DateTime), "Key");

        Assert.Equal(typeof(int), intKey.Type);
        Assert.Equal(typeof(DateTime), dateTimeKey.Type);
    }

    #endregion

    #region Type Property Tests

    [Fact]
    public void Type_ReturnsConstructorType()
    {
        var key = new PropertyBagKey(typeof(double), "TestKey");

        Assert.Equal(typeof(double), key.Type);
    }

    #endregion

    #region Name Property Tests

    [Fact]
    public void Name_ReturnsConstructorName()
    {
        var key = new PropertyBagKey(typeof(string), "MyName");

        Assert.Equal("MyName", key.Name);
    }

    [Fact]
    public void Name_WithEmptyString_ReturnsEmptyString()
    {
        var key = new PropertyBagKey(typeof(string), string.Empty);

        Assert.Equal(string.Empty, key.Name);
    }

    #endregion

    #region Equals Tests

    [Fact]
    public void Equals_WithSameTypeAndName_ReturnsTrue()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        var key2 = new PropertyBagKey(typeof(string), "TestKey");

        Assert.True(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        var key2 = new PropertyBagKey(typeof(int), "TestKey");

        Assert.False(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithDifferentName_ReturnsFalse()
    {
        var key1 = new PropertyBagKey(typeof(string), "Key1");
        var key2 = new PropertyBagKey(typeof(string), "Key2");

        Assert.False(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithObject_WhenSame_ReturnsTrue()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        object key2 = new PropertyBagKey(typeof(string), "TestKey");

        Assert.True(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithObject_WhenDifferent_ReturnsFalse()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        object key2 = new PropertyBagKey(typeof(int), "TestKey");

        Assert.False(key1.Equals(key2));
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        var key = new PropertyBagKey(typeof(string), "TestKey");

        Assert.False(key.Equals(null));
    }

    [Fact]
    public void Equals_WithDifferentObjectType_ReturnsFalse()
    {
        var key = new PropertyBagKey(typeof(string), "TestKey");

        Assert.False(key.Equals("TestKey"));
    }

    #endregion

    #region GetHashCode Tests

    [Fact]
    public void GetHashCode_WithSameTypeAndName_ReturnsSameHashCode()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        var key2 = new PropertyBagKey(typeof(string), "TestKey");

        Assert.Equal(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentType_ReturnsDifferentHashCode()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        var key2 = new PropertyBagKey(typeof(int), "TestKey");

        Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentName_ReturnsDifferentHashCode()
    {
        var key1 = new PropertyBagKey(typeof(string), "Key1");
        var key2 = new PropertyBagKey(typeof(string), "Key2");

        Assert.NotEqual(key1.GetHashCode(), key2.GetHashCode());
    }

    #endregion

    #region Equality Operator Tests

    [Fact]
    public void EqualityOperator_WhenEqual_ReturnsTrue()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        var key2 = new PropertyBagKey(typeof(string), "TestKey");

        Assert.True(key1 == key2);
    }

    [Fact]
    public void EqualityOperator_WhenNotEqual_ReturnsFalse()
    {
        var key1 = new PropertyBagKey(typeof(string), "Key1");
        var key2 = new PropertyBagKey(typeof(string), "Key2");

        Assert.False(key1 == key2);
    }

    [Fact]
    public void InequalityOperator_WhenNotEqual_ReturnsTrue()
    {
        var key1 = new PropertyBagKey(typeof(string), "Key1");
        var key2 = new PropertyBagKey(typeof(string), "Key2");

        Assert.True(key1 != key2);
    }

    [Fact]
    public void InequalityOperator_WhenEqual_ReturnsFalse()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        var key2 = new PropertyBagKey(typeof(string), "TestKey");

        Assert.False(key1 != key2);
    }

    #endregion

    #region Dictionary Usage Tests

    [Fact]
    public void Key_CanBeUsedAsDictionaryKey()
    {
        var key1 = new PropertyBagKey(typeof(string), "Key1");
        var key2 = new PropertyBagKey(typeof(int), "Key2");
        var dict = new Dictionary<PropertyBagKey, object?>
        {
            [key1] = "Value1",
            [key2] = 42
        };

        Assert.Equal("Value1", dict[key1]);
        Assert.Equal(42, dict[key2]);
    }

    [Fact]
    public void Key_SameTypeAndName_OverwritesInDictionary()
    {
        var key1 = new PropertyBagKey(typeof(string), "TestKey");
        var key2 = new PropertyBagKey(typeof(string), "TestKey");
        var dict = new Dictionary<PropertyBagKey, object?> { [key1] = "Original" };

        dict[key2] = "Updated";

        Assert.Single(dict);
        Assert.Equal("Updated", dict[key1]);
    }

    #endregion
}
