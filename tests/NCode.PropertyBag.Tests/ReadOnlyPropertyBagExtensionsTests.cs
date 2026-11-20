// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

public class ReadOnlyPropertyBagExtensionsTests
{
    #region TryGetValue Extension Method Tests

    [Fact]
    public void TryGetValue_WhenBagIsNull_ThrowsArgumentNullException()
    {
        IReadOnlyPropertyBag? propertyBag = null;
        string? userName = null;

        var exception = Assert.Throws<ArgumentNullException>(() =>
            propertyBag!.TryGetValue(out userName)
        );

        Assert.Equal("bag", exception.ParamName);
        Assert.Null(userName);
    }

    [Fact]
    public void TryGetValue_WhenValueExists_ReturnsTrueAndValue()
    {
        var propertyBag = new PropertyBagDictionary();
        propertyBag.SetValue("TestUser", "testKey");

        var result = propertyBag.TryGetValue(out string? value, "testKey");

        Assert.True(result);
        Assert.Equal("TestUser", value);
    }

    [Fact]
    public void TryGetValue_WhenValueNotExists_ReturnsFalseAndDefault()
    {
        var propertyBag = new PropertyBagDictionary();

        string? userName;
        var result = propertyBag.TryGetValue(out userName);

        Assert.False(result);
        Assert.Null(userName);
    }

    [Fact]
    public void TryGetValue_WithIntValue_ReturnsTrueAndValue()
    {
        var propertyBag = new PropertyBagDictionary();
        propertyBag.SetValue(42, "testKey");

        var result = propertyBag.TryGetValue(out int value, "testKey");

        Assert.True(result);
        Assert.Equal(42, value);
    }

    [Fact]
    public void TryGetValue_WithIntValue_WhenNotExists_ReturnsFalseAndZero()
    {
        var propertyBag = new PropertyBagDictionary();

        int userId;
        var result = propertyBag.TryGetValue(out userId);

        Assert.False(result);
        Assert.Equal(0, userId);
    }

    [Fact]
    public void TryGetValue_WithExplicitName_UsesProvidedName()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("customKeyName");
        propertyBag.SetValue(key, "TestValue");

        var result = propertyBag.TryGetValue(out string? someVariable, "customKeyName");

