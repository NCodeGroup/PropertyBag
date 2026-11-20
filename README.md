<p align="center">
  <img src="NCodeGroup.png" alt="NCode" width="96" height="96">
</p>

<h1 align="center">NCode.PropertyBag</h1>

<p align="center">
  Strongly typed property bags for .NET: store and read values by type-safe keys, override a value for a scope, clone a bag, and create bags through dependency injection.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/NCode.PropertyBag/"><img alt="NuGet version" src="https://img.shields.io/nuget/v/NCode.PropertyBag?logo=nuget"></a>
  <a href="https://www.nuget.org/packages/NCode.PropertyBag/"><img alt="NuGet downloads" src="https://img.shields.io/nuget/dt/NCode.PropertyBag?logo=nuget"></a>
  <a href="https://github.com/NCodeGroup/PropertyBag/actions/workflows/ci.yml?query=branch%3Amain"><img alt="CI" src="https://github.com/NCodeGroup/PropertyBag/actions/workflows/ci.yml/badge.svg?branch=main"></a>
  <a href="https://codecov.io/gh/NCodeGroup/PropertyBag"><img alt="Coverage" src="https://codecov.io/gh/NCodeGroup/PropertyBag/branch/main/graph/badge.svg"></a>
  <a href="https://www.nuget.org/packages/NCode.PropertyBag#supportedframeworks-body-tab"><img alt="Target framework" src="https://img.shields.io/badge/dynamic/xml?url=https%3A%2F%2Fraw.githubusercontent.com%2FNCodeGroup%2FPropertyBag%2Fmain%2Fsrc%2FNCode.PropertyBag%2FNCode.PropertyBag.csproj&query=%2F%2FTargetFramework%20%7C%20%2F%2FTargetFrameworks&label=target&logo=dotnet&color=512BD4"></a>
  <a href="LICENSE.txt"><img alt="License" src="https://img.shields.io/github/license/NCodeGroup/PropertyBag"></a>
  <a href="https://scorecard.dev/viewer/?uri=github.com/NCodeGroup/PropertyBag"><img alt="OpenSSF Scorecard" src="https://api.scorecard.dev/projects/github.com/NCodeGroup/PropertyBag/badge"></a>
</p>

A property bag carries context, such as the current user, a culture or a correlation ID, through code that doesn't need to know every value it passes on. Each value is stored and read through a `PropertyBagKey<T>`, which names the value and fixes its type, so a read returns a `T` without a cast.

## Packages

Both packages share the namespace `NCode.PropertyBag`.

