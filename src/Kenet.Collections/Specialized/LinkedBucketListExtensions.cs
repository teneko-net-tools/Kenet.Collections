﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Teronis.Collections.Specialized
{
    public static class LinkedBucketListExtensions
    {
        public static LinkedBucketListNode<KeyType, ValueType> AddFirst<KeyType, ValueType>(this ILinkedBucketList<KeyType, ValueType> list, ValueType value)
            where KeyType : notnull
            where ValueType : KeyType =>
            list.AddFirst(value, value);

        public static LinkedBucketListNode<KeyType, ValueType> AddLast<KeyType, ValueType>(this ILinkedBucketList<KeyType, ValueType> list, ValueType value)
            where KeyType : notnull
            where ValueType : KeyType =>
            list.AddLast(value, value);

        public static IEnumerable<LinkedBucketListNode<KeyType, ValueType>> GetEnumerableButReversed<KeyType, ValueType>(this LinkedBucketListNode<KeyType, ValueType>.LinkedBucketListNodePart? node)
            where KeyType : notnull
            where ValueType : KeyType
        {
            if (node is null) {
                yield break;
            }

            do {
                yield return node.Owner;
                node = node.PreviousPart;
            } while (!(node is null));
        }

        public static void Add<KeyType, ValueType>(this LinkedBucketList<KeyType, ValueType> list, KeyType key, ValueType value)
            where KeyType : notnull =>
            list.AddLast(key, value);
    }
}
