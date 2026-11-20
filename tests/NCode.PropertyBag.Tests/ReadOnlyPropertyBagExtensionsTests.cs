// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

public class ReadOnlyPropertyBagExtensionsTests
{
    #region TryGetValue Extension Method Tests

    [Fact]
    public void TryGetValue_WhenValueExists_ReturnsTrueAndValue()
    {
        var propertyBag = new DefaultPropertyBag();
        propertyBag.SetValue("TestUser", "testKey");

        var result = propertyBag.TryGetValue(out string? value, "testKey");

        Assert.True(result);
        Assert.Equal("TestUser", value);
    }

    [Fact]
    public void TryGetValue_WhenValueNotExists_ReturnsFalseAndDefault()
    {
        var propertyBag = new DefaultPropertyBag();

        string? userName;
        var result = propertyBag.TryGetValue(out userName);

        Assert.False(result);
        Assert.Null(userName);
    }

    [Fact]
    public void TryGetValue_WithIntValue_ReturnsTrueAndValue()
    {
        var propertyBag = new DefaultPropertyBag();
        propertyBag.SetValue(42, "testKey");

        var result = propertyBag.TryGetValue(out int value, "testKey");

        Assert.True(result);
        Assert.Equal(42, value);
    }

    [Fact]
    public void TryGetValue_WithIntValue_WhenNotExists_ReturnsFalseAndZero()
    {
        var propertyBag = new DefaultPropertyBag();

        int userId;
        var result = propertyBag.TryGetValue(out userId);

        Assert.False(result);
        Assert.Equal(0, userId);
    }

    [Fact]
    public void TryGetValue_WithExplicitName_UsesProvidedName()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("customKeyName");
        propertyBag.SetValue(key, "TestValue");

        var result = propertyBag.TryGetValue(out string? someVariable, "customKeyName");

        Assert.True(result);
        Assert.Equal("TestValue", someVariable);
    }

    [Fact]
    public void TryGetValue_WithNullName_UsesEmptyString()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>(string.Empty);
        propertyBag.SetValue(key, "TestValue");

        var result = propertyBag.TryGetValue(out string? value, null);

        Assert.True(result);
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void TryGetValue_WithComplexType_ReturnsTrueAndValue()
    {
        var propertyBag = new DefaultPropertyBag();
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
        var propertyBag = new DefaultPropertyBag();
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
        var propertyBag = new DefaultPropertyBag();
        propertyBag.SetValue("TestUser", "testKey");
        IReadOnlyPropertyBag readOnlyBag = propertyBag;

        var result = readOnlyBag.TryGetValue(out string? value, "testKey");

        Assert.True(result);
        Assert.Equal("TestUser", value);
    }

    [Fact]
    public void TryGetValue_WithDeclaredVariable_ReadsTheKeyNamedAfterIt()
    {
        var propertyBag = new DefaultPropertyBag();
        propertyBag.SetValue(new PropertyBagKey<string>("connectionString"), "Server=localhost");

        string? connectionString;
        var result = propertyBag.TryGetValue(out connectionString);

        Assert.True(result);
        Assert.Equal("Server=localhost", connectionString);
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
