// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using NCode.PropertyBag;

namespace ReadmeSamples;

// begin-snippet: ReadmeInjectFactory
public sealed class MyService(IPropertyBagFactory factory)
{
    public IPropertyBag CreateContext() => factory.Create();
}
// end-snippet
