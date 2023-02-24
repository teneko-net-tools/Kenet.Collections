// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Specialized
{
    public static class LinkedBucketListUtils
    {
        public static IEnumerable<LinkedBucketListNode<KeyType, ItemType>> YieldNodesReversed<KeyType, ItemType>(LinkedBucketListNode<KeyType, ItemType> node)
            where KeyType : notnull
        {
            if (node is null) {
                yield break;
            }

            var currentNode = node;

            while (currentNode != null) {
                yield return currentNode;
                currentNode = currentNode.ListPart.Previous;
            }
        }
    }
}
