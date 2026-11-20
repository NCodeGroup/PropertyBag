# 1000. Disposing a scope restores the previous value

- **Status:** Accepted
- **Date:** 2026-11-19
- **Deciders:** NCode.PropertyBag maintainers

## Context

`IPropertyBag.Scope<T>` sets a value for a while. Its contract, in the XML docs of `IPropertyBag` and
`IPropertyBagScope` and in `README.md`, says that disposing the scope restores the value the key held before, or
removes the key when it held none. `DefaultPropertyBagScope` by itself knows only its bag and its key, so it cannot
restore anything.

## Decision

`DefaultPropertyBag.Scope<T>` captures whether the key held a value and which one, and the scope it returns restores
that state on its first `Dispose`; later calls do nothing.

- A `DefaultPropertyBagScope` built with its public constructor removes its key on `Dispose`.
- Nested scopes restore in reverse order. Scopes disposed out of order leave the state that the last disposed one
  captured.
- The public API does not change: `DefaultPropertyBag.Scope<T>` builds the scope through an `internal` constructor that
  takes the action that restores the captured state.

## Options considered

1. Restore the captured state (chosen). The published contract and the README promise it, and the name `Scope` implies
   it.
2. Keep removing the key and rewrite the contract. Rejected: it changes the documented meaning of a public interface
   and breaks the README's example.
3. A new public scope type or constructor. Rejected: an `internal` constructor does the job without growing the public
   surface.

## Consequences

- Code that relied on removal after a scope over an existing value sees the earlier value come back.
- The change ships as a fix under SemVer, because it makes the code match the documented behavior.
- A key that held no value is still removed, so a scope over a new key leaves the bag as it found it.

## References

- [DefaultPropertyBag.cs](../../NCode.PropertyBag/DefaultPropertyBag.cs)
- [DefaultPropertyBagScope.cs](../../NCode.PropertyBag/DefaultPropertyBagScope.cs)
- [IPropertyBagScope.cs](../../NCode.PropertyBag.Abstractions/IPropertyBagScope.cs)
- [Issue #31](https://github.com/NCodeGroup/PropertyBag/issues/31)
