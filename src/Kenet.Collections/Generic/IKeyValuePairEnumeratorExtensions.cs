﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Generic
{
    public static class IKeyValuePairEnumerableExtensions
    {
        public static IEnumerator<ICovariantKeyValuePair<KeyType?, ValueType>> GetEnumeratorWithCovariantPairsHavingNullableKey<KeyType, ValueType>(this IEnumerable<KeyValuePair<KeyType, ValueType>> enumerable)
           where KeyType : struct =>
           new KeyValuePairEnumeratorWithPairAsCovariantHavingKeyAsNullable<KeyType, ValueType>(enumerable.GetEnumerator());
    }
}
