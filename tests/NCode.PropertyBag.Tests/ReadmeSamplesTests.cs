// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using ReadmeSamples;

namespace NCode.PropertyBag.Tests;

// dotnet mdsnippets copies each begin-snippet block into README.md (ADR-1002). PropertyBagFactory.Create reads the
// process-wide factory, so the class runs in the static factories collection.
[Collection(StaticFactoryCollection.Name)]
public class ReadmeSamplesTests
{
    // begin-snippet: ReadmeDefineKeys
    // Define keys (typically as static readonly fields)
    public static readonly PropertyBagKey<string> UserNameKey = new("UserName");
    public static readonly PropertyBagKey<int> UserAgeKey = new("UserAge");

    // end-snippet

    #region Basic Usage Tests

    [Fact]
    public void ReadmeDefineKeysSample_Runs()
    {
        Assert.Equal("UserName", UserNameKey.Name);
        Assert.Equal("UserAge", UserAgeKey.Name);
    }

    [Fact]
    public void ReadmeBasicUsageSample_Runs()
    {
        // begin-snippet: ReadmeBasicUsage
        // Create and use a property bag
        var propertyBag = PropertyBagFactory.Create();

        propertyBag.Set(UserNameKey, "Alice").Set(UserAgeKey, 30);

        if (propertyBag.TryGetValue(UserNameKey, out var userName))
        {
            Console.WriteLine($"User: {userName}"); // Output: User: Alice
        }
        // end-snippet

        Assert.Equal("Alice", userName);
        bool foundAge = propertyBag.TryGetValue(UserAgeKey, out var userAge);
        Assert.True(foundAge);
        Assert.Equal(30, userAge);
    }

    #endregion

    #region Scoped Values Tests

    [Fact]
    public void ReadmeScopedValuesSample_Runs()
    {
        IPropertyBag propertyBag = new DefaultPropertyBag();

        // begin-snippet: ReadmeScopedValues
        string? culture;
        var cultureKey = new PropertyBagKey<string>("Culture");
        propertyBag.Set(cultureKey, "en-US");

        using (propertyBag.Scope(cultureKey, "fr-FR"))
        {
            // Within this scope, Culture is "fr-FR"
            propertyBag.TryGetValue(cultureKey, out culture); // "fr-FR"
        }

        // After disposal, Culture is restored to "en-US"
        propertyBag.TryGetValue(cultureKey, out var restored); // "en-US"
        // end-snippet

        Assert.Equal("fr-FR", culture);
        Assert.Equal("en-US", restored);
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public async Task ReadmeRegisterServicesSample_Runs()
    {
        var services = new ServiceCollection();

        // begin-snippet: ReadmeRegisterServices
        services.AddPropertyBag();
        // end-snippet

        await using var provider = services.BuildServiceProvider();
        var resolvedFactory = provider.GetRequiredService<IPropertyBagFactory>();
        Assert.IsType<DefaultPropertyBagFactory>(resolvedFactory);
    }

    [Fact]
    public void ReadmeInjectFactorySample_Runs()
    {
        var factory = new DefaultPropertyBagFactory();
        var service = new MyService(factory);

        var context = service.CreateContext();
        Assert.IsType<DefaultPropertyBag>(context);
    }

    #endregion

    #region Convenience Extensions Tests

    [Fact]
    public void ReadmeSetInferredSample_Runs()
    {
        IPropertyBag propertyBag = new DefaultPropertyBag();

        // begin-snippet: ReadmeSetInferred
        const string connectionString = "Server=localhost;Database=test";
        var timeout = TimeSpan.FromSeconds(30);

        // Keys are inferred as "connectionString" and "timeout"
        propertyBag.Set(connectionString).Set(timeout);
        // end-snippet

        var connectionStringKey = new PropertyBagKey<string>("connectionString");
        bool foundConnectionString = propertyBag.TryGetValue(
            connectionStringKey,
            out var storedConnectionString
        );
        Assert.True(foundConnectionString);
        Assert.Equal(connectionString, storedConnectionString);

        var timeoutKey = new PropertyBagKey<TimeSpan>("timeout");
        bool foundTimeout = propertyBag.TryGetValue(timeoutKey, out var storedTimeout);
        Assert.True(foundTimeout);
        Assert.Equal(timeout, storedTimeout);
    }

    [Fact]
    public void ReadmeReadInferredSample_Runs()
    {
        IPropertyBag propertyBag = new DefaultPropertyBag();
        var seededKey = new PropertyBagKey<string>("connectionString");
        propertyBag.Set(seededKey, "Server=localhost;Database=test");

        // begin-snippet: ReadmeReadInferred
        // Retrieve with inferred key name
        // Declare the variable first: an inline declaration (out string? x) names the key "string? x"
        string? connectionString;
        if (propertyBag.TryGet(out connectionString)) // Key inferred as "connectionString"
        {
            Console.WriteLine(connectionString);
        }
        // end-snippet

        Assert.Equal("Server=localhost;Database=test", connectionString);
        bool foundAgain = propertyBag.TryGet(out connectionString);
        Assert.True(foundAgain);
    }

    #endregion

    #region Nullable Values Tests

    [Fact]
    public void ReadmeNullableValuesSample_Runs()
    {
        IPropertyBag propertyBag = new DefaultPropertyBag();

        // begin-snippet: ReadmeNullableValues
        var optionalNameKey = new PropertyBagKey<string?>("OptionalName");

        // Explicitly store null
        propertyBag.Set(optionalNameKey, null);

        // TryGetValue returns true because the key exists (even though value is null)
        if (propertyBag.TryGetValue(optionalNameKey, out var name))
        {
            // name is null here, but the key was found
            Console.WriteLine(name ?? "(not set)");
        }

        // This is different from a missing key (name the type argument: a bare discard infers string)
        var missingKey = new PropertyBagKey<string?>("Missing");
        if (!propertyBag.TryGetValue<string?>(missingKey, out _))
        {
            // Key does not exist
        }
        // end-snippet

        bool foundOptional = propertyBag.TryGetValue(optionalNameKey, out var stored);
        Assert.True(foundOptional);
        Assert.Null(stored);
        bool foundMissing = propertyBag.TryGetValue<string?>(missingKey, out _);
        Assert.False(foundMissing);
    }

    #endregion
}
