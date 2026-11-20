// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using FsCheck.Xunit;

namespace NCode.PropertyBag.Tests;

public class PropertyBagExtensionsPropertyTests
{
    private const string Seed = "20261120,12345";

    #region SetValue Tests

    [Property(Replay = Seed)]
    public bool SetValue_WhenReadBackUnderTheSameName_ReturnsTheValueUnderTheNamedKey(
        string? name,
        int value
    )
    {
        IPropertyBag bag = new PropertyBagDictionary();

        bag.SetValue(value, name);

        return bag.TryGetValue<int>(out var byName, name)
            && byName == value
            && bag.TryGetValue(new PropertyBagKey<int>(name ?? string.Empty), out var byKey)
            && byKey == value;
    }

    #endregion
}
