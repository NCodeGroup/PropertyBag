// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace ReadmeSamples;

// begin-snippet: InferredKeys
using NCode.PropertyBag;

public static class ConnectionSettings
{
    public static void Save(IPropertyBag propertyBag, string connectionString, TimeSpan timeout)
    {
        // The keys are named "connectionString" and "timeout", after the arguments.
        propertyBag.SetValue(connectionString).SetValue(timeout);
    }

    public static string? LoadConnectionString(IReadOnlyPropertyBag propertyBag)
    {
        // Declare the variable first: an inline `out string? connectionString` would name
        // the key "string? connectionString".
        string? connectionString;
        return propertyBag.TryGetValue(out connectionString) ? connectionString : null;
    }
}
// end-snippet
