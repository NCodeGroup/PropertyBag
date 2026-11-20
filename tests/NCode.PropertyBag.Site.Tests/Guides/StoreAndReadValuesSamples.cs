// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/store-and-read-values.mdx. The site's build includes each begin-snippet region in
// the page by its key (ADR-1004).
public class StoreAndReadValuesSamples
{
    [Fact]
    public void SiteStoreAndReadValuesSetSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesSet
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string> userKey = new("User");
        PropertyBagKey<int> retriesKey = new("Retries");

        bag.Set(userKey, "alice").Set(retriesKey, 3);

        // Replaces the earlier value
        bag.Set(userKey, "bob");
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
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<int> countKey = new("Count");

        // false, and count is 0
        bool found = bag.TryGetValue(countKey, out int count);
        // end-snippet

        Assert.False(found);
        Assert.Equal(0, count);
    }

    [Fact]
    public void SiteStoreAndReadValuesRemoveSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesRemove
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string> key = new("Token");
        bag.Set(key, "abc");

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
    public void SiteStoreAndReadValuesNullSample_Runs()
    {
        // begin-snippet: SiteStoreAndReadValuesNull
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string?> nicknameKey = new("Nickname");
        PropertyBagKey<string?> missingKey = new("Missing");
        bag.Set(nicknameKey, null);

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