        Assert.True(result);
        Assert.Equal("TestValue", someVariable);
    }

    [Fact]
    public void TryGetValue_WithNullName_UsesEmptyString()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>(string.Empty);
        propertyBag.SetValue(key, "TestValue");

        var result = propertyBag.TryGetValue(out string? value, null);

        Assert.True(result);
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void TryGetValue_WithComplexType_ReturnsTrueAndValue()
    {
        var propertyBag = new PropertyBagDictionary();
        var original = new TestConfiguration { Timeout = 30, Endpoint = "https://api.example.com" };
        propertyBag.SetValue(original, "testKey");

        var result = propertyBag.TryGetValue(out TestConfiguration? configuration, "testKey");

        Assert.True(result);
        Assert.NotNull(configuration);
        Assert.Equal(30, configuration.Timeout);
        Assert.Equal("https://api.example.com", configuration.Endpoint);
    }

    [Fact]
    public void TryGetValue_WithWrongType_ReturnsFalse()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("value");
        propertyBag.SetValue(key, "StringValue");

        int value;
        var result = propertyBag.TryGetValue(out value);

        Assert.False(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryGetValue_OnReadOnlyPropertyBag_Works()
    {
        var propertyBag = new PropertyBagDictionary();
        propertyBag.SetValue("TestUser", "testKey");
        IReadOnlyPropertyBag readOnlyBag = propertyBag;

        var result = readOnlyBag.TryGetValue(out string? value, "testKey");

        Assert.True(result);
        Assert.Equal("TestUser", value);
    }

    [Fact]
    public void TryGetValue_WithDeclaredVariable_ReadsTheKeyNamedAfterIt()
    {
        var propertyBag = new PropertyBagDictionary();
        propertyBag.SetValue(new PropertyBagKey<string>("connectionString"), "Server=localhost");

        string? connectionString;
        var result = propertyBag.TryGetValue(out connectionString);

        Assert.True(result);
        Assert.Equal("Server=localhost", connectionString);
    }

    #endregion

    #region GetValue Tests

    [Fact]
    public void GetValue_WhenBagIsNull_ThrowsArgumentNullException()
    {
        IReadOnlyPropertyBag? propertyBag = null;
        var key = new PropertyBagKey<string>("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => propertyBag!.GetValue(key));

        Assert.Equal("bag", exception.ParamName);
    }

    [Fact]
    public void GetValue_WhenValueExists_ReturnsValue()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("Name");
        propertyBag.SetValue(key, "TestUser");

        var result = propertyBag.GetValue(key);

        Assert.Equal("TestUser", result);
    }

    [Fact]
    public void GetValue_WhenNullIsStored_ReturnsNull()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string?>("Name");
        propertyBag.SetValue(key, null);

        var result = propertyBag.GetValue(key);

        Assert.Null(result);
    }

    [Fact]
    public void GetValue_WhenKeyIsMissing_ThrowsKeyNotFoundExceptionNamingTheKey()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("Missing");

        var exception = Assert.Throws<KeyNotFoundException>(() => propertyBag.GetValue(key));

        Assert.Contains("Missing (System.String)", exception.Message);
    }

    #endregion

    #region GetValueOrDefault Tests

    [Fact]
    public void GetValueOrDefault_WhenBagIsNull_ThrowsArgumentNullException()
    {
        IReadOnlyPropertyBag? propertyBag = null;
        var key = new PropertyBagKey<string>("Name");

        var exception = Assert.Throws<ArgumentNullException>(() =>
            propertyBag!.GetValueOrDefault(key)
        );

        Assert.Equal("bag", exception.ParamName);
    }

    [Fact]
    public void GetValueOrDefault_WhenValueExists_ReturnsValue()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("Name");
        propertyBag.SetValue(key, "TestUser");

        var result = propertyBag.GetValueOrDefault(key, "Fallback");

        Assert.Equal("TestUser", result);
    }

    [Fact]
    public void GetValueOrDefault_WhenNullIsStored_ReturnsNull()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string?>("Name");
        propertyBag.SetValue(key, null);

        var result = propertyBag.GetValueOrDefault(key, "Fallback");

        Assert.Null(result);
    }

    [Fact]
    public void GetValueOrDefault_WhenKeyIsMissing_ReturnsProvidedDefault()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("Missing");

        var result = propertyBag.GetValueOrDefault(key, "Fallback");

        Assert.Equal("Fallback", result);
    }

    [Fact]
    public void GetValueOrDefault_WhenKeyIsMissingAndNoDefault_ReturnsDefault()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<int>("Missing");

        var result = propertyBag.GetValueOrDefault(key);

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetValueOrDefault_OnPropertyBagDictionary_CallsExtension()
    {
        PropertyBagDictionary propertyBag = new();
        var key = new PropertyBagKey<string>("Name");
        propertyBag.SetValue(key, "TestUser");

        var result = propertyBag.GetValueOrDefault(key, "Fallback");

        Assert.Equal("TestUser", result);
    }

    #endregion

    #region ContainsKey Tests

    [Fact]
    public void ContainsKey_WhenBagIsNull_ThrowsArgumentNullException()
    {
        IReadOnlyPropertyBag? propertyBag = null;
        var key = new PropertyBagKey<string>("Name");

        var exception = Assert.Throws<ArgumentNullException>(() => propertyBag!.ContainsKey(key));

        Assert.Equal("bag", exception.ParamName);
    }

    [Fact]
    public void ContainsKey_WhenValueExists_ReturnsTrue()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("Name");
        propertyBag.SetValue(key, "TestUser");

        var result = propertyBag.ContainsKey(key);

        Assert.True(result);
    }

    [Fact]
    public void ContainsKey_WhenNullIsStored_ReturnsTrue()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string?>("Name");
        propertyBag.SetValue(key, null);

        var result = propertyBag.ContainsKey(key);

        Assert.True(result);
    }

    [Fact]
    public void ContainsKey_WhenKeyIsMissing_ReturnsFalse()
    {
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("Missing");

        var result = propertyBag.ContainsKey(key);

        Assert.False(result);
    }

    #endregion

    #region Helper Classes

    private sealed class TestConfiguration
    {
        public int Timeout { get; set; }
        public string? Endpoint { get; set; }
    }

    #endregion
}
