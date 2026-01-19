[![ci](https://github.com/NCodeGroup/PropertyBag/actions/workflows/main.yml/badge.svg)](https://github.com/NCodeGroup/PropertyBag/actions)
[![Nuget](https://img.shields.io/nuget/v/NCode.PropertyBag.svg)](https://www.nuget.org/packages/NCode.PropertyBag/)

# NCode.PropertyBag

A lightweight, strongly-typed property bag library for .NET that provides type-safe key-value storage with support for scoped values, cloning, and dependency injection.

## Features

- **Strongly-Typed Keys** - Type-safe `PropertyBagKey<T>` ensures compile-time type checking when storing and retrieving values
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

```csharp
// Define keys (typically as static readonly fields)
public static readonly PropertyBagKey<string> UserNameKey = new("UserName");
public static readonly PropertyBagKey<int> UserAgeKey = new("UserAge");

// Create and use a property bag
var propertyBag = PropertyBagFactory.Create();

propertyBag
    .Set(UserNameKey, "Alice")
    .Set(UserAgeKey, 30);

if (propertyBag.TryGetValue(UserNameKey, out var userName))
{
    Console.WriteLine($"User: {userName}"); // Output: User: Alice
}
```

### Scoped Values

Temporarily override a value that automatically restores when disposed:

```csharp
var cultureKey = new PropertyBagKey<string>("Culture");
propertyBag.Set(cultureKey, "en-US");

using (propertyBag.Scope(cultureKey, "fr-FR"))
{
    // Within this scope, Culture is "fr-FR"
    propertyBag.TryGetValue(cultureKey, out var culture); // "fr-FR"
}

// After disposal, Culture is restored to "en-US"
propertyBag.TryGetValue(cultureKey, out var restored); // "en-US"
```

### Dependency Injection

Register services with the DI container:

```csharp
services.AddPropertyBag();
```

Then inject `IPropertyBagFactory` where needed:

```csharp
public class MyService
{
    private readonly IPropertyBagFactory _factory;

    public MyService(IPropertyBagFactory factory)
    {
        _factory = factory;
    }

    public IPropertyBag CreateContext() => _factory.Create();
}
```

### Convenience Extensions

Use automatic key name inference:

```csharp
var connectionString = "Server=localhost;Database=test";
var timeout = TimeSpan.FromSeconds(30);

// Keys are inferred as "connectionString" and "timeout"
propertyBag
    .Set(connectionString)
    .Set(timeout);

// Retrieve with inferred key name
if (propertyBag.TryGet(out string? connectionString))
{
    Console.WriteLine(connectionString);
}
```

## API Overview

### Abstractions (`NCode.PropertyBag.Abstractions`)

| Type | Description |
|------|-------------|
| `PropertyBagKey` | Non-generic key with Type and Name properties |
| `PropertyBagKey<T>` | Strongly-typed key for type-safe value access |
| `IReadOnlyPropertyBag` | Read-only interface with `TryGetValue` and `Clone` |
| `IPropertyBag` | Mutable interface adding `Set`, `Remove`, and `Scope` |
| `IPropertyBagScope` | Disposable scope for temporary value overrides |
| `IPropertyBagFactory` | Factory interface for creating property bags |
| `PropertyBagExtensions` | Convenience methods with automatic key inference |

### Implementation (`NCode.PropertyBag`)

| Type | Description |
|------|-------------|
| `DefaultPropertyBag` | Dictionary-based implementation with lazy initialization |
| `DefaultPropertyBagFactory` | Default factory with singleton support |
| `DefaultPropertyBagScope` | Scope implementation that removes values on dispose |
| `PropertyBagFactory` | Static convenience class for creating property bags |
| `DefaultRegistration` | DI registration extension methods |

## License

Licensed under the Apache License, Version 2.0. See [LICENSE.txt](LICENSE.txt) for details.

## Release Notes
* v1.0.0 - Initial release
* v1.0.1 - Fix CI build
