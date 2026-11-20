// Copyright © NCode Group
// SPDX-License-Identifier: Apache-2.0

using System.Globalization;
using FsCheck;
using FsCheck.Xunit;

namespace NCode.PropertyBag.Tests;

public class PropertyBagDictionaryPropertyTests
{
    private const string KeyName = "key";

    // Operations name one of a few keys, so a generated sequence sets, overwrites and removes the same keys often.
    private const int NameCount = 4;

    private static string NameOf(byte name) =>
        (name % NameCount).ToString(CultureInfo.InvariantCulture);

    private static Dictionary<string, string?> Apply(
        PropertyBagDictionary bag,
        Dictionary<string, string?> model,
        (bool Remove, byte Name, string? Value)[] operations
    )
    {
        foreach (var (remove, nameByte, value) in operations)
        {
            var name = NameOf(nameByte);
            var key = new PropertyBagKey<string>(name);
            if (remove)
            {
                bag.Remove(key);
                model.Remove(name);
            }
            else
            {
                bag.SetValue(key, value);
                model[name] = value;
            }
        }

        return model;
    }

    private static bool Matches(IPropertyBag bag, Dictionary<string, string?> model)
    {
        var dictionary = Assert.IsType<PropertyBagDictionary>(bag);
        IReadOnlyDictionary<PropertyBagKey, object?> entries = dictionary;

        for (byte nameByte = 0; nameByte < NameCount; nameByte++)
        {
            var name = NameOf(nameByte);
            var key = new PropertyBagKey<string>(name);
            var found = bag.TryGetValue(key, out var actual);
            var expectedFound = model.TryGetValue(name, out var expected);
            if (found != expectedFound || actual != expected || entries.ContainsKey(key) != found)
                return false;
        }

        return entries.Count == model.Count;
    }

    #region SetValue Tests

    [Property(Replay = PropertyTestSettings.Seed)]
    public bool SetValue_WhenMixedWithRemove_TheBagMatchesADictionaryModel(
        (bool Remove, byte Name, string? Value)[] operations
    )
    {
        var bag = new PropertyBagDictionary();

        var model = Apply(bag, [], operations);

        return Matches(bag, model);
    }

    #endregion

    #region TryGetValue Tests

    [Property(Replay = PropertyTestSettings.Seed)]
    public bool TryGetValue_WhenTheKeyTypeDiffers_ReturnsFalseAndDefault(
        NonNull<string> name,
        int value
    )
    {
        var intKey = new PropertyBagKey<int>(name.Get);
        var longKey = new PropertyBagKey<long>(name.Get);
        var objectKey = new PropertyBagKey<object>(name.Get);
        var bag = new PropertyBagDictionary();
        bag.SetValue(intKey, value);

        var foundLong = bag.TryGetValue(longKey, out var asLong);
        var foundObject = bag.TryGetValue(objectKey, out var asObject);
        var foundInt = bag.TryGetValue(intKey, out var asInt);

        return !foundLong
            && asLong == 0
            && !foundObject
            && asObject is null
            && foundInt
            && asInt == value;
    }

    #endregion

    #region Scope Tests

    [Property(Replay = PropertyTestSettings.Seed)]
    public bool Scope_WhenNestedScopesAreDisposedInReverse_EachDisposeRestoresTheEnclosingValue(
        bool hasInitial,
        string? initial,
        string?[] values
    )
    {
        var bag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>(KeyName);
        if (hasInitial)
            bag.SetValue(key, initial);

        var scopes = new List<IPropertyBagScope>();
        foreach (var value in values)
        {
            var scope = bag.Scope(key, value);
            scopes.Add(scope);
        }

        for (var index = scopes.Count - 1; index >= 0; index--)
        {
            if (!bag.TryGetValue(key, out var current) || current != values[index])
                return false;

            scopes[index].Dispose();
        }

        var found = bag.TryGetValue(key, out var restored);
        return found == hasInitial && restored == (hasInitial ? initial : null);
    }

    [Property(Replay = PropertyTestSettings.Seed)]
    public bool Scope_WhenDisposedTwice_RestoresOnlyOnce(
        string? before,
        string? scoped,
        string? after
    )
    {
        var bag = new PropertyBagDictionary();
        var key = new PropertyBagKey<string>(KeyName);
        bag.SetValue(key, before);

        var scope = bag.Scope(key, scoped);
        scope.Dispose();
        bag.SetValue(key, after);
        scope.Dispose();

        return bag.TryGetValue(key, out var actual) && actual == after;
    }

    #endregion

    #region Clone Tests

    [Property(Replay = PropertyTestSettings.Seed)]
    public bool Clone_WhenTheOriginalChangesAfterward_TheCloneKeepsItsEntries(
        (bool Remove, byte Name, string? Value)[] operations,
        (bool Remove, byte Name, string? Value)[] laterOperations
    )
    {
        var bag = new PropertyBagDictionary();
        var snapshot = Apply(bag, [], operations);

        var clone = bag.Clone();
        var laterModel = new Dictionary<string, string?>(snapshot);
        Apply(bag, laterModel, laterOperations);

        return Matches(clone, snapshot);
    }

    [Property(Replay = PropertyTestSettings.Seed)]
    public bool Clone_WhenTheCloneChangesAfterward_TheOriginalKeepsItsEntries(
        (bool Remove, byte Name, string? Value)[] operations,
        (bool Remove, byte Name, string? Value)[] laterOperations
    )
    {
        var bag = new PropertyBagDictionary();
        var snapshot = Apply(bag, [], operations);

        var cloneBag = bag.Clone();
        var clone = Assert.IsType<PropertyBagDictionary>(cloneBag);
        var laterModel = new Dictionary<string, string?>(snapshot);
        Apply(clone, laterModel, laterOperations);

        return Matches(bag, snapshot);
    }

    #endregion
}
