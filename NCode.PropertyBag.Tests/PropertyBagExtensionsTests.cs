namespace NCode.PropertyBag.Tests;

public class PropertyBagExtensionsTests
{
    #region Set Extension Method Tests

    [Fact]
    public void Set_WithValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new DefaultPropertyBag();
        const string userName = "TestUser";

        propertyBag.Set(userName);

        var key = new PropertyBagKey<string>("userName");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestUser", retrieved);
    }

    [Fact]
    public void Set_WithIntValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new DefaultPropertyBag();
        const int userId = 42;

        propertyBag.Set(userId);

        var key = new PropertyBagKey<int>("userId");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal(42, retrieved);
    }

    [Fact]
    public void Set_WithNullableValue_InfersKeyNameFromVariable()
    {
        var propertyBag = new DefaultPropertyBag();
        string? optionalValue = "HasValue";

        propertyBag.Set(optionalValue);

        var key = new PropertyBagKey<string?>("optionalValue");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("HasValue", retrieved);
    }

    [Fact]
    public void Set_ReturnsPropertyBagForChaining()
    {
        var propertyBag = new DefaultPropertyBag();
        const string value1 = "First";
        const int value2 = 100;

        var result = propertyBag.Set(value1).Set(value2);

        Assert.Same(propertyBag, result);
    }

    [Fact]
    public void Set_WithExplicitName_UsesProvidedName()
    {
        var propertyBag = new DefaultPropertyBag();
        const string someVariable = "TestValue";

        propertyBag.Set(someVariable, "customKeyName");

        var key = new PropertyBagKey<string>("customKeyName");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestValue", retrieved);
    }

    [Fact]
    public void Set_WithNullName_UsesEmptyString()
    {
        var propertyBag = new DefaultPropertyBag();
        const string value = "TestValue";

        propertyBag.Set(value, null);

        var key = new PropertyBagKey<string>(string.Empty);
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestValue", retrieved);
    }

    [Fact]
    public void Set_OverwritesExistingValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("testValue");
        propertyBag.Set(key, "Original");
        const string testValue = "Updated";

        propertyBag.Set(testValue);

        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("Updated", retrieved);
    }

    [Fact]
    public void Set_WithComplexType_InfersKeyName()
    {
        var propertyBag = new DefaultPropertyBag();
        var configuration = new TestConfiguration { Timeout = 30, Endpoint = "https://api.example.com" };

        propertyBag.Set(configuration);

        var key = new PropertyBagKey<TestConfiguration>("configuration");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.NotNull(retrieved);
        Assert.Equal(30, retrieved.Timeout);
        Assert.Equal("https://api.example.com", retrieved.Endpoint);
    }

    [Fact]
    public void Set_WithDateTimeValue_InfersKeyName()
    {
        var propertyBag = new DefaultPropertyBag();
        var createdAt = new DateTime(2026, 1, 18, 12, 0, 0, DateTimeKind.Utc);

        propertyBag.Set(createdAt);

        var key = new PropertyBagKey<DateTime>("createdAt");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal(new DateTime(2026, 1, 18, 12, 0, 0, DateTimeKind.Utc), retrieved);
    }

    [Fact]
    public void Set_ChainMultipleValues_AllValuesStored()
    {
        var propertyBag = new DefaultPropertyBag();
        const string name = "Alice";
        const int age = 30;
        const bool isActive = true;

        propertyBag
            .Set(name)
            .Set(age)
            .Set(isActive);

        Assert.True(propertyBag.TryGetValue(new PropertyBagKey<string>("name"), out var retrievedName));
        Assert.True(propertyBag.TryGetValue(new PropertyBagKey<int>("age"), out var retrievedAge));
        Assert.True(propertyBag.TryGetValue(new PropertyBagKey<bool>("isActive"), out var retrievedIsActive));
        Assert.Equal("Alice", retrievedName);
        Assert.Equal(30, retrievedAge);
        Assert.True(retrievedIsActive);
    }

    #endregion

    #region TryGet Extension Method Tests

    [Fact]
    public void TryGet_WhenValueExists_ReturnsTrueAndValue()
    {
        var propertyBag = new DefaultPropertyBag();
        propertyBag.Set("TestUser", "testKey");

        var result = propertyBag.TryGet(out string? value, "testKey");

        Assert.True(result);
        Assert.Equal("TestUser", value);
    }

    [Fact]
    public void TryGet_WhenValueNotExists_ReturnsFalseAndDefault()
    {
        var propertyBag = new DefaultPropertyBag();

        string? userName;
        var result = propertyBag.TryGet(out userName);

        Assert.False(result);
        Assert.Null(userName);
    }

    [Fact]
    public void TryGet_WithIntValue_ReturnsTrueAndValue()
    {
        var propertyBag = new DefaultPropertyBag();
        propertyBag.Set(42, "testKey");

        var result = propertyBag.TryGet(out int value, "testKey");

        Assert.True(result);
        Assert.Equal(42, value);
    }

    [Fact]
    public void TryGet_WithIntValue_WhenNotExists_ReturnsFalseAndZero()
    {
        var propertyBag = new DefaultPropertyBag();

        int userId;
        var result = propertyBag.TryGet(out userId);

        Assert.False(result);
        Assert.Equal(0, userId);
    }

    [Fact]
    public void TryGet_WithExplicitName_UsesProvidedName()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("customKeyName");
        propertyBag.Set(key, "TestValue");

        var result = propertyBag.TryGet(out string? someVariable, "customKeyName");

        Assert.True(result);
        Assert.Equal("TestValue", someVariable);
    }

    [Fact]
    public void TryGet_WithNullName_UsesEmptyString()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>(string.Empty);
        propertyBag.Set(key, "TestValue");

        var result = propertyBag.TryGet(out string? value, null);

        Assert.True(result);
        Assert.Equal("TestValue", value);
    }

    [Fact]
    public void TryGet_WithComplexType_ReturnsTrueAndValue()
    {
        var propertyBag = new DefaultPropertyBag();
        var original = new TestConfiguration { Timeout = 30, Endpoint = "https://api.example.com" };
        propertyBag.Set(original, "testKey");

        var result = propertyBag.TryGet(out TestConfiguration? configuration, "testKey");

        Assert.True(result);
        Assert.NotNull(configuration);
        Assert.Equal(30, configuration.Timeout);
        Assert.Equal("https://api.example.com", configuration.Endpoint);
    }

    [Fact]
    public void TryGet_WithWrongType_ReturnsFalse()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("value");
        propertyBag.Set(key, "StringValue");

        int value;
        var result = propertyBag.TryGet(out value);

        Assert.False(result);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryGet_OnReadOnlyPropertyBag_Works()
    {
        var propertyBag = new DefaultPropertyBag();
        propertyBag.Set("TestUser", "testKey");
        IReadOnlyPropertyBag readOnlyBag = propertyBag;

        var result = readOnlyBag.TryGet(out string? value, "testKey");

        Assert.True(result);
        Assert.Equal("TestUser", value);
    }

    #endregion

    #region Round-Trip Tests

    [Fact]
    public void SetAndTryGet_WithSameVariableName_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        const string userName = "TestUser";

        propertyBag.Set(userName);

        string? userName2;
        var result = propertyBag.TryGet(out userName2);

        Assert.False(result); // "userName2" != "userName"
    }

    [Fact]
    public void SetAndTryGet_WithExplicitMatchingNames_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        const string inputValue = "TestValue";

        propertyBag.Set(inputValue, "sharedKey");
        var result = propertyBag.TryGet(out string? outputValue, "sharedKey");

        Assert.True(result);
        Assert.Equal("TestValue", outputValue);
    }

    [Fact]
    public void SetAndTryGet_MultipleTypes_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();

        propertyBag
            .Set("Alice", "name")
            .Set(30, "age")
            .Set(true, "isActive")
            .Set(DateTime.UtcNow, "createdAt");

        Assert.True(propertyBag.TryGet(out string? name, "name"));
        Assert.True(propertyBag.TryGet(out int age, "age"));
        Assert.True(propertyBag.TryGet(out bool isActive, "isActive"));
        Assert.True(propertyBag.TryGet(out DateTime createdAt, "createdAt"));

        Assert.Equal("Alice", name);
        Assert.Equal(30, age);
        Assert.True(isActive);
    }

    #endregion

    #region CallerArgumentExpression Behavior Tests

    [Fact]
    public void Set_CallerArgumentExpression_CapturesVariableName()
    {
        var propertyBag = new DefaultPropertyBag();
        const string myVariableName = "TestValue";

        propertyBag.Set(myVariableName);

        // The key name should be "myVariableName" (the variable name, not the value)
        var key = new PropertyBagKey<string>("myVariableName");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal("TestValue", retrieved);
    }

    [Fact]
    public void Set_CallerArgumentExpression_CapturesPropertyAccess()
    {
        var propertyBag = new DefaultPropertyBag();
        var config = new TestConfiguration { Timeout = 60 };

        propertyBag.Set(config.Timeout);

        // The key name should be "config.Timeout" (the property access expression)
        var key = new PropertyBagKey<int>("config.Timeout");
        Assert.True(propertyBag.TryGetValue(key, out var retrieved));
        Assert.Equal(60, retrieved);
    }

    [Fact]
    public void TryGet_CallerArgumentExpression_CapturesOutVariableName()
    {
        // CallerArgumentExpression on an out parameter captures the variable name
        // when using an existing variable (not inline declaration)
        var propertyBag = new DefaultPropertyBag();

        // Store with key name matching the out variable name
        var key = new PropertyBagKey<string>("retrievedValue");
        propertyBag.Set(key, "TestValue");

        // Declare variable first, then use with out - this captures just "retrievedValue"
        string? retrievedValue;
        var result = propertyBag.TryGet(out retrievedValue);

        Assert.True(result);
        Assert.Equal("TestValue", retrievedValue);
    }

    [Fact]
    public void TryGet_CallerArgumentExpression_DifferentVariableName_ReturnsFalse()
    {
        var propertyBag = new DefaultPropertyBag();
        var key = new PropertyBagKey<string>("storedKey");
        propertyBag.Set(key, "TestValue");

        // TryGet infers "differentKey" which doesn't match "storedKey"
        string? differentKey;
        var result = propertyBag.TryGet(out differentKey);

        Assert.False(result);
        Assert.Null(differentKey);
    }

    [Fact]
    public void SetAndTryGet_CallerArgumentExpression_MatchingVariableNames_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        const string sharedKeyName = "StoredValue";

        // Set using CallerArgumentExpression (key = "sharedKeyName")
        propertyBag.Set(sharedKeyName);

        // TryGet with different variable name should fail (key = "sharedKeyName2")
        string? sharedKeyName2;
        var result = propertyBag.TryGet(out sharedKeyName2);

        // This should fail because "sharedKeyName2" != "sharedKeyName"
        Assert.False(result);
    }

    [Fact]
    public void SetAndTryGet_CallerArgumentExpression_SameVariableNameReused_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        string? userName = "Alice";

        // Set using CallerArgumentExpression (key = "userName")
        propertyBag.Set(userName);

        // Reassign the variable (to prove we retrieve from the bag, not the variable)
        userName = string.Empty;

        // TryGet with same variable name should find it (key = "userName")
        var result = propertyBag.TryGet(out userName);

        Assert.True(result);
        Assert.Equal("Alice", userName);
    }

    [Fact]
    public void SetAndTryGet_CallerArgumentExpression_IntType_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        int counter = 42;

        // Set using CallerArgumentExpression (key = "counter")
        propertyBag.Set(counter);

        // Reset to prove retrieval
        counter = 0;

        // TryGet with same variable name (key = "counter")
        var result = propertyBag.TryGet(out counter);

        Assert.True(result);
        Assert.Equal(42, counter);
    }

    [Fact]
    public void SetAndTryGet_CallerArgumentExpression_MultipleValues_RoundTrips()
    {
        var propertyBag = new DefaultPropertyBag();
        string? name = "Alice";
        int age = 30;
        bool isActive = true;

        // Set all using CallerArgumentExpression
        propertyBag
            .Set(name)
            .Set(age)
            .Set(isActive);

        // Reset all to prove retrieval
        name = string.Empty;
        age = 0;
        isActive = false;

        // TryGet all using same variable names
        Assert.True(propertyBag.TryGet(out name));
        Assert.True(propertyBag.TryGet(out age));
        Assert.True(propertyBag.TryGet(out isActive));

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
        propertyBag.Set(connectionString);

        // Clear the local variable to prove we're getting from the bag
        connectionString = string.Empty;

        // TryGet with a variable with a DIFFERENT name won't find it
        string? differentName;
        var result1 = propertyBag.TryGet(out differentName);
        Assert.False(result1);

        // But TryGet with explicit name will find it
        string? anyName;
        var result2 = propertyBag.TryGet(out anyName, "connectionString");
        Assert.True(result2);
        Assert.Equal("Server=localhost", anyName);
    }

    #endregion

    #region Helper Classes

    private class TestConfiguration
    {
        public int Timeout { get; set; }
        public string? Endpoint { get; set; }
    }

    #endregion
}
