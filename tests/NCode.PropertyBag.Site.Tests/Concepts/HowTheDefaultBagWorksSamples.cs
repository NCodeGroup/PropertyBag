// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Concepts;

// The samples of docs/site/concepts/how-the-default-bag-works.mdx. The site's build includes each begin-snippet
// region in the page by its key (ADR-1006).
public class HowTheDefaultBagWorksSamples
{
    [Fact]
    public void SiteHowTheDefaultBagWorksEmptySample_Runs()
    {
        // begin-snippet: SiteHowTheDefaultBagWorksEmpty
        var bag = new DefaultPropertyBag();

        bool found = bag.TryGetValue(new PropertyBagKey<int>("Count"), out _); // false

        IPropertyBag copy = bag.Clone(); // an empty bag
        // end-snippet

        Assert.False(found);
        var entries = (IReadOnlyCollection<KeyValuePair<PropertyBagKey, object?>>)copy;
        Assert.Empty(entries);
    }

    [Fact]
    public void SiteHowTheDefaultBagWorksLockSample_Runs()
    {
        // begin-snippet: SiteHowTheDefaultBagWorksLock
        var bag = new DefaultPropertyBag();
        var gate = new Lock();

        // The bag holds no lock, so concurrent writers share one
        Parallel.For(0, 100, i =>
        {
            lock (gate)
            {
                bag.Set(new PropertyBagKey<int>($"item{i}"), i);
            }
        });
        // end-snippet

        var entries = (IReadOnlyCollection<KeyValuePair<PropertyBagKey, object?>>)bag;
        Assert.Equal(100, entries.Count);
    }

    [Fact]
    public void SiteHowTheDefaultBagWorksDirectScopeSample_Runs()
    {
        // begin-snippet: SiteHowTheDefaultBagWorksDirectScope
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string> key = new("Culture");
        bag.Set(key, "A");

        var scope = new DefaultPropertyBagScope(bag, key);

        // Removes the key: this constructor has no captured value to restore
        scope.Dispose();
        // end-snippet

        bool found = bag.TryGetValue(key, out string? value);

        Assert.False(found);
        Assert.Null(value);
    }
}
