# NCode.PropertyBag.Abstractions

[![CI](https://github.com/NCodeGroup/PropertyBag/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/NCodeGroup/PropertyBag/actions/workflows/ci.yml?query=branch%3Amain) [![Coverage](https://codecov.io/gh/NCodeGroup/PropertyBag/branch/main/graph/badge.svg)](https://codecov.io/gh/NCodeGroup/PropertyBag) [![Target framework](https://img.shields.io/badge/dynamic/xml?url=https%3A%2F%2Fraw.githubusercontent.com%2FNCodeGroup%2FPropertyBag%2Fmain%2Fsrc%2FNCode.PropertyBag.Abstractions%2FNCode.PropertyBag.Abstractions.csproj&query=%2F%2FTargetFramework%20%7C%20%2F%2FTargetFrameworks&label=target&logo=dotnet&color=512BD4)](https://www.nuget.org/packages/NCode.PropertyBag.Abstractions#supportedframeworks-body-tab) [![License](https://img.shields.io/github/license/NCodeGroup/PropertyBag)](https://github.com/NCodeGroup/PropertyBag/blob/main/LICENSE.txt)

The interfaces and strongly typed keys of NCode.PropertyBag: `IPropertyBag`, `IReadOnlyPropertyBag`, `IPropertyBagScope`, `PropertyBagKey<T>` and the extensions that name a key after its variable. Reference this package from a library that accepts or reads a property bag; the application references [NCode.PropertyBag](https://www.nuget.org/packages/NCode.PropertyBag/), which creates them.

## Installation

```shell
dotnet add package NCode.PropertyBag.Abstractions
```

The target frameworks are the ones the package's `.csproj` declares; they are listed on [nuget.org's Frameworks tab](https://www.nuget.org/packages/NCode.PropertyBag.Abstractions#supportedframeworks-body-tab) and shown by the badge above. [CHANGELOG.md](https://github.com/NCodeGroup/PropertyBag/blob/main/CHANGELOG.md) records when a framework is added or dropped and which release line to stay on.

## Example

A library defines its keys once and reads and writes through the interfaces:

<!-- snippet: DefineKeys -->
```cs
using NCode.PropertyBag;

public static class UserKeys
{
    public static readonly PropertyBagKey<string> UserName = new("UserName");
    public static readonly PropertyBagKey<int> UserAge = new("UserAge");
}
```
<!-- endSnippet -->

<!-- snippet: InferredKeys -->
```cs
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
```
<!-- endSnippet -->

## Learn more

- [Documentation site](https://propertybag.ncode.io/): the quick start, the guides, the concepts and the API map
- [README and samples](https://github.com/NCodeGroup/PropertyBag#readme)
- [API overview](https://github.com/NCodeGroup/PropertyBag#api-overview)
- [Changelog](https://github.com/NCodeGroup/PropertyBag/blob/main/CHANGELOG.md)
- [Report a vulnerability](https://github.com/NCodeGroup/PropertyBag/blob/main/SECURITY.md)
