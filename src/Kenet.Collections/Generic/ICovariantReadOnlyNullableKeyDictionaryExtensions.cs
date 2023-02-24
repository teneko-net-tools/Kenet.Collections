// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Kenet.Collections.Generic
{
    public static class ICovariantReadOnlyNullableKeyDictionaryExtensions
    {
        public static bool TryGetValue<TKey, TValue>(this ICovariantReadOnlyNullableKeyDictionary<TKey, TValue> dictionary, YetNullable<TKey> key, out TValue value)
            where TKey : notnull
        {
            bool gotValue;
            (gotValue, value) = dictionary.TryGetValue(key);
            return gotValue;
        }
    }
}
