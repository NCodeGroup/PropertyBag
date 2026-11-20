// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using ReadmeSamples;

namespace NCode.PropertyBag.Tests;

// dotnet mdsnippets copies each begin-snippet block into README.md (ADR-1002). PropertyBagFactory.Create reads the process-wide factory, so the class runs in the static factories collection.
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
        Assert.True(propertyBag.TryGetValue(UserAgeKey, out var userAge));
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
    public void ReadmeRegisterServicesSample_Runs()
    {
        var services = new ServiceCollection();

        // begin-snippet: ReadmeRegisterServices
        services.AddPropertyBag();
        // end-snippet

        using var provider = services.BuildServiceProvider();
        Assert.IsType<DefaultPropertyBagFactory>(
            provider.GetRequiredService<IPropertyBagFactory>()
        );
    }

    [Fact]
    public void ReadmeInjectFactorySample_Runs()
    {
        var service = new MyService(new DefaultPropertyBagFactory());

        Assert.IsType<DefaultPropertyBag>(service.CreateContext());
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

        Assert.True(
            propertyBag.TryGetValue(
                new PropertyBagKey<string>("connectionString"),
                out var storedConnectionString
            )
        );
        Assert.Equal(connectionString, storedConnectionString);
        Assert.True(
            propertyBag.TryGetValue(new PropertyBagKey<TimeSpan>("timeout"), out var storedTimeout)
        );
        Assert.Equal(timeout, storedTimeout);
    }

    [Fact]
    public void ReadmeReadInferredSample_Runs()
    {
        IPropertyBag propertyBag = new DefaultPropertyBag();
        propertyBag.Set(
            new PropertyBagKey<string>("connectionString"),
            "Server=localhost;Database=test"
        );

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
        Assert.True(propertyBag.TryGet(out connectionString));
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

        Assert.True(propertyBag.TryGetValue(optionalNameKey, out var stored));
        Assert.Null(stored);
        Assert.False(propertyBag.TryGetValue<string?>(missingKey, out _));
    }

    #endregion
}
