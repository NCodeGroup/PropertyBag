# 1001. Public names that trip naming analyzers keep their suppressions until the next major version

- **Status:** Accepted
- **Date:** 2026-11-20
- **Deciders:** NCode Group

## Context

Three public names trip a .NET naming analyzer that the build enforces as an error:

- `IPropertyBag.Set` matches a language keyword in some languages (CA1716).
- The extension blocks of `PropertyBagExtensions` compile to generated names that differ only by case in the
  analyzer's view (CA1708).
- `DefaultPropertyBag` implements a dictionary interface without a `Dictionary` or `Collection` suffix (CA1710).

Each name has shipped in a release, so a rename is a breaking change to the public API, which package validation
(`PackageValidationBaselineVersion`) and the `PublicAPI.Shipped.txt` files reject within a major version.

## Decision

**Each of the three names keeps a `SuppressMessage` attribute with a justification until the next major version, which
decides each rename.**

- The attributes sit on the member or type that trips the analyzer, and the justification says the name is the
  published API name.
- A new public name is chosen to pass the analyzers; no new suppression is added for a name that has not shipped.

## Options considered

1. Keep the names and suppress (chosen). The names are in use by consumers, and the suppression is local and explained.
2. Rename now. Rejected: a breaking change to a shipped API in a minor or patch release.
3. Disable the rules in `.editorconfig`. Rejected: it silences the analyzer for every future name, not these three.

## Consequences

- The suppressions are visible in the code, and the next major version has a list of renames to decide.
- A consumer's code is unaffected.

## References

- `src/NCode.PropertyBag.Abstractions/IPropertyBag.cs`, `src/NCode.PropertyBag.Abstractions/PropertyBagExtensions.cs`,
  `src/NCode.PropertyBag/DefaultPropertyBag.cs`
- [docs/api-compatibility.md](../api-compatibility.md)
