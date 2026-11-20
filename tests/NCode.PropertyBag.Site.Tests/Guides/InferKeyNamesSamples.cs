// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/infer-key-names.mdx. The site's build includes each begin-snippet
// region in the page by its key (ADR-1006).
public class InferKeyNamesSamples
{
    [Fact]
    public void SiteInferKeyNamesSetSample_Runs()
    {
        IPropertyBag bag = new PropertyBagDictionary();

        // begin-snippet: SiteInferKeyNamesSet
        var timeout = TimeSpan.FromSeconds(30);
        const string region = "eu-west";

        // Keys "timeout" (TimeSpan) and "region" (string)
        bag.SetValue(timeout).SetValue(region);
        // end-snippet

        bool foundTimeout = bag.TryGetValue(
            new PropertyBagKey<TimeSpan>("timeout"),
            out TimeSpan storedTimeout
        );
        bool foundRegion = bag.TryGetValue(
            new PropertyBagKey<string>("region"),
            out string? storedRegion
        );

        Assert.True(foundTimeout);
        Assert.Equal(timeout, storedTimeout);
        Assert.True(foundRegion);
        Assert.Equal(region, storedRegion);
    }

    [Fact]
    public void SiteInferKeyNamesTryGetSample_Runs()
    {
        IPropertyBag bag = new PropertyBagDictionary();
        bag.SetValue(new PropertyBagKey<string>("region"), "eu-west");

        // begin-snippet: SiteInferKeyNamesTryGet
        string? region;
        bool found = false;

        // Declared first, so the key is "region"
        if (bag.TryGetValue(out region))
        {
            found = true;
        }
        // end-snippet

        Assert.True(found);
        Assert.Equal("eu-west", region);
    }

    [Fact]
    public void SiteInferKeyNamesExpressionSample_Runs()
    {
        IPropertyBag bag = new PropertyBagDictionary();

        // begin-snippet: SiteInferKeyNamesExpression
        var order = new { Region = "eu-west" };

        // The key is the whole expression: "order.Region"
        bag.SetValue(order.Region);
        // end-snippet

        bool found = bag.TryGetValue(
            new PropertyBagKey<string>("order.Region"),
            out string? stored
        );

        Assert.True(found);
        Assert.Equal("eu-west", stored);
    }

    [Fact]
    public void SiteInferKeyNamesExplicitSample_Runs()
    {
        IPropertyBag bag = new PropertyBagDictionary();

        // begin-snippet: SiteInferKeyNamesExplicit
        const string currentRegion = "eu-west";

        // An explicit name wins
        bag.SetValue(currentRegion, name: "region");
        // end-snippet

        bool found = bag.TryGetValue(
            new PropertyBagKey<string>("region"),
            out string? stored
        );

        Assert.True(found);
        Assert.Equal("eu-west", stored);
    }

    [Fact]
    public void SiteInferKeyNamesInlineOutSample_Runs()
    {
        IPropertyBag bag = new PropertyBagDictionary();
        bag.SetValue(new PropertyBagKey<string>("region"), "eu-west");

        // begin-snippet: SiteInferKeyNamesInlineOut
        // false: the key is "string? region"
        bool found = bag.TryGetValue(out string? region);
        // end-snippet

        Assert.False(found);
        Assert.Null(region);
    }
}
