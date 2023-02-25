// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Kenet.Collections.Specialized;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    /// <summary>
    /// The algorithm creates modifications that can transform one collection into another collection.
    /// If equal left and right items are appearing the right items are going to act as markers. The
    /// right items before markers are the children of the markers, to assure that the these right
    /// items are inserted before their individual marker.
    /// </summary>
    public static class EqualityTrailingCollectionSynchronizationMethod
    {
        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftComparands"/>.
        /// The more the collection is synchronized in an orderly way, the more efficient the algorithm is.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TLeftItem">The type of left items.</typeparam>
        /// <typeparam name="TRightItem">The type of right items.</typeparam>
        /// <typeparam name="TLeftComparand">The type of left items.</typeparam>
        /// <typeparam name="TRightComparand">The type of right items.</typeparam>
        /// <typeparam name="TComparablePart">The type of the comparable part of left item and right item.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="leftComparands"></param>
        /// <param name="rightComparands"></param>
        /// <param name="equalityComparer">The equality comparer to be used to compare comparable parts.</param>
        /// <param name="yieldCapabilities">The yield capabilities, e.g. only insert or only remove.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        private static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModificationsImpl<TLeftItem, TRightItem, TLeftComparand, TRightComparand, TComparablePart>(
            IEnumerator<TLeftItem> leftItems,
            IEnumerator<TRightItem> rightItems,
            IEnumerator<TLeftComparand> leftComparands,
            IEnumerator<TRightComparand> rightComparands,
            IEqualityComparer<TComparablePart>? equalityComparer,
            CollectionModificationYieldCapabilities yieldCapabilities)
            where TLeftComparand : TComparablePart
            where TRightComparand : TComparablePart
            where TComparablePart : notnull
        {
            static CollectionModification<TRightItem, TLeftItem> CreateReplaceModification(
                LinkedBucketListNode<TComparablePart, LeftItemContainer<TLeftItem>> leftComparandNode,
                LinkedBucketListNode<TComparablePart, RightItemContainer<TLeftItem, TRightItem>> rightComparandNode) =>
                CollectionModification.ForReplace(
                    leftComparandNode.Value.IndexEntry,
                    leftComparandNode.Value.Item,
                    rightComparandNode.Value.Item);

            static CollectionModification<TRightItem, TLeftItem> CreateMoveModification(
                LinkedBucketListNode<TComparablePart, LeftItemContainer<TLeftItem>> leftComparandNode,
                int leftComparandMoveToIndex,
                LinkedBucketListNode<TComparablePart, RightItemContainer<TLeftItem, TRightItem>> rightComparandNode) =>
                CollectionModification.ForMove<TRightItem, TLeftItem>(
                    leftComparandNode.Value.IndexEntry,
                    leftComparandNode.Value.Item,
                    leftComparandMoveToIndex);

            equalityComparer ??= EqualityComparer<TComparablePart>.Default;

            var canInsert = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Insert);
            var canRemove = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Remove);
            var canReplace = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Replace);
            var canMove = canInsert && canRemove;

            var leftIndexDirectory = new IndexDirectory();

            var leftItemEnumeratorIndexBased = leftItems as IIndexBasedEnumerator<TLeftItem>;
            var isLeftItemEnumeratorIndexBased = leftItemEnumeratorIndexBased is IIndexBasedEnumerator<TLeftItem>;

            var leftComparandEnumeratorIndexBased = leftComparands as IIndexBasedEnumerator<TLeftItem>;
            var isLeftComparandEnumeratorIndexBased = leftComparandEnumeratorIndexBased is not null;

            var leftComparandsNodes = new LinkedBucketList<TComparablePart, LeftItemContainer<TLeftItem>>(equalityComparer);
            var leftEnumeratorsAreEqual = ReferenceEquals(leftItems, leftComparands);

            bool MoveLeftEnumerators() =>
                (isLeftItemEnumeratorIndexBased ? leftItemEnumeratorIndexBased!.MoveTo(leftIndexDirectory.Count) : leftItems.MoveNext())
                && (leftEnumeratorsAreEqual || (isLeftComparandEnumeratorIndexBased ? leftComparandEnumeratorIndexBased!.MoveTo(leftIndexDirectory.Count) : leftComparands.MoveNext()));

            var leftEnumeratorsAreFunctional = MoveLeftEnumerators();

            var rightComparandIndexNext = 0;
            var rightComparandNodes = new LinkedBucketList<TComparablePart, RightItemContainer<TLeftItem, TRightItem>>(equalityComparer);
            var rightEnumeratorsAreEqual = ReferenceEquals(rightItems, rightComparands);

            bool MoveRightEnumerators() =>
                rightItems.MoveNext()
                && (rightEnumeratorsAreEqual || rightComparands.MoveNext());

            var rightEnumeratorsAreFunctional = MoveRightEnumerators();

            var leftIndexOfLatestSyncedRightItem = new IndexDirectoryEntry(-1, IndexDirectoryEntryMode.Floating);

            void SetLeftIndexOfLatestSyncedRightItem(int newIndex)
            {
                if (newIndex > leftIndexOfLatestSyncedRightItem.Index) {
                    leftIndexDirectory.ReplaceEntry(leftIndexOfLatestSyncedRightItem, newIndex);
                }
            }

            while (leftEnumeratorsAreFunctional || rightEnumeratorsAreFunctional) {
                LinkedBucketListNode<TComparablePart, LeftItemContainer<TLeftItem>>? leftComparandNodeLast;
                LinkedBucketListNode<TComparablePart, RightItemContainer<TLeftItem, TRightItem>>? rightComparandNodeLast;

                if (rightEnumeratorsAreFunctional) {
                    var rightComparand = rightComparands.Current;

                    rightComparandNodeLast = rightComparandNodes.AddLast(rightComparand, new RightItemContainer<TLeftItem, TRightItem>(rightItems.Current, rightComparandIndexNext));
                    rightComparandIndexNext++;
                } else {
                    rightComparandNodeLast = null;
                }

                var rightComparandNodeLastBucketFirstNode = rightComparandNodeLast?.Bucket!.First;

                /* Is the first node of bucket of right node anywhere on left side? */
                if (!(rightComparandNodeLastBucketFirstNode is null) && leftComparandsNodes.TryGetBucket(rightComparandNodeLastBucketFirstNode!.Key, out var leftComparandBucket)) {
                    var leftComparandNode = leftComparandBucket.First!;

                    if (canReplace) {
                        yield return CreateReplaceModification(leftComparandNode, rightComparandNodeLastBucketFirstNode);
                    }

                    int leftComparandMoveToIndex;

                    if (leftComparandNode.Value.IndexEntry > leftIndexOfLatestSyncedRightItem.Index) {
                        // We do not need to move, because the item has not 
                        // exceeded the index of latest synced right item.
                        leftComparandMoveToIndex = leftComparandNode.Value.IndexEntry;
                    } else {
                        // The index where it would be inserted when it is removed.
                        leftComparandMoveToIndex = leftIndexOfLatestSyncedRightItem.Index;
                    }

                    LinkedBucketListNode<TComparablePart, RightItemContainer<TLeftItem, TRightItem>>? rightComparandNodeLastBucketFirstNodeListPreviousNode;

                    {
                        if (canMove && leftComparandNode.Value.IndexEntry != leftComparandMoveToIndex) {
                            var moveModification = CreateMoveModification(leftComparandNode, leftComparandMoveToIndex, rightComparandNodeLastBucketFirstNode);
                            yield return moveModification;
                            leftIndexDirectory.Move(moveModification.OldIndex, moveModification.NewIndex);
                        }

                        rightComparandNodeLastBucketFirstNodeListPreviousNode = rightComparandNodeLastBucketFirstNode.ListPart.Previous;
                        rightComparandNodeLastBucketFirstNode.Bucket?.Remove(rightComparandNodeLastBucketFirstNode);

                        leftComparandNode.Bucket!.Remove(leftComparandNode);
                    }

                    var currentLeftItemNodeAsParentForPreviousRightItemNodes = leftComparandNode;

                    while (!(rightComparandNodeLastBucketFirstNodeListPreviousNode is null)) {
                        // Do not set parent after it has been already set.
                        if (rightComparandNodeLastBucketFirstNodeListPreviousNode.Value.Parent is null) {
                            // We cannot add right item blindly to left side, because we do not know if exact this node will
                            // appear on left side. So we tell current previous node about a left node that is definitely below him.
                            rightComparandNodeLastBucketFirstNodeListPreviousNode.Value.Parent = currentLeftItemNodeAsParentForPreviousRightItemNodes.Value;
                        }

                        var tempPrevious = rightComparandNodeLastBucketFirstNodeListPreviousNode.ListPart.Previous;

                        if (tempPrevious is null || !(tempPrevious.Value.Parent is null)) {
                            rightComparandNodeLastBucketFirstNodeListPreviousNode = null;
                        } else {
                            rightComparandNodeLastBucketFirstNodeListPreviousNode = tempPrevious;
                        }
                    }

                    // Let's refresh index after left item may have been moved.
                    SetLeftIndexOfLatestSyncedRightItem(leftComparandNode.Value.IndexEntry);
                }

                if (leftEnumeratorsAreFunctional) {
                    var leftComparand = leftComparands.Current;

                    var nextLeftIndex = leftIndexDirectory.Count;

                    leftComparandNodeLast = leftComparandsNodes.AddLast(leftComparand, new LeftItemContainer<TLeftItem>(leftItems.Current, leftIndexDirectory.Add(nextLeftIndex)));
                } else {
                    leftComparandNodeLast = null;
                }

                var leftComparandNodeLastBucketFirstNode = leftComparandNodeLast?.Bucket?.First;

                /* Is the first node of bucket of left node anywhere on right side? */
                if (!(leftComparandNodeLastBucketFirstNode is null) && rightComparandNodes.TryGetBucket(leftComparandNodeLastBucketFirstNode.Key, out var rightComparandbucket)) {
                    var rightComparandNode = rightComparandbucket.First!;

                    if (canReplace) {
                        yield return CreateReplaceModification(leftComparandNodeLastBucketFirstNode, rightComparandNode);
                    }

                    if (canMove && !(rightComparandNode.Value.Parent is null)) {
                        var moveModification = CreateMoveModification(
                            leftComparandNodeLastBucketFirstNode,
                            rightComparandNode.Value.Parent.IndexEntry,
                            rightComparandNode);

                        yield return moveModification;
                        leftIndexDirectory.Move(moveModification.OldIndex, moveModification.NewIndex);
                    }

                    var rightComparandNodePrevious = rightComparandNode.ListPart.Previous;

                    while (!(rightComparandNodePrevious is null)
                        && (rightComparandNode.Value.Parent is null && rightComparandNodePrevious.Value.Parent is null
                            || !(rightComparandNode.Value.Parent is null) && ReferenceEquals(rightComparandNodePrevious.Value.Parent, rightComparandNode.Value.Parent))) {
                        rightComparandNodePrevious.Value.Parent = leftComparandNodeLastBucketFirstNode.Value;
                        rightComparandNodePrevious = rightComparandNodePrevious.ListPart.Previous;
                    }

                    leftComparandNodeLastBucketFirstNode.Bucket?.Remove(leftComparandNodeLastBucketFirstNode);
                    rightComparandNode.Bucket?.Remove(rightComparandNode);
                    SetLeftIndexOfLatestSyncedRightItem(leftComparandNodeLastBucketFirstNode.Value.IndexEntry);
                }

                leftEnumeratorsAreFunctional = leftEnumeratorsAreFunctional && MoveLeftEnumerators();
                rightEnumeratorsAreFunctional = rightEnumeratorsAreFunctional && MoveRightEnumerators();
            }

            // ISSUE: check if left index directory count can replace below line
            //var leftComparandsLength = leftComparandEnumerator.CurrentLength;
            var leftComparandsLength = leftIndexDirectory.Count;

            if (canRemove && !(leftComparandsNodes.Last is null)) {
                foreach (var leftComparandNode in LinkedBucketListUtils.YieldNodesReversed(leftComparandsNodes.Last)) {
                    var removeModification = CollectionModification.ForRemove<TRightItem, TLeftItem>(leftComparandNode.Value.IndexEntry, leftComparandNode.Value.Item);
                    yield return removeModification;

                    leftIndexDirectory.Remove(removeModification.OldIndex);
                    leftComparandsLength--;
                }
            }

            if (canInsert && !(rightComparandNodes.First is null)) {
                foreach (var rightComparandNode in rightComparandNodes) {
                    int insertItemTo;

                    if (rightComparandNode.Parent is null) {
                        insertItemTo = leftComparandsLength;
                    } else {
                        insertItemTo = rightComparandNode.Parent.IndexEntry;
                    }

                    var addModification = CollectionModification.ForAdd<TRightItem, TLeftItem>(insertItemTo, rightComparandNode.Item);
                    yield return addModification;

                    leftIndexDirectory.Insert(insertItemTo);
                    leftComparandsLength++;
                }
            }
        }

        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// The more the collection is synchronized in an orderly way, the more efficient the algorithm is.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TLeftItem">The type of left items.</typeparam>
        /// <typeparam name="TRightItem">The type of right items.</typeparam>
        /// <typeparam name="TComparablePart">The type of the comparable part of left item and right item.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="leftComparands"></param>
        /// <param name="rightComparands"></param>
        /// <param name="equalityComparer">The equality comparer to be used to compare comparable parts.</param>
        /// <param name="yieldCapabilities">The yield capabilities, e.g. only insert or only remove.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TLeftComparand, TRightComparand, TComparablePart>(
            IEnumerator<TLeftItem> leftItems,
            IEnumerator<TRightItem> rightItems,
            IEnumerator<TLeftComparand> leftComparands,
            IEnumerator<TRightComparand> rightComparands,
            IEqualityComparer<TComparablePart>? equalityComparer,
            CollectionModificationYieldCapabilities yieldCapabilities)
            where TLeftComparand : TComparablePart
            where TRightComparand : TComparablePart
            where TComparablePart : notnull =>
            YieldCollectionModificationsImpl(
                leftItems,
                rightItems,
                leftComparands,
                rightComparands,
                equalityComparer,
                yieldCapabilities);


        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// The more the collection is synchronized in an orderly way, the more efficient the algorithm is.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TLeftItem">The type of left items.</typeparam>
        /// <typeparam name="TRightItem">The type of right items.</typeparam>
        /// <typeparam name="TComparablePart">The type of the comparable part of left item and right item.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="leftComparands"></param>
        /// <param name="rightComparands"></param>
        /// <param name="equalityComparer">The equality comparer to be used to compare comparable parts.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TLeftComparand, TRightComparand, TComparablePart>(
        IEnumerator<TLeftItem> leftItems,
        IEnumerator<TRightItem> rightItems,
        IEnumerator<TLeftComparand> leftComparands,
        IEnumerator<TRightComparand> rightComparands,
        IEqualityComparer<TComparablePart>? equalityComparer)
        where TLeftComparand : TComparablePart
        where TRightComparand : TComparablePart
        where TComparablePart : notnull =>
        YieldCollectionModifications(
            leftItems,
            rightItems,
            leftComparands,
            rightComparands,
            equalityComparer,
            CollectionModificationYieldCapabilities.All);

        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// The more the collection is synchronized in an orderly way, the more efficient the algorithm is.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TLeftItem">The type of left items.</typeparam>
        /// <typeparam name="TRightItem">The type of right items.</typeparam>
        /// <typeparam name="TComparablePart">The type of the comparable part of left item and right item.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="leftComparands"></param>
        /// <param name="rightComparands"></param>
        /// <param name="yieldCapabilities">The yield capabilities, e.g. only insert or only remove.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TLeftComparand, TRightComparand, TComparablePart>(
            IEnumerator<TLeftItem> leftItems,
            IEnumerator<TRightItem> rightItems,
            IEnumerator<TLeftComparand> leftComparands,
            IEnumerator<TRightComparand> rightComparands,
            CollectionModificationYieldCapabilities yieldCapabilities)
            where TLeftComparand : TComparablePart
            where TRightComparand : TComparablePart
            where TComparablePart : notnull =>
            YieldCollectionModifications(
                leftItems,
                rightItems,
                leftComparands,
                rightComparands,
                EqualityComparer<TComparablePart>.Default,
                yieldCapabilities);

        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// The more the collection is synchronized in an orderly way, the more efficient the algorithm is.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TLeftItem">The type of left items.</typeparam>
        /// <typeparam name="TRightItem">The type of right items.</typeparam>
        /// <typeparam name="TComparablePart">The type of the comparable part of left item and right item.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="leftComparands"></param>
        /// <param name="rightComparands"></param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TLeftComparand, TRightComparand, TComparablePart>(
            IEnumerator<TLeftItem> leftItems,
            IEnumerator<TRightItem> rightItems,
            IEnumerator<TLeftComparand> leftComparands,
            IEnumerator<TRightComparand> rightComparands)
            where TLeftComparand : TComparablePart
            where TRightComparand : TComparablePart
            where TComparablePart : notnull =>
            YieldCollectionModifications(
                leftItems,
                rightItems,
                leftComparands,
                rightComparands,
                EqualityComparer<TComparablePart>.Default,
                CollectionModificationYieldCapabilities.All);

        private class LeftItemContainer<TLeftItem>
        {
            public TLeftItem Item { get; }
            public IndexDirectoryEntry IndexEntry { get; }

            public LeftItemContainer(TLeftItem item, IndexDirectoryEntry indexEntry)
            {
                Item = item;
                IndexEntry = indexEntry ?? throw new ArgumentNullException(nameof(indexEntry));
            }
        }

        private class RightItemContainer<TLeftItem, TRightItem>
        {
            public TRightItem Item { get; }
            /// <summary>
            /// Not null means that this right item should be BEFORE parent.
            /// </summary>
            public LeftItemContainer<TLeftItem>? Parent { get; set; }
            public int Index { get; }

            public RightItemContainer(TRightItem item, int index)
            {
                Item = item;
                Index = index;
            }
        }
    }
}
