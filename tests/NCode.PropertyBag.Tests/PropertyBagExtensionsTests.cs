// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

public class PropertyBagExtensionsTests
{
    #region SetValue Extension Method Tests

    [Fact]
    public void SetValue_WhenBagIsNull_ThrowsArgumentNullException()
    {
        IPropertyBag? propertyBag = null;
        const string userName = "Alice";

        var exception = Assert.Throws<ArgumentNullException>(() => propertyBag!.SetValue(userName));

        Assert.Equal("bag", exception.ParamName);
    }

    [Fact]
    public void SetValue_WithValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new PropertyBagDictionary();
        const string userName = "TestUser";

        propertyBag.SetValue(userName);

        var key = new PropertyBagKey<string>("userName");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestUser", retrieved);
    }

    [Fact]
    public void SetValue_WithIntValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new PropertyBagDictionary();
        const int userId = 42;

        propertyBag.SetValue(userId);

        var key = new PropertyBagKey<int>("userId");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal(42, retrieved);
    }

    [Fact]
    public void SetValue_WithNullableValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new PropertyBagDictionary();
        string? optionalValue = "HasValue";

        propertyBag.SetValue(optionalValue);

        var key = new PropertyBagKey<string?>("optionalValue");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("HasValue", retrieved);
    }

    [Fact]
    public void SetValue_ReturnsPropertyBagForChaining()
    {
        var propertyBag = new PropertyBagDictionary();
        const string value1 = "First";
        const int value2 = 100;

        var result = propertyBag.SetValue(value1).SetValue(value2);

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void SetValue_WithExplicitName_UsesProvidedName()
    {
        var propertyBag = new PropertyBagDictionary();
        const string someVariable = "TestValue";

        propertyBag.SetValue(someVariable, "customKeyName");

        var key = new PropertyBagKey<string>("customKeyName");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestValue", retrieved);
    }

    [Fact]
    public void SetValue_WithNullName_UsesEmptyString()
    {
        var propertyBag = new PropertyBagDictionary();
        const string value = "TestValue";

        propertyBag.SetValue(value, null);

        var key = new PropertyBagKey<string>(string.Empty);
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestValue", retrieved);
    }

    [Fact]
    public void SetValue_OverwritesExistingValue()
    {
        var propertyBag = new PropertyBagDictionary();
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
        var propertyBag = new PropertyBagDictionary();
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
        var propertyBag = new PropertyBagDictionary();
        var createdAt = new DateTime(2026, 1, 18, 12, 0, 0, DateTimeKind.Utc);

        propertyBag.SetValue(createdAt);

        var key = new PropertyBagKey<DateTime>("createdAt");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal(new DateTime(2026, 1, 18, 12, 0, 0, DateTimeKind.Utc), retrieved);
    }

    [Fact]
    public void SetValue_ChainMultipleValues_AllValuesStored()
    {
        var propertyBag = new PropertyBagDictionary();
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

    [Fact]
    public void SetValue_WithKeyAndStringValue_CallsTheInterfaceMethod()
    {
        IPropertyBag propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("UserName");

        propertyBag.SetValue(key, "Alice");

        var found = propertyBag.TryGetValue(key, out var value);
        var wrongKey = new PropertyBagKey<PropertyBagKey<string>>("Alice");
        var foundWrongKey = propertyBag.TryGetValue(wrongKey, out _);

        Assert.True(found);
        Assert.Equal("Alice", value);
        Assert.False(foundWrongKey);
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void SetValueAndTryGetValue_WithDifferentVariableName_ReturnsFalse()
    {
        var propertyBag = new PropertyBagDictionary();
        const string userName = "TestUser";

        propertyBag.SetValue(userName);

        string? userName2;
        var result = propertyBag.TryGetValue(out userName2);

        Assert.False(result); // "userName2" != "userName"
    }

    [Fact]
    public void SetValueAndTryGetValue_WithExplicitMatchingNames_RoundTrips()
    {
        var propertyBag = new PropertyBagDictionary();
        const string inputValue = "TestValue";

        propertyBag.SetValue(inputValue, "sharedKey");
        var result = propertyBag.TryGetValue(out string? outputValue, "sharedKey");

        Assert.True(result);
        Assert.Equal("TestValue", outputValue);
    }

    [Fact]
    public void SetValueAndTryGetValue_MultipleTypes_RoundTrips()
    {
        var propertyBag = new PropertyBagDictionary();

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
        var propertyBag = new PropertyBagDictionary();
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
        var propertyBag = new PropertyBagDictionary();
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
        var propertyBag = new PropertyBagDictionary();

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
        var propertyBag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>("storedKey");
        propertyBag.SetValue(key, "TestValue");

        // TryGetValue infers "differentKey" which doesn't match "storedKey"
        string? differentKey;
        var result = propertyBag.TryGetValue(out differentKey);

        Assert.False(result);
        Assert.Null(differentKey);
    }

    [Fact]
    public void SetValueAndTryGetValue_CallerArgumentExpression_DifferentVariableNames_ReturnsFalse()
    {
        var propertyBag = new PropertyBagDictionary();
        const string sharedKeyName = "StoredValue";

        // SetValue using CallerArgumentExpression (key = "sharedKeyName")
        propertyBag.SetValue(sharedKeyName);

        // TryGetValue with different variable name should fail (key = "sharedKeyName2")
        string? sharedKeyName2;
        var result = propertyBag.TryGetValue(out sharedKeyName2);

        // This should fail because "sharedKeyName2" != "sharedKeyName"
        Assert.False(result);
    }

    [Fact]
    public void SetValueAndTryGetValue_CallerArgumentExpression_SameVariableNameReused_RoundTrips()
    {
        var propertyBag = new PropertyBagDictionary();
        string? userName = "Alice";

        // SetValue using CallerArgumentExpression (key = "userName")
        propertyBag.SetValue(userName);

        // Reassign the variable (to prove we retrieve from the bag, not the variable)
        userName = string.Empty;

        // TryGetValue with same variable name should find it (key = "userName")
        var result = propertyBag.TryGetValue(out userName);

        Assert.True(result);
        Assert.Equal("Alice", userName);
    }

    [Fact]
    public void SetValueAndTryGetValue_CallerArgumentExpression_IntType_RoundTrips()
    {
        var propertyBag = new PropertyBagDictionary();
        int counter = 42;

        // SetValue using CallerArgumentExpression (key = "counter")
        propertyBag.SetValue(counter);

        // Reset to prove retrieval
        counter = 0;

        // TryGetValue with same variable name (key = "counter")
        var result = propertyBag.TryGetValue(out counter);

        Assert.True(result);
        Assert.Equal(42, counter);
    }

    [Fact]
    public void SetValueAndTryGetValue_CallerArgumentExpression_MultipleValues_RoundTrips()
    {
        var propertyBag = new PropertyBagDictionary();
        string? name = "Alice";
        int age = 30;
        bool isActive = true;

        // SetValue all using CallerArgumentExpression
        propertyBag.SetValue(name).SetValue(age).SetValue(isActive);

        // Reset all to prove retrieval
        name = string.Empty;
        age = 0;
        isActive = false;

        // TryGetValue all using same variable names
        Assert.True(propertyBag.TryGetValue(out name));
        Assert.True(propertyBag.TryGetValue(out age));
        Assert.True(propertyBag.TryGetValue(out isActive));

        Assert.Equal("Alice", name);
        Assert.Equal(30, age);
        Assert.True(isActive);
    }

    [Fact]
    public void SetValueThenTryGetValue_WithMatchingVariableNames_Works()
    {
        var propertyBag = new PropertyBagDictionary();

        // Using variable with same name for both SetValue and TryGetValue
        string connectionString = "Server=localhost";
        propertyBag.SetValue(connectionString);

        // Clear the local variable to prove we're getting from the bag
        connectionString = string.Empty;

        // TryGetValue with a variable with a DIFFERENT name won't find it
        string? differentName;
        var result1 = propertyBag.TryGetValue(out differentName);
        Assert.False(result1);

        // But TryGetValue with explicit name will find it
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
