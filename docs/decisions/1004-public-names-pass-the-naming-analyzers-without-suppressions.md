# 1004. Public names pass the naming analyzers without suppressions

- **Status:** Accepted
- **Date:** 2026-11-20
- **Deciders:** NCode Group
- **Supersedes:** ADR-1001

## Context

Three shipped public names trip a .NET naming analyzer that the build enforces as an error:

- `IPropertyBag.Set` matches a language keyword in Visual Basic (CA1716).
- The two extension blocks of `PropertyBagExtensions`, one for `IPropertyBag` and one for `IReadOnlyPropertyBag`, compile
  to generated names that differ only by case in the analyzer's view (CA1708).
- `DefaultPropertyBag` implements `IReadOnlyDictionary` without a `Dictionary` suffix (CA1710).

[ADR-1001](1001-public-names-that-trip-naming-analyzers-keep-their-suppressions-until-the-next-major-version.md) keeps
a `SuppressMessage` attribute on each until the next major version. Version 2.0 is that version
([ADR-1002](1002-the-packages-target-only-supported-lts-dotnet-versions.md)), so each rename is decided here.

## Decision

**The 2.0 public API passes the .NET naming analyzers with no suppression.**

- `IPropertyBag.SetValue<T>(key, value)` replaces `Set`, and pairs with `TryGetValue`.
- The extension `SetValue<T>(value)` is in `PropertyBagExtensions` and the extension `TryGetValue<T>(out value)` is in
  `ReadOnlyPropertyBagExtensions`: each static class has one receiver type.
- The extension takes the interface's verb too, so a 1.x call `bag.Set(key, "x")` fails to compile instead of binding
  to an extension that would store the key itself.
- `PropertyBagDictionary` is the default bag.
- `PropertyBagDictionary`, `DefaultPropertyBagFactory` and `DefaultPropertyBagScope` are `sealed`, since none is
  designed for derivation, and the extension members and `AddPropertyBag()` throw `ArgumentNullException` for a `null`
  receiver, as the C# conventions ask.
- A new public name passes the analyzers, and no suppression is added for one.

## Options considered

1. Keep the names and the suppressions for 2.x. Rejected: it carries the interop issue with Visual Basic and the
   analyzer exceptions into another major version.
2. Rename only the interface member. Rejected: a 1.x call `bag.Set(key, "x")` then binds silently to an extension that
   would store the key itself.
3. Rename as listed above (chosen).

## Consequences

- Consumers rename `Set`, `TryGet` and `DefaultPropertyBag` at their call sites, and code that calls the extensions
  through the class name also uses `ReadOnlyPropertyBagExtensions`.
- Code that derives from the sealed classes composes them instead.
- `CompatibilitySuppressions.xml` records the breaks until the validation baseline reaches 2.0.0.

## References

- `src/NCode.PropertyBag.Abstractions/IPropertyBag.cs`, `src/NCode.PropertyBag.Abstractions/PropertyBagExtensions.cs`,
  `src/NCode.PropertyBag/PropertyBagDictionary.cs`
- [ADR-1001](1001-public-names-that-trip-naming-analyzers-keep-their-suppressions-until-the-next-major-version.md)
- `.github/instructions/csharp.instructions.md`: one receiver type per static class
