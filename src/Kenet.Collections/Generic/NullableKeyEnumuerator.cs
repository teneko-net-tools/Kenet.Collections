// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Kenet.Collections.Generic;

namespace Kenet.Collections.Generic
{
    public class NullableKeyEnumuerator<KeyType, ValueType> : IEnumerator<KeyValuePair<YetNullable<KeyType>, ValueType>>
        where KeyType : notnull
    {
        [MaybeNull]
        public KeyValuePair<YetNullable<KeyType>, ValueType> Current { get; private set; }

        private bool _movedOverNull;
        private readonly IEnumerator<KeyValuePair<KeyType, ValueType>> _enumerator;
        private readonly KeyValuePair<YetNullable<KeyType>, ValueType>? _nullableKeyValuePair;

        object? IEnumerator.Current => Current;

        public NullableKeyEnumuerator(IEnumerator<KeyValuePair<KeyType, ValueType>> enumerator, KeyValuePair<YetNullable<KeyType>, ValueType>? nullableKeyValuePair)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _movedOverNull = nullableKeyValuePair is null;
            _nullableKeyValuePair = nullableKeyValuePair;
        }

        public void Dispose() =>
            _enumerator.Dispose();

        public bool MoveNext()
        {
            if (!_movedOverNull) {
                Current = _nullableKeyValuePair!.Value;
                _movedOverNull = true;
                return true;
            }

            if (_enumerator.MoveNext()) {
                var (key, value) = _enumerator.Current;
                var nullableKey = new YetNullable<KeyType>(key);
                Current = new KeyValuePair<YetNullable<KeyType>, ValueType>(nullableKey, value);
                return true;
            }

            return false;
        }

        public void Reset() =>
            _enumerator.Reset();
    }
}
