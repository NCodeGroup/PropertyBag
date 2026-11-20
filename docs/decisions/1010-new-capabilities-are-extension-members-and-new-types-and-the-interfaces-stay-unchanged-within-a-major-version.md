# 1010. New capabilities are extension members and new types, and the interfaces stay unchanged within a major version

- **Status:** Accepted
- **Date:** 2026-11-20
- **Deciders:** NCode Group

## Context

Third parties implement `IReadOnlyPropertyBag`, `IPropertyBag` and `IPropertyBagFactory`. A member added to one of them
breaks every implementation, and package validation fails the build on the change. A capability that every bag offers,
such as a helper that reads or stores a value, still has to reach the existing implementations.

## Decision

**Within a major version, the three interfaces keep their members, and a new capability is an extension member or a new
type.**

- A capability that works through the interface members is an extension member in the existing extension class of its
  receiver ([ADR-1004](1004-public-names-pass-the-naming-analyzers-without-suppressions.md)).
- Behavior that must be atomic is an instance method of the type that can make it atomic, such as
  `ConcurrentPropertyBagDictionary`, which owns the lock or the concurrent collection.
- An extension that constructs an implementation type lives in `NCode.PropertyBag`, because the abstractions package
  depends on no implementation (project-template
  [ADR-0132](project-template/0132-implementation-packages-depend-only-on-abstractions.md)).

## Options considered

1. Default interface members. Rejected: implementers' documentation does not show them, and a default member cannot be
   atomic for a lock the implementer owns.
2. A new interface, such as `IPropertyBag2`. Rejected: every consumer then tests for the second interface, and a bag has
   two shapes.
3. Extension members and new types (chosen).

## Consequences

- Implementations compile and validate unchanged.
- The extension helpers are not atomic on a bag other than the one that offers the atomic method.
- The next major version may fold the helpers into the interfaces.

## References

- `src/NCode.PropertyBag.Abstractions/PropertyBagExtensions.cs`
- [ADR-1004](1004-public-names-pass-the-naming-analyzers-without-suppressions.md)
- project-template [ADR-0132](project-template/0132-implementation-packages-depend-only-on-abstractions.md)
