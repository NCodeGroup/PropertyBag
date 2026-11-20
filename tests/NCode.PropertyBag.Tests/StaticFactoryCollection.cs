// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

/// <summary>
/// Runs the tests that replace <see cref="DefaultPropertyBagFactory.Singleton"/> or
/// <see cref="PropertyBagFactory.DefaultFactory"/> one after another, since both are process-wide.
/// </summary>
internal static class StaticFactoryCollection
{
    public const string Name = "Static factories";
}
