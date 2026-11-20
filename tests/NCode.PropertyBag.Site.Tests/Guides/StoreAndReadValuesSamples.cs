// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/store-and-read-values.mdx. The site's build includes each begin-snippet region in
// the page by its key (ADR-1006).
public class StoreAndReadValuesSamples
{
    [Fact]
    public void SiteStoreAndReadValuesSetSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesSet
        IPropertyBag bag = new PropertyBagDictionary();
        PropertyBagKey<string> userKey = new("User");
        PropertyBagKey<int> retriesKey = new("Retries");

        bag.SetValue(userKey, "alice").SetValue(retriesKey, 3);

        // Replaces the earlier value
        bag.SetValue(userKey, "bob");
        // end-snippet

        bool hasUser = bag.TryGetValue(userKey, out string? user);
        bool hasRetries = bag.TryGetValue(retriesKey, out int retries);

        Assert.True(hasUser);
        Assert.Equal("bob", user);
        Assert.True(hasRetries);
        Assert.Equal(3, retries);
    }

    [Fact]
    public void SiteStoreAndReadValuesMissingSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesMissing
        IPropertyBag bag = new PropertyBagDictionary();
        PropertyBagKey<int> countKey = new("Count");

        // false, and count is 0
        bool found = bag.TryGetValue(countKey, out int count);
        // end-snippet

        Assert.False(found);
        Assert.Equal(0, count);
    }

    [Fact]
    public void SiteStoreAndReadValuesFallbackSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesFallback
        IPropertyBag bag = new PropertyBagDictionary();
        PropertyBagKey<int> retriesKey = new("Retries");
        PropertyBagKey<string> userKey = new("User");
        bag.SetValue(userKey, "alice");

        // The stored value, or the fallback when the key is missing
        int retries = bag.GetValueOrDefault(retriesKey, 3);

        // Tests for the key without reading its value
        bool hasUser = bag.ContainsKey(userKey);

        // Returns the stored value, and throws when the key is missing
        string? user = bag.GetValue(userKey);
        bool threw = false;
        try
        {
            bag.GetValue(retriesKey);
        }
        catch (KeyNotFoundException)
        {
            threw = true;
        }
        // end-snippet

        Assert.Equal(3, retries);
        Assert.True(hasUser);
        Assert.Equal("alice", user);
        Assert.True(threw);
    }

    [Fact]
    public void SiteStoreAndReadValuesRemoveSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesRemove
        IPropertyBag bag = new PropertyBagDictionary();
        PropertyBagKey<string> key = new("Token");
        bag.SetValue(key, "abc");

        // The typed key converts to the non-generic PropertyBagKey
        bag.Remove(key);

        // Removing a key the bag does not hold does nothing
        bag.Remove(key);
        // end-snippet

        bool hasToken = bag.TryGetValue(key, out string? token);

        Assert.False(hasToken);
        Assert.Null(token);
    }

    [Fact]
    public void SiteStoreAndReadValuesAddOnceSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesAddOnce
        IPropertyBag bag = new PropertyBagDictionary();
        PropertyBagKey<string> userKey = new("User");
        PropertyBagKey<int> retriesKey = new("Retries");

        // true: the key was missing, so the value is stored
        bool added = bag.TryAdd(userKey, "alice");

        // false: the key exists, so the stored value stays
        bool addedAgain = bag.TryAdd(userKey, "bob");

        // Returns the stored value, or stores and returns the one the factory makes
        int retries = bag.GetOrAdd(retriesKey, _ => 3);

        // true, and removed is "alice"
        bool wasRemoved = bag.TryRemove(userKey, out string? removed);
        // end-snippet

        Assert.True(added);
        Assert.False(addedAgain);
        Assert.Equal(3, retries);
        Assert.True(wasRemoved);
        Assert.Equal("alice", removed);
    }

    [Fact]
    public void SiteStoreAndReadValuesNullSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesNull
        IPropertyBag bag = new PropertyBagDictionary();
        PropertyBagKey<string?> nicknameKey = new("Nickname");
        PropertyBagKey<string?> missingKey = new("Missing");
        bag.SetValue(nicknameKey, null);

        // true, and nickname is null: null is a stored value
        bool hasNickname = bag.TryGetValue(nicknameKey, out var nickname);

        // false: the key was never set. Name the type argument,
        // because a bare out _ infers string and does not compile
        bool hasMissing = bag.TryGetValue<string?>(missingKey, out _);
        // end-snippet

        Assert.True(hasNickname);
        Assert.Null(nickname);
        Assert.False(hasMissing);
    }
}
