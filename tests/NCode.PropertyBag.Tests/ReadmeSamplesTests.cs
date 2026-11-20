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

        var foundAge = propertyBag.TryGetValue(UserKeys.UserAge, out var userAge);

        Assert.Equal("Alice", userName);
        Assert.True(foundAge);
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
        var timeout = TimeSpan.FromSeconds(30);
        ConnectionSettings.Save(propertyBag, "Server=localhost", timeout);

        var connectionString = ConnectionSettings.LoadConnectionString(propertyBag);
        var timeoutKey = new PropertyBagKey<TimeSpan>("timeout");
        var found = propertyBag.TryGetValue(timeoutKey, out var storedTimeout);

        Assert.Equal("Server=localhost", connectionString);
        Assert.True(found);
        Assert.Equal(timeout, storedTimeout);
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

        var foundCopy = copy.TryGetValue(UserKeys.UserName, out var copied);

        Assert.Equal("Alice", original);
        Assert.True(foundCopy);
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

    [Fact]
    public void ReadHelpersSample_Runs()
    {
        var propertyBag = PropertyBagFactory.Create();

        // begin-snippet: ReadHelpers
        var age = propertyBag.GetValueOrDefault(UserKeys.UserAge, 18); // 18: no value yet
        var name = propertyBag.GetOrAdd(UserKeys.UserName, _ => "Guest"); // stores "Guest"
        var added = propertyBag.TryAdd(UserKeys.UserAge, 30); // true: the key was free
        var storedAge = propertyBag.GetValue(UserKeys.UserAge); // 30
        // end-snippet

        Assert.Equal(18, age);
        Assert.Equal("Guest", name);
        Assert.True(added);
        Assert.Equal(30, storedAge);
    }

    [Fact]
    public void ChildBagSample_Runs()
    {
        var propertyBag = PropertyBagFactory.Create().SetValue(UserKeys.UserName, "Alice");

        // begin-snippet: ChildBag
        var request = propertyBag.CreateChild();
        request.SetValue(UserKeys.UserAge, 30);

        request.TryGetValue(UserKeys.UserName, out var inherited); // "Alice", read from the parent
        var parentHasAge = propertyBag.ContainsKey(UserKeys.UserAge); // false: the parent is untouched
        // end-snippet

        Assert.Equal("Alice", inherited);
        Assert.False(parentHasAge);
    }

    [Fact]
    public void ReadOnlyViewSample_Runs()
    {
        var propertyBag = PropertyBagFactory.Create().SetValue(UserKeys.UserName, "Alice");

        // begin-snippet: ReadOnlyView
        IReadOnlyPropertyBag view = propertyBag.AsReadOnly();

        view.TryGetValue(UserKeys.UserName, out var userName); // "Alice"
        // end-snippet

        Assert.Equal("Alice", userName);
        Assert.IsNotAssignableFrom<IPropertyBag>(view);
    }

    [Fact]
    public void ConcurrentBagSample_Runs()
    {
        // begin-snippet: ConcurrentBag
        var shared = new ConcurrentPropertyBagDictionary();

        Parallel.For(0, 100, i => shared.GetOrAdd(UserKeys.UserAge, _ => i));

        var winner = shared.GetValue(UserKeys.UserAge); // one of the values; every thread saw it
        // end-snippet

        Assert.InRange(winner, 0, 99);
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
        var contextFactory = provider.GetRequiredService<RequestContextFactory>();
        var context = contextFactory.CreateContext("Alice");

        var found = context.TryGetValue(UserKeys.UserName, out var userName);

        Assert.True(found);
        Assert.Equal("Alice", userName);
    }

    #endregion
}
