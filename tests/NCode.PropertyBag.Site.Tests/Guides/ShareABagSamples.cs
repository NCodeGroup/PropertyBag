// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/share-a-bag.mdx. The site's build includes each begin-snippet
// region in the page by its key (ADR-1006).
public class ShareABagSamples
{
    [Fact]
    public void SiteShareABagReadOnlySample_Runs()
    {
        // begin-snippet: SiteShareABagReadOnly
        static string? ReadRegion(IReadOnlyPropertyBag settings)
        {
            PropertyBagKey<string> regionKey = new("Region");
            return settings.TryGetValue(regionKey, out string? value) ? value : null;
        }

        IPropertyBag bag = new DefaultPropertyBag();
        bag.Set(new PropertyBagKey<string>("Region"), "eu-west");

        string? current = ReadRegion(bag);
        // end-snippet

        Assert.Equal("eu-west", current);
    }

    [Fact]
    public void SiteShareABagCloneSample_Runs()
    {
        // begin-snippet: SiteShareABagClone
        PropertyBagKey<string> nameKey = new("Name");
        PropertyBagKey<string> roleKey = new("Role");

        IPropertyBag bag = new DefaultPropertyBag();
        bag.Set(nameKey, "alice").Set(roleKey, "admin");

        IPropertyBag copy = bag.Clone();

        // The change stays in the copy: the original still holds "alice"
        copy.Set(nameKey, "bob");

        // The removal stays in the original: the copy still holds "admin"
        bag.Remove(roleKey);
        // end-snippet

        bool bagHasName = bag.TryGetValue(nameKey, out string? bagName);
        bool bagHasRole = bag.TryGetValue(roleKey, out _);
        bool copyHasName = copy.TryGetValue(nameKey, out string? copyName);
        bool copyHasRole = copy.TryGetValue(roleKey, out string? copyRole);

        Assert.True(bagHasName);
        Assert.Equal("alice", bagName);
        Assert.False(bagHasRole);
        Assert.True(copyHasName);
        Assert.Equal("bob", copyName);
        Assert.True(copyHasRole);
        Assert.Equal("admin", copyRole);
    }

    [Fact]
    public void SiteShareABagCloneValuesSample_Runs()
    {
        // begin-snippet: SiteShareABagCloneValues
        PropertyBagKey<int[]> arrayKey = new("Array");
        PropertyBagKey<List<int>> listKey = new("List");

        int[] numbers = [1, 2, 3];
        List<int> items = [1, 2, 3];

        IPropertyBag bag = new DefaultPropertyBag();
        bag.Set(arrayKey, numbers).Set(listKey, items);

        IPropertyBag copy = bag.Clone();

        // A different array: int[] implements ICloneable
        bool foundArray = copy.TryGetValue(arrayKey, out int[]? copiedArray);

        // The same list: List<int> does not
        bool foundList = copy.TryGetValue(listKey, out List<int>? copiedList);
        // end-snippet

        Assert.True(foundArray);
        Assert.NotSame(numbers, copiedArray);
        Assert.Equal(numbers, copiedArray);
        Assert.True(foundList);
        Assert.Same(items, copiedList);
    }

    [Fact]
    public void SiteShareABagEnumerateSample_Runs()
    {
        // begin-snippet: SiteShareABagEnumerate
        IPropertyBag bag = new DefaultPropertyBag();
        bag.Set(new PropertyBagKey<string>("Region"), "eu-west");
        bag.Set(new PropertyBagKey<int>("Retries"), 3);

        var entries = (IReadOnlyDictionary<PropertyBagKey, object?>)bag;

        List<string> lines = [];
        foreach (var entry in entries)
        {
            lines.Add($"{entry.Key.Name}: {entry.Value}");
        }

        int count = entries.Count;

        // null, not KeyNotFoundException
        PropertyBagKey<int> missingKey = new("Missing");
        object? missing = entries[missingKey];
        // end-snippet

        Assert.Equal(2, count);
        Assert.Equal(2, lines.Count);
        Assert.Contains("Region: eu-west", lines);
        Assert.Contains("Retries: 3", lines);
        Assert.Null(missing);
    }
}
