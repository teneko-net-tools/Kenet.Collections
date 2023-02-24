﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Kenet.Collections.Specialized
{
    public interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IOrderedDictionary, IReadOnlyDictionary<TKey, TValue>
        where TKey : notnull
    {
        new int Count { get; }
        new ICollection<TKey> Keys { get; }
        new ICollection<TValue> Values { get; }

        new KeyValuePair<TKey, TValue> this[int index] { get; set; }
        new TValue this[TKey key] { get; set; }

        new void Add(TKey key, TValue value);
        new void Clear();
        void Insert(int index, TKey key, TValue value);
        int IndexOf(TKey key);
        bool ContainsValue(TValue value);
        bool ContainsValue(TValue value, IEqualityComparer<TValue> comparer);
        new bool ContainsKey(TKey key);
        new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
        new bool Remove(TKey key);
        new void RemoveAt(int index);
        new bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
        TValue GetValue(TKey key);
        void SetValue(TKey key, TValue value);
        KeyValuePair<TKey, TValue> GetItem(int index);
        void SetItem(int index, TValue value);
    }
}
