﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Kenet.Collections.Generic;

namespace Kenet.Collections.Specialized
{
    internal partial class LinkedBucketListBase<KeyType, ValueType>
    {
        internal class LinkedBucket : LinkedBucketListBase<KeyType, ValueType>
        {
            public override bool IsBucket => true;
            public YetNullable<KeyType> Key { get; }

            internal override NullableKeyDictionary<KeyType, LinkedBucket> buckets => List.buckets;

            public LinkedBucket(LinkedList list, YetNullable<KeyType> key)
                : base(list) =>
                Key = key;

            internal override LinkedBucketListNode<KeyType, ValueType>.LinkedBucketListNodePart GetNodePart(LinkedBucketListNode<KeyType, ValueType> node) =>
                node.BucketPart;

            internal override LinkedBucketListNode<KeyType, ValueType> InsertNodeFirst(LinkedBucketListNode<KeyType, ValueType> node)
            {
                base.InsertNodeFirst(node);
                node.bucket = this;
                return node;
            }

            internal override LinkedBucketListNode<KeyType, ValueType> InsertNodeLast(LinkedBucketListNode<KeyType, ValueType> node)
            {
                base.InsertNodeLast(node);
                node.bucket = this;
                return node;
            }

            private void validateBucket()
            {
                if (list is null) {
                    throw new InvalidOperationException("This bucket is not attached.");
                }
            }

            public override void AddBefore(LinkedBucketListNode<KeyType, ValueType> node, LinkedBucketListNode<KeyType, ValueType> newNode)
            {
                validateBucket();
                list.AddBefore(node, newNode);
            }

            public override LinkedBucketListNode<KeyType, ValueType> AddBefore(LinkedBucketListNode<KeyType, ValueType> node, ValueType value)
            {
                validateBucket();
                return list.AddBefore(node, value);
            }

            public override void AddAfter(LinkedBucketListNode<KeyType, ValueType> node, LinkedBucketListNode<KeyType, ValueType> newNode)
            {
                validateBucket();
                list.AddAfter(node, newNode);
            }

            public override LinkedBucketListNode<KeyType, ValueType> AddAfter(LinkedBucketListNode<KeyType, ValueType> node, ValueType value)
            {
                validateBucket();
                return list.AddAfter(node, value);
            }

            public override void AddFirst(YetNullable<KeyType> key, LinkedBucketListNode<KeyType, ValueType> node)
            {
                validateBucket();
                list.AddFirst(key, node);
            }

            public override LinkedBucketListNode<KeyType, ValueType> AddFirst(YetNullable<KeyType> key, ValueType value)
            {
                validateBucket();
                return list.AddFirst(key, value);
            }

            public override void AddLast(YetNullable<KeyType> key, LinkedBucketListNode<KeyType, ValueType> node)
            {
                validateBucket();
                list.AddLast(key, node);
            }

            public override LinkedBucketListNode<KeyType, ValueType> AddLast(YetNullable<KeyType> key, ValueType value)
            {
                validateBucket();
                return list.AddLast(key, value);
            }

            internal void Clear(bool invokedFromList)
            {
                if (invokedFromList) {
                    base.Clear();
                } else {
                    if (head is null) {
                        return;
                    }

                    var node = head;

                    do {
                        var temp = node;
                        node = node.BucketPart.Next;
                        Remove(temp);
                    } while (node != null);

                    head = null;
                    count = 0;
        }
            }

            public override void Clear()
            {
                validateBucket();
                Clear(false);
            }

            internal void Invalidate() =>
                list = null!;
        }
    }
}
