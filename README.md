<p align="center">
  <img src="NCodeGroup.png" alt="NCode" width="96" height="96">
</p>

<h1 align="center">NCode.PropertyBag</h1>

<p align="center">
  A lightweight, strongly-typed property bag library for .NET that provides type-safe key-value storage with support for scoped values, cloning, and dependency injection.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/NCode.PropertyBag/"><img alt="NuGet version" src="https://img.shields.io/nuget/v/NCode.PropertyBag?logo=nuget"></a>
  <a href="https://www.nuget.org/packages/NCode.PropertyBag/"><img alt="NuGet downloads" src="https://img.shields.io/nuget/dt/NCode.PropertyBag?logo=nuget"></a>
  <a href="https://github.com/NCodeGroup/PropertyBag/actions/workflows/ci.yml?query=branch%3Amain"><img alt="CI" src="https://github.com/NCodeGroup/PropertyBag/actions/workflows/ci.yml/badge.svg?branch=main"></a>
  <a href="https://codecov.io/gh/NCodeGroup/PropertyBag"><img alt="Coverage" src="https://codecov.io/gh/NCodeGroup/PropertyBag/branch/main/graph/badge.svg"></a>
  <a href="LICENSE.txt"><img alt="License" src="https://img.shields.io/github/license/NCodeGroup/PropertyBag"></a>
</p>

