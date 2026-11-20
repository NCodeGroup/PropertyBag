// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace SiteSamples.Concepts;

// The samples of docs/site/concepts/key-identity.mdx. The site's build includes each begin-snippet
// region in the page by its key (ADR-1004).
public class KeyIdentitySamples
{
    [Fact]
    public void SiteKeyIdentityConversionSample_Runs()
    {
        // begin-snippet: SiteKeyIdentityConversion
        var typed = new PropertyBagKey<string>("UserName");
        PropertyBagKey untyped = typed;

        // Same Type and Name: typeof(string), "UserName"
        bool same = untyped == new PropertyBagKey(typeof(string), "UserName"); // true
        // end-snippet

        Assert.Equal(typeof(string), untyped.Type);
        Assert.Equal("UserName", untyped.Name);
        Assert.True(same);
    }

    [Fact]
    public void SiteKeyIdentityNullableSample_Runs()
    {
        // begin-snippet: SiteKeyIdentityNullable
        IPropertyBag bag = new DefaultPropertyBag();
        bag.Set(new PropertyBagKey<string>("Name"), "alice");
        bag.Set(new PropertyBagKey<int>("Count"), 3);

        PropertyBagKey<string?> nameKey = new("Name");
        PropertyBagKey<int?> countKey = new("Count");

        // true: string? is string at run time
        bool foundName = bag.TryGetValue<string?>(nameKey, out string? name);

        // false: int? is Nullable<int>
        bool foundCount = bag.TryGetValue(countKey, out int? count);
        // end-snippet

        Assert.True(foundName);
        Assert.Equal("alice", name);
        Assert.False(foundCount);
        Assert.Null(count);
    }

    [Fact]
    public void SiteKeyIdentityDeclaredTypeSample_Runs()
    {
        // begin-snippet: SiteKeyIdentityDeclaredType
        IPropertyBag bag = new DefaultPropertyBag();
        bag.Set(new PropertyBagKey<object>("Value"), "text");

        PropertyBagKey<string> textKey = new("Value");

        // false: the key's type is the declared T, not the value's type
        bool found = bag.TryGetValue(textKey, out string? value);
        // end-snippet

        Assert.False(found);
        Assert.Null(value);
    }
}
