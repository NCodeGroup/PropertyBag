// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

public class PropertyBagExtensionsTests
{
    #region SetValue Extension Method Tests

    [Fact]
    public void SetValue_WithValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new DefaultPropertyBag();
        const string userName = "TestUser";

        propertyBag.SetValue(userName);

        var key = new PropertyBagKey<string>("userName");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestUser", retrieved);
    }

    [Fact]
    public void SetValue_WithIntValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new DefaultPropertyBag();
        const int userId = 42;

        propertyBag.SetValue(userId);

        var key = new PropertyBagKey<int>("userId");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal(42, retrieved);
    }

    [Fact]
    public void SetValue_WithNullableValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new DefaultPropertyBag();
        string? optionalValue = "HasValue";

        propertyBag.SetValue(optionalValue);

        var key = new PropertyBagKey<string?>("optionalValue");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("HasValue", retrieved);
    }

    [Fact]
    public void SetValue_ReturnsPropertyBagForChaining()
    {
        var propertyBag = new DefaultPropertyBag();
        const string value1 = "First";
        const int value2 = 100;

        var result = propertyBag.SetValue(value1).SetValue(value2);

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void SetValue_WithExplicitName_UsesProvidedName()
    {
        var propertyBag = new DefaultPropertyBag();
        const string someVariable = "TestValue";

        propertyBag.SetValue(someVariable, "customKeyName");

        var key = new PropertyBagKey<string>("customKeyName");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestValue", retrieved);
    }

    [Fact]
    public void SetValue_WithNullName_UsesEmptyString()
    {
        var propertyBag = new DefaultPropertyBag();
        const string value = "TestValue";

        propertyBag.SetValue(value, null);

        var key = new PropertyBagKey<string>(string.Empty);
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestValue", retrieved);
    }

    [Fact]
    public void SetValue_OverwritesExistingValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("testValue");
        propertyBag.SetValue(key, "Original");
        const string testValue = "Updated";

        propertyBag.SetValue(testValue);

        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("Updated", retrieved);
    }

    [Fact]
    public void SetValue_WithComplexType_InfersKeyName()
    {
        var propertyBag = new DefaultPropertyBag();
        var configuration = new TestConfiguration
        {
            Timeout = 30,
            Endpoint = "https://api.example.com",
        };

        propertyBag.SetValue(configuration);

        var key = new PropertyBagKey<TestConfiguration>("configuration");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.NotNull(retrieved);
        Assert.Equal(30, retrieved.Timeout);
        Assert.Equal("https://api.example.com", retrieved.Endpoint);
    }

    [Fact]
    public void SetValue_WithDateTimeValue_InfersKeyName()
    {
        var propertyBag = new DefaultPropertyBag();
        var createdAt = new DateTime(2026, 1, 18, 12, 0, 0, DateTimeKind.Utc);

        propertyBag.SetValue(createdAt);

        var key = new PropertyBagKey<DateTime>("createdAt");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal(new DateTime(2026, 1, 18, 12, 0, 0, DateTimeKind.Utc), retrieved);
    }

    [Fact]
    public void SetValue_ChainMultipleValues_AllValuesStored()
    {
        var propertyBag = new DefaultPropertyBag();
        const string name = "Alice";
        const int age = 30;
        const bool isActive = true;

        propertyBag.SetValue(name).SetValue(age).SetValue(isActive);

        Assert.True(
            propertyBag.TryGetValue(new PropertyBagKey<string>("name"), out var retrievedName)
        );
        Assert.True(propertyBag.TryGetValue(new PropertyBagKey<int>("age"), out var retrievedAge));
        Assert.True(
            propertyBag.TryGetValue(new PropertyBagKey<bool>("isActive"), out var retrievedIsActive)
        );
        Assert.Equal("Alice", retrievedName);
        Assert.Equal(30, retrievedAge);
        Assert.True(retrievedIsActive);
    }

    #endregion

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

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void SetValueAndTryGetValue_WithSameVariableName_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        const string userName = "TestUser";

        propertyBag.SetValue(userName);

        string? userName2;
        var result = propertyBag.TryGetValue(out userName2);

        Assert.False(result); // "userName2" != "userName"
    }

    [Fact]
    public void SetValueAndTryGetValue_WithExplicitMatchingNames_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        const string inputValue = "TestValue";

        propertyBag.SetValue(inputValue, "sharedKey");
        var result = propertyBag.TryGetValue(out string? outputValue, "sharedKey");

        Assert.True(result);
        Assert.Equal("TestValue", outputValue);
    }

    [Fact]
    public void SetValueAndTryGetValue_MultipleTypes_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();

        propertyBag
            .SetValue("Alice", "name")
            .SetValue(30, "age")
            .SetValue(true, "isActive")
            .SetValue(DateTime.UtcNow, "createdAt");

        Assert.True(propertyBag.TryGetValue(out string? name, "name"));
        Assert.True(propertyBag.TryGetValue(out int age, "age"));
        Assert.True(propertyBag.TryGetValue(out bool isActive, "isActive"));
        Assert.True(propertyBag.TryGetValue(out DateTime createdAt, "createdAt"));

        Assert.Equal("Alice", name);
        Assert.Equal(30, age);
        Assert.True(isActive);
    }

    #endregion

    #region CallerArgumentExpression Behavior Tests

    [Fact]
    public void SetValue_CallerArgumentExpression_CapturesVariableName()
    {
        var propertyBag = new DefaultPropertyBag();
        const string myVariableName = "TestValue";

        propertyBag.SetValue(myVariableName);

        // The key name should be "myVariableName" (the variable name, not the value)
        var key = new PropertyBagKey<string>("myVariableName");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestValue", retrieved);
    }

    [Fact]
    public void SetValue_CallerArgumentExpression_CapturesPropertyAccess()
    {
        var propertyBag = new DefaultPropertyBag();
        var config = new TestConfiguration { Timeout = 60 };

        propertyBag.SetValue(config.Timeout);

        // The key name should be "config.Timeout" (the property access expression)
        var key = new PropertyBagKey<int>("config.Timeout");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal(60, retrieved);
    }

    [Fact]
    public void TryGetValue_CallerArgumentExpression_CapturesOutVariableName()
    {
        // CallerArgumentExpression on an out parameter captures the variable name
        // when using an existing variable (not inline declaration)
        var propertyBag = new DefaultPropertyBag();

        // Store with key name matching the out variable name
        var key = new PropertyBagKey<string>("retrievedValue");
        propertyBag.SetValue(key, "TestValue");

        // Declare variable first, then use with out - this captures just "retrievedValue"
        string? retrievedValue;
        var result = propertyBag.TryGetValue(out retrievedValue);

        Assert.True(result);
        Assert.Equal("TestValue", retrievedValue);
    }

    [Fact]
    public void TryGetValue_CallerArgumentExpression_DifferentVariableName_ReturnsFalse()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("storedKey");
        propertyBag.SetValue(key, "TestValue");

        // TryGet infers "differentKey" which doesn't match "storedKey"
        string? differentKey;
        var result = propertyBag.TryGetValue(out differentKey);

        Assert.False(result);
        Assert.Null(differentKey);
    }

    [Fact]
    public void SetValueAndTryGetValue_CallerArgumentExpression_MatchingVariableNames_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        const string sharedKeyName = "StoredValue";

        // Set using CallerArgumentExpression (key = "sharedKeyName")
        propertyBag.SetValue(sharedKeyName);

        // TryGet with different variable name should fail (key = "sharedKeyName2")
        string? sharedKeyName2;
        var result = propertyBag.TryGetValue(out sharedKeyName2);

        // This should fail because "sharedKeyName2" != "sharedKeyName"
        Assert.False(result);
    }

    [Fact]
    public void SetValueAndTryGetValue_CallerArgumentExpression_SameVariableNameReused_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        string? userName = "Alice";

        // Set using CallerArgumentExpression (key = "userName")
        propertyBag.SetValue(userName);

        // Reassign the variable (to prove we retrieve from the bag, not the variable)
        userName = string.Empty;

        // TryGet with same variable name should find it (key = "userName")
        var result = propertyBag.TryGetValue(out userName);

        Assert.True(result);
        Assert.Equal("Alice", userName);
    }

    [Fact]
    public void SetValueAndTryGetValue_CallerArgumentExpression_IntType_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        int counter = 42;

        // Set using CallerArgumentExpression (key = "counter")
        propertyBag.SetValue(counter);

        // Reset to prove retrieval
        counter = 0;

        // TryGet with same variable name (key = "counter")
        var result = propertyBag.TryGetValue(out counter);

        Assert.True(result);
        Assert.Equal(42, counter);
    }

    [Fact]
    public void SetValueAndTryGetValue_CallerArgumentExpression_MultipleValues_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        string? name = "Alice";
        int age = 30;
        bool isActive = true;

        // Set all using CallerArgumentExpression
        propertyBag.SetValue(name).SetValue(age).SetValue(isActive);

        // Reset all to prove retrieval
        name = string.Empty;
        age = 0;
        isActive = false;

        // TryGet all using same variable names
        Assert.True(propertyBag.TryGetValue(out name));
        Assert.True(propertyBag.TryGetValue(out age));
        Assert.True(propertyBag.TryGetValue(out isActive));

        Assert.Equal("Alice", name);
        Assert.Equal(30, age);
        Assert.True(isActive);
    }

    [Fact]
    public void SetThenTryGet_WithMatchingVariableNames_Works()
    {
        var propertyBag = new DefaultPropertyBag();

        // Using variable with same name for both Set and TryGet
        string connectionString = "Server=localhost";
        propertyBag.SetValue(connectionString);

        // Clear the local variable to prove we're getting from the bag
        connectionString = string.Empty;

        // TryGet with a variable with a DIFFERENT name won't find it
        string? differentName;
        var result1 = propertyBag.TryGetValue(out differentName);
        Assert.False(result1);

        // But TryGet with explicit name will find it
        string? anyName;
        var result2 = propertyBag.TryGetValue(out anyName, "connectionString");
        Assert.True(result2);
        Assert.Equal("Server=localhost", anyName);
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
