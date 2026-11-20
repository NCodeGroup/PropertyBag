// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/scope-a-value.mdx. The site's build includes each begin-snippet region in the page by its key (ADR-1004).
public class ScopeAValueSamples
{
    [Fact]
    public void SiteScopeAValueOverrideSample_Runs()
    {
        // begin-snippet: SiteScopeAValueOverride
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string> cultureKey = new("Culture");
        bag.Set(cultureKey, "en-US");

        string? inside;
        using (bag.Scope(cultureKey, "fr-FR"))
        {
            bag.TryGetValue(cultureKey, out inside); // "fr-FR"
        }

        bag.TryGetValue(cultureKey, out string? after); // "en-US"
        // end-snippet

        Assert.Equal("fr-FR", inside);
        Assert.Equal("en-US", after);
    }

    [Fact]
    public void SiteScopeAValueNoPreviousSample_Runs()
    {
        // begin-snippet: SiteScopeAValueNoPrevious
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string> traceKey = new("TraceId");

        using (bag.Scope(traceKey, "t-1"))
        {
            // The key holds "t-1" until the scope is disposed
        }

        // The key held nothing, so disposing the scope removes it
        bool found = bag.TryGetValue(traceKey, out string? trace);
        // end-snippet

        Assert.False(found);
        Assert.Null(trace);
    }

    [Fact]
    public void SiteScopeAValueNestedSample_Runs()
    {
        // begin-snippet: SiteScopeAValueNested
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string> levelKey = new("Level");
        bag.Set(levelKey, "original");

        string? atOuter;
        string? atInner;
        using (bag.Scope(levelKey, "outer"))
        {
            using (bag.Scope(levelKey, "inner"))
            {
                bag.TryGetValue(levelKey, out atInner); // "inner"
            }

            bag.TryGetValue(levelKey, out atOuter); // "outer"
        }

        bag.TryGetValue(levelKey, out string? atEnd); // "original"
        // end-snippet

        Assert.Equal("inner", atInner);
        Assert.Equal("outer", atOuter);
        Assert.Equal("original", atEnd);
    }

    [Fact]
    public void SiteScopeAValueOutOfOrderSample_Runs()
    {
        // begin-snippet: SiteScopeAValueOutOfOrder
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string> key = new("Mode");
        bag.Set(key, "A");

        var first = bag.Scope(key, "B");
        var second = bag.Scope(key, "C");
        first.Dispose();
        second.Dispose();

        // "B": the last scope disposed restores what it captured
        bag.TryGetValue(key, out string? mode);
        // end-snippet

        Assert.Equal("B", mode);
    }

    [Fact]
    public void SiteScopeAValueSetInsideSample_Runs()
    {
        // begin-snippet: SiteScopeAValueSetInside
        IPropertyBag bag = new DefaultPropertyBag();
        PropertyBagKey<string> key = new("Mode");
        bag.Set(key, "A");

        using (bag.Scope(key, "B"))
        {
            bag.Set(key, "changed");
        }

        // The restore replaces the value set inside the scope
        bag.TryGetValue(key, out string? mode); // "A"
        // end-snippet

        Assert.Equal("A", mode);
    }
}
