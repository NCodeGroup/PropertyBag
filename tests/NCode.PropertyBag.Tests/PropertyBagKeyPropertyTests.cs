// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using FsCheck;
using FsCheck.Xunit;

namespace NCode.PropertyBag.Tests;

public class PropertyBagKeyPropertyTests
{
    private const string Seed = "20261120,12345";

    #region Equals Tests

    [Property(Replay = Seed)]
    public bool Equals_WhenTypeAndNameMatch_KeysAreEqualWithEqualHashCodes(NonNull<string> name)
    {
        var left = new PropertyBagKey(typeof(int), name.Get);
        var right = new PropertyBagKey(typeof(int), new string(name.Get.AsSpan()));

        return left == right
            && !(left != right)
            && left.Equals((object)right)
            && left.GetHashCode() == right.GetHashCode();
    }

    [Property(Replay = Seed)]
    public bool Equals_ForAnyTwoNames_KeysAreEqualOnlyWhenTheNamesAreOrdinallyEqual(
        NonNull<string> first,
        NonNull<string> second
    )
    {
        var left = new PropertyBagKey(typeof(string), first.Get);
        var right = new PropertyBagKey(typeof(string), second.Get);

        var namesEqual = string.Equals(first.Get, second.Get, StringComparison.Ordinal);

        return left.Equals(right) == namesEqual
            && left == right == namesEqual
            && left != right == !namesEqual;
    }

    [Property(Replay = Seed)]
    public bool Equals_WhenOnlyTheTypeDiffers_KeysDiffer(NonNull<string> name)
    {
        var left = new PropertyBagKey(typeof(int), name.Get);
        var right = new PropertyBagKey(typeof(long), name.Get);

        return left != right && !left.Equals((object)right);
    }

    #endregion
}
