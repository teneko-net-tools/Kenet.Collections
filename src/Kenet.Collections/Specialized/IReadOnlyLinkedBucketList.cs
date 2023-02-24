﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Teronis.Collections.Generic;

namespace Teronis.Collections.Specialized
{
    public interface IReadOnlyLinkedBucketList<KeyType, out ValueType> : IEnumerable<ValueType>
        where KeyType : notnull
    {
        ICovariantReadOnlyNullableKeyDictionary<KeyType, IReadOnlyLinkedBucketList<KeyType, ValueType>> Buckets { get; }
        int Count { get; }
        IReadOnlyLinkedBucketListNode<KeyType, ValueType>? First { get; }
        bool IsBucket { get; }
        IReadOnlyLinkedBucketListNode<KeyType, ValueType>? Last { get; }
        IReadOnlyLinkedBucketList<KeyType, ValueType> List { get; }

        IReadOnlyLinkedBucketListNode<KeyType, ValueType>? FindFirst(Predicate<ValueType> predicate);
        IReadOnlyLinkedBucketListNode<KeyType, ValueType>? FindLast(Predicate<ValueType> predicate);
    }
}
