// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Kenet.Collections.Reactive
{
    public static class ISynchronizedCollectionGenericExtensions
    {
        public static SynchronizedDictionary<TKey, TItem> CreateSynchronizedDictionary<TItem, TKey>(
            this ISynchronizedCollection<TItem> collection,
            Func<TItem, TKey> getItemKey,
            IEqualityComparer<TKey> keyEqualityComparer)
            where TKey : notnull =>
            new(collection, getItemKey, keyEqualityComparer);

        public static SynchronizedDictionary<KeyType, ItemType> CreateSynchronizedDictionary<ItemType, KeyType>(
            this ISynchronizedCollection<ItemType> collection,
            Func<ItemType, KeyType> getItemKey)
            where KeyType : notnull =>
            new(collection, getItemKey);
    }
}
