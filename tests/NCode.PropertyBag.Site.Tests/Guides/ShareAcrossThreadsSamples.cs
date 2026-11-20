// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/share-across-threads.mdx. The site's build includes each
// sample in the page by its key (ADR-1006).
public class ShareAcrossThreadsSamples
{
    [Fact]
    public void SiteShareAcrossThreadsSetSample_Runs()
    {
        // begin-snippet: SiteShareAcrossThreadsSet
        ConcurrentPropertyBagDictionary bag = new();

        // Every thread writes its own key, and the lock keeps the bag consistent
        Parallel.For(0, 100, index =>
        {
            PropertyBagKey<int> key = new($"Worker{index}");
            bag.SetValue(key, index);
        });

        int count = ((IReadOnlyDictionary<PropertyBagKey, object?>)bag).Count;
        // end-snippet

        Assert.Equal(100, count);
    }

    [Fact]
    public void SiteShareAcrossThreadsGetOrAddSample_Runs()
    {
        // begin-snippet: SiteShareAcrossThreadsGetOrAdd
        PropertyBagKey<object> cacheKey = new("Cache");
        ConcurrentPropertyBagDictionary bag = new();

        // The factory can run more than once under contention, but the bag stores one result
        // and every caller gets the stored value
        object?[] results = new object?[50];
        Parallel.For(0, results.Length, index =>
        {
            results[index] = bag.GetOrAdd(cacheKey, _ => new object());
        });

        int distinct = results.Distinct().Count();
        // end-snippet

        Assert.Equal(1, distinct);
    }

    [Fact]
    public void SiteShareAcrossThreadsEnumerateSample_Runs()
    {
        // begin-snippet: SiteShareAcrossThreadsEnumerate
        PropertyBagKey<string> regionKey = new("Region");
        PropertyBagKey<int> retriesKey = new("Retries");

        ConcurrentPropertyBagDictionary bag = new();
        bag.SetValue(regionKey, "eu-west").SetValue(retriesKey, 3);

        // Enumeration walks a snapshot taken under the lock, so a write during the loop never
        // throws and does not appear in the loop
        IReadOnlyDictionary<PropertyBagKey, object?> entries = bag;
        List<string> names = [];
        foreach (KeyValuePair<PropertyBagKey, object?> entry in entries)
        {
            bag.SetValue(new PropertyBagKey<int>($"Extra{names.Count}"), 1);
            names.Add(entry.Key.Name);
        }
        // end-snippet

        Assert.Equal(2, names.Count);
        Assert.Contains("Region", names);
        Assert.Contains("Retries", names);
    }
}
