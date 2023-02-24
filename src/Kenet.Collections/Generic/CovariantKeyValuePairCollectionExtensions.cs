﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Generic
{
    public static class CovariantKeyValuePairCollectionExtensions
    {
        public static IReadOnlyCollection<KeyValuePair<TKey, TValue>> AsReadOnlyKeyValuePairCollection<TKey, TValue>(this CovariantKeyValuePairCollection<TKey, TValue> collection) =>
            collection;

        public static ICovariantKeyValuePairCollection<KeyType, ValueType> AsCovariantKeyValuePairCollection<KeyType, ValueType>(this CovariantKeyValuePairCollection<KeyType, ValueType> collection) =>
            collection;
    }
}
