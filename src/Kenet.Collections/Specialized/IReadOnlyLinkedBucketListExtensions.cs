﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Specialized
{
    public static class IReadOnlyLinkedBucketListExtensions
    {
        private static bool ValuesAreEqual<ValueType>(ValueType x, ValueType y, IEqualityComparer<ValueType> equalityComparer) =>
            equalityComparer.Equals(x, y);

        public static IReadOnlyLinkedBucketListNode<KeyType, ValueType>? FindFirstReadOnly<KeyType, ValueType>(
            this IReadOnlyLinkedBucketList<KeyType, ValueType> bucketList,
            ValueType value,
            IEqualityComparer<ValueType>? equalityComparer)
            where KeyType : notnull
        {
            equalityComparer ??= EqualityComparer<ValueType>.Default;
            return bucketList.FindFirst(otherValue => ValuesAreEqual(value, otherValue, equalityComparer));
        }

        public static IReadOnlyLinkedBucketListNode<KeyType, ValueType>? FindFirstReadOnly<KeyType, ValueType>(
            this IReadOnlyLinkedBucketList<KeyType, ValueType> bucketList,
            ValueType value)
            where KeyType : notnull =>
            FindFirstReadOnly(bucketList, value, equalityComparer: null);

        public static IReadOnlyLinkedBucketListNode<KeyType, ValueType>? FindLastReadOnly<KeyType, ValueType>(
            this IReadOnlyLinkedBucketList<KeyType, ValueType> bucketList,
            ValueType value,
            IEqualityComparer<ValueType>? equalityComparer)
            where KeyType : notnull
        {
            equalityComparer ??= EqualityComparer<ValueType>.Default;
            return bucketList.FindLast(otherValue => ValuesAreEqual(value, otherValue, equalityComparer));
        }

        public static IReadOnlyLinkedBucketListNode<KeyType, ValueType>? FindLastReadOnly<KeyType, ValueType>(
            this IReadOnlyLinkedBucketList<KeyType, ValueType> bucketList,
            ValueType value)
            where KeyType : notnull =>
            FindLastReadOnly(bucketList, value, equalityComparer: null);
    }
}
