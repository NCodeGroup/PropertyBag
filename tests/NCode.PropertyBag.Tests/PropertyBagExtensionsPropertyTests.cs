// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using FsCheck.Xunit;

namespace NCode.PropertyBag.Tests;

public class PropertyBagExtensionsPropertyTests
{
    #region SetValue Tests

    [Property(Replay = PropertyTestSettings.Seed)]
    public bool SetValue_WhenReadBackUnderTheSameName_ReturnsTheValueUnderTheNamedKey(
        string? name,
        int value
    )
    {
        var namedKey = new PropertyBagKey<int>(name ?? string.Empty);
        IPropertyBag bag = new PropertyBagDictionary();

        bag.SetValue(value, name);

        return bag.TryGetValue<int>(out var byName, name)
            && byName == value
            && bag.TryGetValue(namedKey, out var byKey)
            && byKey == value;
    }

    #endregion
}
