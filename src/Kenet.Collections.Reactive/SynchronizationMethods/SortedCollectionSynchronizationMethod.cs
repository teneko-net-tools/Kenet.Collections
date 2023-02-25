// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Kenet.Collections.Specialized;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    public static class SortedCollectionSynchronizationMethod
    {
        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// Assumes <paramref name="leftItems"/> and <paramref name="rightItems"/> to be sorted by that order you specify by <paramref name="collectionOrder"/>.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TLeftItem">The type of left items.</typeparam>
        /// <typeparam name="TRightItem">The type of right items.</typeparam>
        /// <typeparam name="TComparablePart">The type of the comparable part of left item and right item.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="leftComparablePartProvider">The part of left item that is comparable with part of right item.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="rightComparablePartProvider">The part of right item that is comparable with part of left item.</param>
        /// <param name="comparer">The comparer to be used to compare comparable parts of left and right item.</param>
        /// <param name="yieldCapabilities">The yieldCapabilities that regulates how <paramref name="leftItems"/> and <paramref name="rightItems"/> are synchronized.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TComparablePart>(
            IEnumerator<TLeftItem> leftItems,
            IEnumerator<TRightItem> rightItems,
            Func<TLeftItem, TComparablePart> leftComparablePartProvider,
            Func<TRightItem, TComparablePart> rightComparablePartProvider,
            IComparer<TComparablePart> comparer,
            CollectionModificationYieldCapabilities yieldCapabilities)
        {
            var canInsert = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Insert);
            var canRemove = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Remove);
            var canReplace = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Replace);

            var leftIndexDirectory = new IndexDirectory();
            var leftItemEnumeratorIndexBased = leftItems as IIndexBasedEnumerator<TLeftItem>;
            var isLeftItemEnumeratorIndexBased = leftItemEnumeratorIndexBased is IIndexBasedEnumerator<TLeftItem>;
            bool MoveLeftEnumerators() => isLeftItemEnumeratorIndexBased ? leftItemEnumeratorIndexBased!.MoveTo(leftIndexDirectory.Count) : leftItems.MoveNext();
            var leftItemsEnumeratorIsFunctional = MoveLeftEnumerators();

            bool MoveRightEnumerators() => rightItems.MoveNext();
            var rightItemsEnumeratorIsFunctional = MoveRightEnumerators();

            var leftIndexOfLatestSyncedRightItem = -1;

            while (leftItemsEnumeratorIsFunctional || rightItemsEnumeratorIsFunctional) {
                if (!rightItemsEnumeratorIsFunctional) {
                    if (canRemove) {
                        var leftItem = leftItems.Current;
                        yield return CollectionModification.ForRemove<TRightItem, TLeftItem>(leftIndexOfLatestSyncedRightItem + 1, leftItem);
                    } else {
                        leftIndexDirectory.Expand(leftIndexDirectory.Count);
                        leftIndexOfLatestSyncedRightItem++;
                    }

                    leftItemsEnumeratorIsFunctional = MoveLeftEnumerators();
                } else if (!leftItemsEnumeratorIsFunctional) {
                    if (canInsert) {
                        var rightItem = rightItems.Current;
                        leftIndexOfLatestSyncedRightItem++;
                        yield return CollectionModification.ForAdd<TRightItem, TLeftItem>(leftIndexOfLatestSyncedRightItem, rightItem);
                        leftIndexDirectory.Expand(leftIndexDirectory.Count);
                    }

                    rightItemsEnumeratorIsFunctional = MoveRightEnumerators();
                } else {
                    var leftItem = leftItems.Current;
                    var rightItem = rightItems.Current;

                    var comparablePartOfLeftItem = leftComparablePartProvider(leftItem);
                    var comparablePartOfRightItem = rightComparablePartProvider(rightItem);
                    var comparablePartComparison = comparer.Compare(comparablePartOfLeftItem, comparablePartOfRightItem);

                    if (comparablePartComparison < 0) {
                        if (canRemove) {
                            yield return CollectionModification.ForRemove<TRightItem, TLeftItem>(leftIndexOfLatestSyncedRightItem + 1, leftItem);
                        } else {
                            leftIndexDirectory.Expand(leftIndexDirectory.Count);
                            leftIndexOfLatestSyncedRightItem++;
                        }

                        leftItemsEnumeratorIsFunctional = MoveLeftEnumerators();
                    } else if (comparablePartComparison > 0) {
                        if (canInsert) {
                            leftIndexOfLatestSyncedRightItem++;
                            yield return CollectionModification.ForAdd<TRightItem, TLeftItem>(leftIndexOfLatestSyncedRightItem, rightItem);
                            leftIndexDirectory.Expand(leftIndexDirectory.Count);
                        }

                        rightItemsEnumeratorIsFunctional = MoveRightEnumerators();
                    } else {
                        leftIndexOfLatestSyncedRightItem++;

                        if (canReplace) {
                            yield return CollectionModification.ForReplace(
                                leftIndexOfLatestSyncedRightItem,
                                leftItem,
                                rightItem);
                        }

                        leftIndexDirectory.Expand(leftIndexDirectory.Count);
                        leftItemsEnumeratorIsFunctional = MoveLeftEnumerators();
                        rightItemsEnumeratorIsFunctional = MoveRightEnumerators();
                    }
                }
            }
        }

        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// Assumes <paramref name="leftItems"/> and <paramref name="rightItems"/> to be sorted by that order you specify by <paramref name="collectionOrder"/>.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TLeftItem">The type of left items.</typeparam>
        /// <typeparam name="TRightItem">The type of right items.</typeparam>
        /// <typeparam name="TComparablePart">The type of the comparable part of left item and right item.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="leftComparablePartProvider">The part of left item that is comparable with part of right item.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="rightComparablePartProvider">The part of right item that is comparable with part of left item.</param>
        /// <param name="comparer">The comparer to be used to compare comparable parts of left and right item.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TComparablePart>(
            IEnumerator<TLeftItem> leftItems,
            IEnumerator<TRightItem> rightItems,
            Func<TLeftItem, TComparablePart> leftComparablePartProvider,
            Func<TRightItem, TComparablePart> rightComparablePartProvider,
            IComparer<TComparablePart> comparer) =>
            YieldCollectionModifications(
                leftItems,
                rightItems,
                leftComparablePartProvider,
                rightComparablePartProvider,
                comparer,
                CollectionModificationYieldCapabilities.All);

        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// Assumes <paramref name="leftItems"/> and <paramref name="rightItems"/> to be sorted by that order you specify by <paramref name="collectionOrder"/>.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TLeftItem">The type of left items.</typeparam>
        /// <typeparam name="TRightItem">The type of right items.</typeparam>
        /// <typeparam name="TComparablePart">The type of the comparable part of left item and right item.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="leftComparablePartProvider">The part of left item that is comparable with part of right item.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="rightComparablePartProvider">The part of right item that is comparable with part of left item.</param>
        /// <param name="yieldCapabilities">The yieldCapabilities that regulates how <paramref name="leftItems"/> and <paramref name="rightItems"/> are synchronized.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TComparablePart>(
            IEnumerator<TLeftItem> leftItems,
            IEnumerator<TRightItem> rightItems,
            Func<TLeftItem, TComparablePart> leftComparablePartProvider,
            Func<TRightItem, TComparablePart> rightComparablePartProvider,
            CollectionModificationYieldCapabilities yieldCapabilities) =>
            YieldCollectionModifications(
                leftItems,
                rightItems,
                leftComparablePartProvider,
                rightComparablePartProvider,
                Comparer<TComparablePart>.Default,
                yieldCapabilities);

        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// Assumes <paramref name="leftItems"/> and <paramref name="rightItems"/> to be sorted by that order you specify by <paramref name="collectionOrder"/>.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="comparer">The comparer to be used to compare comparable parts of left and right item.</param>
        /// <param name="yieldCapabilities">The yieldCapabilities that regulates how <paramref name="leftItems"/> and <paramref name="rightItems"/> are synchronized.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TItem, TItem>> YieldCollectionModifications<TItem>(
            IEnumerator<TItem> leftItems,
            IEnumerator<TItem> rightItems,
            IComparer<TItem> comparer,
            CollectionModificationYieldCapabilities yieldCapabilities) =>
            YieldCollectionModifications(
                leftItems,
                rightItems,
                leftItem => leftItem,
                rightItem => rightItem,
                comparer: comparer,
                yieldCapabilities: yieldCapabilities);

        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// Assumes <paramref name="leftItems"/> and <paramref name="rightItems"/> to be sorted by that order you specify by <paramref name="collectionOrder"/>.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="comparer">The comparer to be used to compare comparable parts of left and right item.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TItem, TItem>> YieldCollectionModifications<TItem>(
            IEnumerator<TItem> leftItems,
            IEnumerator<TItem> rightItems,
            IComparer<TItem> comparer) =>
            YieldCollectionModifications(
                leftItems,
                rightItems,
                static leftItem => leftItem,
                static rightItem => rightItem,
                comparer,
                CollectionModificationYieldCapabilities.All);

        /// <summary>
        /// The algorithm creates modifications that can transform one collection into another collection.
        /// The collection modifications may be used to transform <paramref name="leftItems"/>.
        /// Assumes <paramref name="leftItems"/> and <paramref name="rightItems"/> to be sorted by that order you specify by <paramref name="collectionOrder"/>.
        /// Duplications are allowed but take into account that duplications are yielded as they are appearing.
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="yieldCapabilities">The yieldCapabilities that regulates how <paramref name="leftItems"/> and <paramref name="rightItems"/> are synchronized.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TItem, TItem>> YieldCollectionModifications<TItem>(
            IEnumerator<TItem> leftItems,
            IEnumerator<TItem> rightItems,
            CollectionModificationYieldCapabilities yieldCapabilities) =>
            YieldCollectionModifications(
                leftItems,
                rightItems,
                static leftItem => leftItem,
                static rightItem => rightItem,
                Comparer<TItem>.Default,
                yieldCapabilities);
    }
}