| Package | Reference it from |
|---------|-------------------|
| [NCode.PropertyBag](https://www.nuget.org/packages/NCode.PropertyBag/) | Applications: the bag, its factories and AddPropertyBag() |
| [NCode.PropertyBag.Abstractions](https://www.nuget.org/packages/NCode.PropertyBag.Abstractions/) | Libraries that accept or read a bag |

## Installation

```shell
dotnet add package NCode.PropertyBag
```

A library that only accepts or reads bags references the abstractions:

```shell
dotnet add package NCode.PropertyBag.Abstractions
```

The target frameworks are the ones each package's `.csproj` declares; they are listed on [nuget.org's Frameworks tab](https://www.nuget.org/packages/NCode.PropertyBag#supportedframeworks-body-tab) and shown by the badge above. [CHANGELOG.md](CHANGELOG.md) records when a framework is added or dropped and which release line to stay on.

## Quick start

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

## Usage

### Scoped values

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

### Null values

A bag stores null explicitly, and a stored null differs from a missing key:

<!-- snippet: NullValues -->
```cs
var middleNameKey = new PropertyBagKey<string?>("MiddleName");
propertyBag.SetValue(middleNameKey, null);

// True, with a null value: the key holds null.
var found = propertyBag.TryGetValue(middleNameKey, out var middleName);

// False: a missing key differs from a stored null.
var nicknameKey = new PropertyBagKey<string?>("Nickname");
var missing = propertyBag.TryGetValue(nicknameKey, out var nickname);
```
<!-- endSnippet -->

### Keys named after the variable

The `SetValue` and `TryGetValue` extensions name the key after the argument expression, through `[CallerArgumentExpression]`:

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

### Cloning

`Clone` returns an independent bag: a value that implements `ICloneable` is copied through its own `Clone()`, and any other value is shared by reference:

<!-- snippet: Clone -->
```cs
var copy = propertyBag.Clone();
copy.SetValue(UserKeys.UserName, "Bob");

propertyBag.TryGetValue(UserKeys.UserName, out var original); // still "Alice"
```
<!-- endSnippet -->

### Enumerating entries

`PropertyBagDictionary`, the default bag, is also an `IReadOnlyDictionary<PropertyBagKey, object?>`, keyed by each value's name and type:

<!-- snippet: EnumerateEntries -->
```cs
var propertyBag = new PropertyBagDictionary();
propertyBag.SetValue(UserKeys.UserName, "Alice").SetValue(UserKeys.UserAge, 30);

IReadOnlyDictionary<PropertyBagKey, object?> entries = propertyBag;
foreach (var (key, value) in entries)
{
    lines.Add($"{key.Name} ({key.Type.Name}) = {value}");
}
```
<!-- endSnippet -->

### Dependency injection

`AddPropertyBag()` registers `IPropertyBagFactory` as a singleton, keeping one the application registered first:

<!-- snippet: RegisterServices -->
```cs
var services = new ServiceCollection();
services.AddPropertyBag();
services.AddSingleton<RequestContextFactory>();
```
<!-- endSnippet -->

Inject `IPropertyBagFactory` where a bag is needed:

<!-- snippet: InjectFactory -->
```cs
using NCode.PropertyBag;

public sealed class RequestContextFactory(IPropertyBagFactory propertyBagFactory)
{
    public IPropertyBag CreateContext(string userName) =>
        propertyBagFactory.Create().SetValue(UserKeys.UserName, userName);
}
```
<!-- endSnippet -->

Without a container, `PropertyBagFactory.Create()` creates a bag through the settable `PropertyBagFactory.DefaultFactory`.

## API overview

### NCode.PropertyBag.Abstractions

| Type | Role |
| --- | --- |
| `PropertyBagKey<T>` | A value's name and type; converts to `PropertyBagKey` |
| `PropertyBagKey` | The untyped key: a `Type` and a `Name` |
| `IReadOnlyPropertyBag` | `TryGetValue` and `Clone` |
| `IPropertyBag` | Adds `SetValue`, `Remove` and `Scope`, each chainable |
| `IPropertyBagScope` | Restores its key when disposed |
| `PropertyBagExtensions` | `SetValue(value)`, the key named after the argument |
| `ReadOnlyPropertyBagExtensions` | `TryGetValue(out value)`, the key named after the argument |

### NCode.PropertyBag

| Type | Role |
| --- | --- |
| `PropertyBagDictionary` | The default bag; allocates nothing until its first value |
| `DefaultPropertyBagScope` | The scope `PropertyBagDictionary.Scope` returns |
| `IPropertyBagFactory` | Creates bags |
| `DefaultPropertyBagFactory` | Creates a `PropertyBagDictionary`; `Singleton` is settable |
| `PropertyBagFactory` | Static `Create()` through the settable `DefaultFactory` |
| `DefaultRegistration` | `AddPropertyBag()` for `IServiceCollection` |

## Thread safety

A bag and its scopes hold no lock: synchronize concurrent use of one bag. [docs/assurance-case.md](docs/assurance-case.md) lists what the library guarantees and what it doesn't.

## Project

- [CONTRIBUTING.md](CONTRIBUTING.md): how to set up, build, test and propose a change.
- [SECURITY.md](SECURITY.md): report a vulnerability privately through the repository's Security tab, never in a public issue.
- [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md), [GOVERNANCE.md](GOVERNANCE.md) and [ROADMAP.md](ROADMAP.md): how we work together, who decides, and where the project is going.
- [docs/architecture.md](docs/architecture.md) and [docs/assurance-case.md](docs/assurance-case.md): how the project is built, and why it meets its security requirements.
- [docs/decisions/](docs/decisions/README.md): the decisions behind the design, the project's and the template's.
- [CHANGELOG.md](CHANGELOG.md): what changed in each release.
- [LICENSE.txt](LICENSE.txt): Apache License 2.0, copyright NCode Group.
