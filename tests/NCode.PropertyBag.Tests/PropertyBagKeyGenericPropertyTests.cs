// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using FsCheck;
using FsCheck.Xunit;

namespace NCode.PropertyBag.Tests;

public class PropertyBagKeyGenericPropertyTests
{
    private const string Seed = "20261120,12345";

    #region Equals Tests

    [Property(Replay = Seed)]
    public bool Equals_ForAnyTwoNames_KeysAreEqualOnlyWhenTheNamesAreOrdinallyEqual(
        NonNull<string> first,
        NonNull<string> second
    )
    {
        var left = new PropertyBagKey<int>(first.Get);
        var right = new PropertyBagKey<int>(second.Get);

        var namesEqual = string.Equals(first.Get, second.Get, StringComparison.Ordinal);

        return left.Equals(right) == namesEqual
            && left.Equals((object)right) == namesEqual
            && left == right == namesEqual
            && left != right == !namesEqual
            && (!namesEqual || left.GetHashCode() == right.GetHashCode());
    }

    [Property(Replay = Seed)]
    public bool Equals_WhenTheTypeArgumentsDiffer_KeysDiffer(NonNull<string> name)
    {
        var intKey = new PropertyBagKey<int>(name.Get);
        var longKey = new PropertyBagKey<long>(name.Get);

        return !intKey.Equals((object)longKey);
    }

    #endregion

    #region Implicit Conversion Tests

    [Property(Replay = Seed)]
    public bool ImplicitConversion_ForAnyName_KeepsTheTypeTheNameAndTheHashCode(
        NonNull<string> name
    )
    {
        var key = new PropertyBagKey<int>(name.Get);

        PropertyBagKey converted = key;

        return converted == new PropertyBagKey(typeof(int), name.Get)
            && converted.Name == key.Name
            && converted.GetHashCode() == key.GetHashCode();
    }

    [Property(Replay = Seed)]
    public bool ImplicitConversion_WhenTheTypeArgumentsDiffer_ConvertedKeysDiffer(
        NonNull<string> name
    )
    {
        PropertyBagKey intKey = new PropertyBagKey<int>(name.Get);
        PropertyBagKey longKey = new PropertyBagKey<long>(name.Get);

        return intKey != longKey;
    }

    #endregion
}
