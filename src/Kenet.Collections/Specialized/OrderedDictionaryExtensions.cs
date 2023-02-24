﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Teronis.Utils;

namespace Teronis.Collections.Specialized
{
    public static class OrderedDictionaryExtensions
    {
        public static bool Swap<K, V>(this OrderedDictionary<K, V> source, int fromIndex, int toIndex)
            where K : notnull
        {
            void insertAt(int index, object item) =>
                source.Insert(index, (KeyValuePair<K, V>)item);

            object getAt(int index) => source[index];
            void removeAt(int index) => source.RemoveAt(index);
            return ListUtils.SwapItem(fromIndex, toIndex, insertAt!, getAt, removeAt);
        }
    }
}
