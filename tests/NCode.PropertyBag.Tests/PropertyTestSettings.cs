// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

namespace NCode.PropertyBag.Tests;

/// <summary>
/// The settings every FsCheck property test in this project shares.
/// </summary>
internal static class PropertyTestSettings
{
    /// <summary>
    /// The seed every property test replays, so every run checks the same generated cases.
    /// </summary>
    public const string Seed = "20261120,12345";
}
