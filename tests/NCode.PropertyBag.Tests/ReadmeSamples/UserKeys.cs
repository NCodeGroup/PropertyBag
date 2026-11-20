// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace ReadmeSamples;

// begin-snippet: DefineKeys
using NCode.PropertyBag;

public static class UserKeys
{
    public static readonly PropertyBagKey<string> UserName = new("UserName");
    public static readonly PropertyBagKey<int> UserAge = new("UserAge");
}
// end-snippet
