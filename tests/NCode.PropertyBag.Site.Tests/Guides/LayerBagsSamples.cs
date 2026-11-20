// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/layer-bags.mdx. The site's build includes each begin-snippet
// region in the page by its key (ADR-1006).
public class LayerBagsSamples
{
    [Fact]
    public void SiteLayerBagsCreateSample_Runs()
    {
        // begin-snippet: SiteLayerBagsCreate
        PropertyBagKey<string> regionKey = new("Region");
        PropertyBagKey<int> retriesKey = new("Retries");

        IPropertyBag parent = new PropertyBagDictionary();
        parent.SetValue(regionKey, "eu-west").SetValue(retriesKey, 3);

        IPropertyBag child = parent.CreateChild();

        // A read falls through to the parent
        bool foundRegion = child.TryGetValue(regionKey, out string? region);

        // A write stays in the child: the parent still holds 3
        child.SetValue(retriesKey, 5);
        bool foundRetries = child.TryGetValue(retriesKey, out int retries);
        bool parentFound = parent.TryGetValue(retriesKey, out int parentRetries);
        // end-snippet

        Assert.True(foundRegion);
        Assert.Equal("eu-west", region);
        Assert.True(foundRetries);
        Assert.Equal(5, retries);
        Assert.True(parentFound);
        Assert.Equal(3, parentRetries);
    }

    [Fact]
    public void SiteLayerBagsRemoveSample_Runs()
    {
        // begin-snippet: SiteLayerBagsRemove
        PropertyBagKey<string> regionKey = new("Region");

        IPropertyBag parent = new PropertyBagDictionary();
        parent.SetValue(regionKey, "eu-west");

        IPropertyBag child = parent.CreateChild();

        // The child hides the parent's value and leaves the parent alone
        child.Remove(regionKey);
        bool hidden = child.TryGetValue(regionKey, out string? _);
        bool parentHolds = parent.TryGetValue(regionKey, out string? parentRegion);

        // Setting the key again shows a value in the child
        child.SetValue(regionKey, "us-east");
        bool shown = child.TryGetValue(regionKey, out string? region);
        // end-snippet

        Assert.False(hidden);
        Assert.True(parentHolds);
        Assert.Equal("eu-west", parentRegion);
        Assert.True(shown);
        Assert.Equal("us-east", region);
    }

    [Fact]
    public void SiteLayerBagsScopeSample_Runs()
    {
        // begin-snippet: SiteLayerBagsScope
        PropertyBagKey<string> cultureKey = new("Culture");

        IPropertyBag parent = new PropertyBagDictionary();
        parent.SetValue(cultureKey, "en-US");

        IPropertyBag child = parent.CreateChild();

        string? inside;
        using (child.Scope(cultureKey, "fr-FR"))
        {
            child.TryGetValue(cultureKey, out inside);
        }

        // The scope restores the child's own state, a fall-through to the parent
        bool found = child.TryGetValue(cultureKey, out string? after);
        bool parentFound = parent.TryGetValue(cultureKey, out string? parentCulture);
        // end-snippet

        Assert.Equal("fr-FR", inside);
        Assert.True(found);
        Assert.Equal("en-US", after);
        Assert.True(parentFound);
        Assert.Equal("en-US", parentCulture);
    }

    [Fact]
    public void SiteLayerBagsCloneSample_Runs()
    {
        // begin-snippet: SiteLayerBagsClone
        PropertyBagKey<string> regionKey = new("Region");
        PropertyBagKey<string> nameKey = new("Name");

        IPropertyBag parent = new PropertyBagDictionary();
        parent.SetValue(regionKey, "eu-west");

        IPropertyBag child = parent.CreateChild();
        child.SetValue(nameKey, "alice");

        // The clone copies the child's own entries and shares the parent
        IPropertyBag copy = child.Clone();
        copy.SetValue(nameKey, "bob");

        // A later parent change shows in the child and in the clone
        parent.SetValue(regionKey, "us-east");
        bool childFound = child.TryGetValue(regionKey, out string? childRegion);
        bool copyFound = copy.TryGetValue(regionKey, out string? copyRegion);
        bool childName = child.TryGetValue(nameKey, out string? name);
        // end-snippet

        Assert.True(childFound);
        Assert.Equal("us-east", childRegion);
        Assert.True(copyFound);
        Assert.Equal("us-east", copyRegion);
        Assert.True(childName);
        Assert.Equal("alice", name);
    }
}