The [documentation site](https://propertybag.ncode.io/) has a quick start, a guide for each task, the concepts behind keys and the default bag, and a reference for every public member.

## Features

- **Strongly-Typed Keys** - Type-safe `PropertyBagKey<T>` ensures compile-time type checking when storing and retrieving values
- **Nullable Value Support** - Explicitly store and retrieve `null` values for reference types and nullable value types
- **Read-Only Interface** - `IReadOnlyPropertyBag` for passing property bags to code that should only read values
- **Fluent API** - Method chaining support for `Set` and `Remove` operations
- **Scoped Values** - Temporarily override values with automatic cleanup using `IPropertyBagScope`
- **Cloning Support** - Create independent copies with `Clone()`, supporting deep cloning for `ICloneable` values
- **Lazy Initialization** - Empty property bags are lightweight with no memory allocated until first use
- **Dictionary Access** - `DefaultPropertyBag` implements `IReadOnlyDictionary<PropertyBagKey, object?>` for enumeration
- **Dependency Injection** - Built-in integration with `Microsoft.Extensions.DependencyInjection`
- **Convenience Extensions** - Automatic key name inference using `CallerArgumentExpression`

## Installation

```shell
dotnet add package NCode.PropertyBag
```

Or for abstractions only:

```shell
dotnet add package NCode.PropertyBag.Abstractions
```

## Quick Start

### Basic Usage

<!-- snippet: ReadmeDefineKeys -->
```cs
// Define keys (typically as static readonly fields)
public static readonly PropertyBagKey<string> UserNameKey = new("UserName");
public static readonly PropertyBagKey<int> UserAgeKey = new("UserAge");
```
<!-- endSnippet -->

<!-- snippet: ReadmeBasicUsage -->
```cs
// Create and use a property bag
var propertyBag = PropertyBagFactory.Create();

propertyBag.Set(UserNameKey, "Alice").Set(UserAgeKey, 30);

if (propertyBag.TryGetValue(UserNameKey, out var userName))
{
    Console.WriteLine($"User: {userName}"); // Output: User: Alice
}
```
<!-- endSnippet -->

### Scoped Values

Temporarily override a value that automatically restores when disposed:

<!-- snippet: ReadmeScopedValues -->
```cs
string? culture;
var cultureKey = new PropertyBagKey<string>("Culture");
propertyBag.Set(cultureKey, "en-US");

using (propertyBag.Scope(cultureKey, "fr-FR"))
{
    // Within this scope, Culture is "fr-FR"
    propertyBag.TryGetValue(cultureKey, out culture); // "fr-FR"
}

// After disposal, Culture is restored to "en-US"
propertyBag.TryGetValue(cultureKey, out var restored); // "en-US"
```
<!-- endSnippet -->

### Dependency Injection

Register services with the DI container:

<!-- snippet: ReadmeRegisterServices -->
```cs
services.AddPropertyBag();
```
<!-- endSnippet -->

Then inject `IPropertyBagFactory` where needed:

<!-- snippet: ReadmeInjectFactory -->
```cs
public sealed class MyService(IPropertyBagFactory factory)
{
    public IPropertyBag CreateContext() => factory.Create();
}
```
<!-- endSnippet -->

### Convenience Extensions

Use automatic key name inference:

<!-- snippet: ReadmeSetInferred -->
```cs
const string connectionString = "Server=localhost;Database=test";
var timeout = TimeSpan.FromSeconds(30);

// Keys are inferred as "connectionString" and "timeout"
propertyBag.Set(connectionString).Set(timeout);
```
<!-- endSnippet -->

Retrieve a value with an inferred key name:

<!-- snippet: ReadmeReadInferred -->
```cs
// Retrieve with inferred key name
// Declare the variable first: an inline declaration (out string? x) names the key "string? x"
string? connectionString;
if (propertyBag.TryGet(out connectionString)) // Key inferred as "connectionString"
{
    Console.WriteLine(connectionString);
}
```
<!-- endSnippet -->

### Nullable Values

Null values can be explicitly stored and retrieved:

<!-- snippet: ReadmeNullableValues -->
```cs
var optionalNameKey = new PropertyBagKey<string?>("OptionalName");

// Explicitly store null
propertyBag.Set(optionalNameKey, null);

// TryGetValue returns true because the key exists (even though value is null)
if (propertyBag.TryGetValue(optionalNameKey, out var name))
{
    // name is null here, but the key was found
    Console.WriteLine(name ?? "(not set)");
}

// This is different from a missing key (name the type argument: a bare discard infers string)
var missingKey = new PropertyBagKey<string?>("Missing");
if (!propertyBag.TryGetValue<string?>(missingKey, out _))
{
    // Key does not exist
}
```
<!-- endSnippet -->

## API Overview

### Abstractions (`NCode.PropertyBag.Abstractions`)

| Type | Description |
|------|-------------|
| `PropertyBagKey` | Non-generic key with Type and Name properties |
| `PropertyBagKey<T>` | Strongly-typed key for type-safe value access |
| `IReadOnlyPropertyBag` | Read-only interface with `TryGetValue` and `Clone` |
| `IPropertyBag` | Mutable interface adding `Set`, `Remove`, and `Scope` |
| `IPropertyBagScope` | Disposable scope for temporary value overrides |
| `PropertyBagExtensions` | Convenience methods with automatic key inference |

### Implementation (`NCode.PropertyBag`)

| Type | Description |
|------|-------------|
| `IPropertyBagFactory` | Factory interface for creating property bags |
| `DefaultPropertyBag` | Dictionary-based implementation with lazy initialization |
| `DefaultPropertyBagFactory` | Default factory with singleton support |
| `DefaultPropertyBagScope` | Scope implementation that restores the previous value, or removes the key when it held none, on dispose |
| `PropertyBagFactory` | Static convenience class for creating property bags |
| `DefaultRegistration` | DI registration extension methods |

## License

Licensed under the Apache License, Version 2.0. See [LICENSE.txt](LICENSE.txt) for details.

## Target Frameworks

- .NET 8.0
- .NET 10.0

## Project

- [Documentation site](https://propertybag.ncode.io/): the quick start, the guides, the concepts and the API map.
- [CONTRIBUTING.md](CONTRIBUTING.md): how to set up, build, test and propose a change.
- [SECURITY.md](SECURITY.md): how to report a vulnerability privately.
- [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md), [GOVERNANCE.md](GOVERNANCE.md) and [ROADMAP.md](ROADMAP.md): how we work together, who decides, and where the project is going.
- [docs/architecture.md](docs/architecture.md) and [docs/assurance-case.md](docs/assurance-case.md): how the project is built, and why it meets its security requirements.
- [docs/decisions/](docs/decisions/README.md): the decisions behind the design, the project's and the template's.
- [CHANGELOG.md](CHANGELOG.md): what changed in each release, in the Keep a Changelog format.
- [LICENSE.txt](LICENSE.txt): the license.
