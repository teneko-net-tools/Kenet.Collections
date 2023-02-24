﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Generic
{
    public class KeyValuePairEnumeratorWithPairAsCovariantHavingKeyAsNullable<KeyType, ValueType> : KeyValuePairEnumeratorWithConversionBase<ICovariantKeyValuePair<KeyType?, ValueType>, KeyType, ValueType>
        where KeyType : struct
    {
        public KeyValuePairEnumeratorWithPairAsCovariantHavingKeyAsNullable(IEnumerator<KeyValuePair<KeyType, ValueType>> enumerator)
            : base(enumerator)
        { }

        protected override ICovariantKeyValuePair<KeyType?, ValueType> CreateCurrent(KeyValuePair<KeyType, ValueType> currentPair) =>
            new CovariantKeyValuePair<KeyType?, ValueType>(currentPair.Key, currentPair.Value);
    }
}
