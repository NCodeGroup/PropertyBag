# NCode.PropertyBag

[![CI](https://github.com/NCodeGroup/PropertyBag/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/NCodeGroup/PropertyBag/actions/workflows/ci.yml?query=branch%3Amain) [![Coverage](https://codecov.io/gh/NCodeGroup/PropertyBag/branch/main/graph/badge.svg)](https://codecov.io/gh/NCodeGroup/PropertyBag) [![Target framework](https://img.shields.io/badge/dynamic/xml?url=https%3A%2F%2Fraw.githubusercontent.com%2FNCodeGroup%2FPropertyBag%2Fmain%2Fsrc%2FNCode.PropertyBag%2FNCode.PropertyBag.csproj&query=%2F%2FTargetFramework%20%7C%20%2F%2FTargetFrameworks&label=target&logo=dotnet&color=512BD4)](https://www.nuget.org/packages/NCode.PropertyBag#supportedframeworks-body-tab) [![License](https://img.shields.io/github/license/NCodeGroup/PropertyBag)](https://github.com/NCodeGroup/PropertyBag/blob/main/LICENSE.txt)

Strongly typed property bags for .NET: store and read values by type-safe keys, override a value for a scope, clone a bag, and create bags through dependency injection.

This package holds the bag, `PropertyBagDictionary`, its factories and `AddPropertyBag()`; the interfaces and keys are in [NCode.PropertyBag.Abstractions](https://www.nuget.org/packages/NCode.PropertyBag.Abstractions/), which it depends on.

## Installation

```shell
dotnet add package NCode.PropertyBag
```

The target frameworks are the ones the package's `.csproj` declares; they are listed on [nuget.org's Frameworks tab](https://www.nuget.org/packages/NCode.PropertyBag#supportedframeworks-body-tab) and shown by the badge above. [CHANGELOG.md](https://github.com/NCodeGroup/PropertyBag/blob/main/CHANGELOG.md) records when a framework is added or dropped and which release line to stay on.

## Example

Define each key once, typically as a `static readonly` field:

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

Then create a bag, store values and read them back:

<!-- snippet: BasicUsage -->
```cs
var propertyBag = PropertyBagFactory.Create();

propertyBag.SetValue(UserKeys.UserName, "Alice").SetValue(UserKeys.UserAge, 30);

if (propertyBag.TryGetValue(UserKeys.UserName, out var userName))
{
    Console.WriteLine($"User: {userName}"); // User: Alice
}
```
<!-- endSnippet -->

`Scope` sets a value until the returned scope is disposed, then restores the value the key held before, or removes the key when it held none:

<!-- snippet: ScopedValues -->
```cs
var cultureKey = new PropertyBagKey<string>("Culture");
propertyBag.SetValue(cultureKey, "en-US");

string? inScope;
using (propertyBag.Scope(cultureKey, "fr-FR"))
{
    propertyBag.TryGetValue(cultureKey, out inScope); // "fr-FR"
}

propertyBag.TryGetValue(cultureKey, out var afterScope); // "en-US" again
```
<!-- endSnippet -->

## Dependency injection

`AddPropertyBag()` registers `IPropertyBagFactory` as a singleton, keeping one the application registered first:

<!-- snippet: RegisterServices -->
```cs
var services = new ServiceCollection();
services.AddPropertyBag();
services.AddSingleton<RequestContextFactory>();
```
<!-- endSnippet -->

## Learn more

- [README and samples](https://github.com/NCodeGroup/PropertyBag#readme)
- [API overview](https://github.com/NCodeGroup/PropertyBag#api-overview)
- [Changelog](https://github.com/NCodeGroup/PropertyBag/blob/main/CHANGELOG.md)
- [Report a vulnerability](https://github.com/NCodeGroup/PropertyBag/blob/main/SECURITY.md)
