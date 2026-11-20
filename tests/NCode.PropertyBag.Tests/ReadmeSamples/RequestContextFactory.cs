// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace ReadmeSamples;

// begin-snippet: InjectFactory
using NCode.PropertyBag;

public sealed class RequestContextFactory(IPropertyBagFactory propertyBagFactory)
{
    public IPropertyBag CreateContext(string userName) =>
        propertyBagFactory.Create().SetValue(UserKeys.UserName, userName);
}
// end-snippet
