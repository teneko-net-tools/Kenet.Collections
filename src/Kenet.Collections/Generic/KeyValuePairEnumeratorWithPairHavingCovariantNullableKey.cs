﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Teronis.Collections.Generic
{
    public class KeyValuePairEnumeratorWithPairHavingCovariantNullableKey<KeyType, ValueType> : KeyValuePairEnumeratorWithConversionBase<KeyValuePair<IYetNullable<KeyType>, ValueType>, YetNullable<KeyType>, ValueType>
        where KeyType : notnull
    {
        public KeyValuePairEnumeratorWithPairHavingCovariantNullableKey(IEnumerator<KeyValuePair<YetNullable<KeyType>, ValueType>> enumerator)
            : base(enumerator)
        { }

        protected override KeyValuePair<IYetNullable<KeyType>, ValueType> CreateCurrent(KeyValuePair<YetNullable<KeyType>, ValueType> currentPair) =>
            new KeyValuePair<IYetNullable<KeyType>, ValueType>(currentPair.Key, currentPair.Value);
    }
}
