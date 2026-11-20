// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Start;

// The samples of docs/site/start/quick-start.mdx. The site's build includes each begin-snippet region in the page by
// its key (ADR-1006).
public class QuickStartSamples
{
    // begin-snippet: SiteQuickStartKeys
    private static readonly PropertyBagKey<string> UserNameKey = new("UserName");
    private static readonly PropertyBagKey<int> RetryCountKey = new("RetryCount");

    // end-snippet

    [Fact]
    public void SiteQuickStartKeysSample_Runs()
    {
        Assert.Equal("UserName", UserNameKey.Name);
        Assert.Equal("RetryCount", RetryCountKey.Name);
        Assert.Equal(typeof(int), RetryCountKey.Type);
    }

    [Fact]
    public void SiteQuickStartStoreSample_Runs()
    {
        // begin-snippet: SiteQuickStartStore
        IPropertyBag bag = PropertyBagFactory.Create();

        bag.SetValue(UserNameKey, "alice").SetValue(RetryCountKey, 3);

        bool found = bag.TryGetValue(RetryCountKey, out int retries); // true, 3

        PropertyBagKey<int> timeoutKey = new("Timeout");
        bool missing = bag.TryGetValue(timeoutKey, out int timeout); // false, 0
        // end-snippet

        Assert.True(found);
        Assert.Equal(3, retries);
        Assert.False(missing);
        Assert.Equal(0, timeout);
        Assert.Equal("Timeout", timeoutKey.Name);
    }

    [Fact]
    public void SiteQuickStartScopeSample_Runs()
    {
        IPropertyBag bag = PropertyBagFactory.Create();
        bag.SetValue(UserNameKey, "alice");

        // begin-snippet: SiteQuickStartScope
        string? inside;
        using (bag.Scope(UserNameKey, "bob"))
        {
            bag.TryGetValue(UserNameKey, out inside); // "bob"
        }

        bag.TryGetValue(UserNameKey, out string? after); // "alice"
        // end-snippet

        Assert.Equal("bob", inside);
        Assert.Equal("alice", after);
    }

    [Fact]
    public void SiteQuickStartRemoveSample_Runs()
    {
        IPropertyBag bag = PropertyBagFactory.Create();
        bag.SetValue(UserNameKey, "alice");

        // begin-snippet: SiteQuickStartRemove
        bag.Remove(UserNameKey);

        bool stillThere = bag.TryGetValue(UserNameKey, out _); // false
        // end-snippet

        Assert.False(stillThere);
    }
}
