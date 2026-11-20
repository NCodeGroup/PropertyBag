// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Guides;

// The samples of docs/site/guides/define-keys.mdx. The site's build includes each begin-snippet
// region in the page by its key (ADR-1004).
public class DefineKeysSamples
{
    // begin-snippet: SiteDefineKeysDeclare
    private static class RequestKeys
    {
        public static readonly PropertyBagKey<string> RequestId = new("RequestId");
        public static readonly PropertyBagKey<int> Attempt = new("Attempt");
    }

    // end-snippet

    [Fact]
    public void SiteDefineKeysDeclareSample_Runs()
    {
        IPropertyBag bag = new DefaultPropertyBag();

        bag.Set(RequestKeys.RequestId, "abc-123").Set(RequestKeys.Attempt, 2);

        bool foundRequestId = bag.TryGetValue(
            RequestKeys.RequestId,
            out string? requestId
        );
        bool foundAttempt = bag.TryGetValue(RequestKeys.Attempt, out int attempt);

        Assert.True(foundRequestId);
        Assert.Equal("abc-123", requestId);
        Assert.True(foundAttempt);
        Assert.Equal(2, attempt);
    }

    [Fact]
    public void SiteDefineKeysSameNameSample_Runs()
    {
        // begin-snippet: SiteDefineKeysSameName
        IPropertyBag bag = new DefaultPropertyBag();

        // Two keys: the same name with another type
        PropertyBagKey<string> textKey = new("Id");
        PropertyBagKey<int> numberKey = new("Id");
        bag.Set(textKey, "order-17").Set(numberKey, 17);
        // end-snippet

        bool foundText = bag.TryGetValue(textKey, out string? text);
        bool foundNumber = bag.TryGetValue(numberKey, out int number);

        Assert.True(foundText);
        Assert.Equal("order-17", text);
        Assert.True(foundNumber);
        Assert.Equal(17, number);
    }

    [Fact]
    public void SiteDefineKeysNewInstanceSample_Runs()
    {
        // begin-snippet: SiteDefineKeysNewInstance
        IPropertyBag bag = new DefaultPropertyBag();
        bag.Set(new PropertyBagKey<string>("Region"), "eu-west");

        // Equal keys, not the same instance, find the value
        PropertyBagKey<string> sameKey = new("Region");
        bool found = bag.TryGetValue(sameKey, out string? region);
        // end-snippet

        Assert.True(found);
        Assert.Equal("eu-west", region);
    }

    [Fact]
    public void SiteDefineKeysPitfallsSample_Runs()
    {
        // begin-snippet: SiteDefineKeysPitfalls
        PropertyBagKey<string> unnamed = default; // Name is null

        IPropertyBag bag = new DefaultPropertyBag();
        bag.Set(new PropertyBagKey<string>("UserName"), "alice");

        // false: names are case-sensitive
        PropertyBagKey<string> lowerCase = new("username");
        bool found = bag.TryGetValue(lowerCase, out string? user);
        // end-snippet

        Assert.Null(unnamed.Name);
        Assert.False(found);
        Assert.Null(user);
    }
}
