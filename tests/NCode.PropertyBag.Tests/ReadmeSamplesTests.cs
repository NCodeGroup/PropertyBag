// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.DependencyInjection;
using ReadmeSamples;

namespace NCode.PropertyBag.Tests;

// `dotnet mdsnippets` copies each begin-snippet block into README.md and the package READMEs (ADR-1003).
// PropertyBagFactory.Create reads a process-wide factory, so these tests run with the other static-factory tests.
[Collection(StaticFactoryCollection.Name)]
public class ReadmeSamplesTests
{
    #region Quick Start Tests

    [Fact]
    public void DefineKeysSample_Runs()
    {
        Assert.Equal("UserName", UserKeys.UserName.Name);
        Assert.Equal(typeof(int), UserKeys.UserAge.Type);
    }

    [Fact]
    public void BasicUsageSample_Runs()
    {
        // begin-snippet: BasicUsage
        var propertyBag = PropertyBagFactory.Create();

        propertyBag.SetValue(UserKeys.UserName, "Alice").SetValue(UserKeys.UserAge, 30);

        if (propertyBag.TryGetValue(UserKeys.UserName, out var userName))
        {
            Console.WriteLine($"User: {userName}"); // User: Alice
        }
        // end-snippet

        Assert.Equal("Alice", userName);
        Assert.True(propertyBag.TryGetValue(UserKeys.UserAge, out var userAge));
        Assert.Equal(30, userAge);
    }

    #endregion

    #region Usage Tests

    [Fact]
    public void ScopedValuesSample_Runs()
    {
        var propertyBag = PropertyBagFactory.Create();

        // begin-snippet: ScopedValues
        var cultureKey = new PropertyBagKey<string>("Culture");
        propertyBag.SetValue(cultureKey, "en-US");

        string? inScope;
        using (propertyBag.Scope(cultureKey, "fr-FR"))
        {
            propertyBag.TryGetValue(cultureKey, out inScope); // "fr-FR"
        }

        propertyBag.TryGetValue(cultureKey, out var afterScope); // "en-US" again
        // end-snippet

        Assert.Equal("fr-FR", inScope);
        Assert.Equal("en-US", afterScope);
    }

    [Fact]
    public void NullValuesSample_Runs()
    {
        var propertyBag = PropertyBagFactory.Create();

        // begin-snippet: NullValues
        var middleNameKey = new PropertyBagKey<string?>("MiddleName");
        propertyBag.SetValue(middleNameKey, null);

        // True, with a null value: the key holds null.
        var found = propertyBag.TryGetValue(middleNameKey, out var middleName);

        // False: a missing key differs from a stored null.
        var nicknameKey = new PropertyBagKey<string?>("Nickname");
        var missing = propertyBag.TryGetValue(nicknameKey, out var nickname);
        // end-snippet

        Assert.True(found);
        Assert.Null(middleName);
        Assert.False(missing);
        Assert.Null(nickname);
    }

    [Fact]
    public void InferredKeysSample_Runs()
    {
        var propertyBag = PropertyBagFactory.Create();
        ConnectionSettings.Save(propertyBag, "Server=localhost", TimeSpan.FromSeconds(30));

        Assert.Equal("Server=localhost", ConnectionSettings.LoadConnectionString(propertyBag));
        Assert.True(
            propertyBag.TryGetValue(new PropertyBagKey<TimeSpan>("timeout"), out var timeout)
        );
        Assert.Equal(TimeSpan.FromSeconds(30), timeout);
    }

    [Fact]
    public void CloneSample_Runs()
    {
        var propertyBag = PropertyBagFactory.Create().SetValue(UserKeys.UserName, "Alice");

        // begin-snippet: Clone
        var copy = propertyBag.Clone();
        copy.SetValue(UserKeys.UserName, "Bob");

        propertyBag.TryGetValue(UserKeys.UserName, out var original); // still "Alice"
        // end-snippet

        Assert.Equal("Alice", original);
        Assert.True(copy.TryGetValue(UserKeys.UserName, out var copied));
        Assert.Equal("Bob", copied);
    }

    [Fact]
    public void EnumerateEntriesSample_Runs()
    {
        var lines = new List<string>();

        // begin-snippet: EnumerateEntries
        var propertyBag = new PropertyBagDictionary();
        propertyBag.SetValue(UserKeys.UserName, "Alice").SetValue(UserKeys.UserAge, 30);

        IReadOnlyDictionary<PropertyBagKey, object?> entries = propertyBag;
        foreach (var (key, value) in entries)
        {
            lines.Add($"{key.Name} ({key.Type.Name}) = {value}");
        }
        // end-snippet

        Assert.Equal(["UserName (String) = Alice", "UserAge (Int32) = 30"], lines);
    }

    #endregion

    #region Dependency Injection Tests

    [Fact]
    public async Task RegisterServicesSample_Runs()
    {
        // begin-snippet: RegisterServices
        var services = new ServiceCollection();
        services.AddPropertyBag();
        services.AddSingleton<RequestContextFactory>();
        // end-snippet

        await using var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<RequestContextFactory>().CreateContext("Alice");

        Assert.True(context.TryGetValue(UserKeys.UserName, out var userName));
        Assert.Equal("Alice", userName);
    }

    #endregion
}
