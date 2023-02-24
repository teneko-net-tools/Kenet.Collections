﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Kenet.Collections.Specialized;

namespace Kenet.Collections.Algorithms.Modifications
{
    public static class SortedCollectionModifications
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
        /// <param name="getComparablePartOfLeftItem">The part of left item that is comparable with part of right item.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="getComparablePartOfRightItem">The part of right item that is comparable with part of left item.</param>
        /// <param name="comparer">The comparer to be used to compare comparable parts of left and right item.</param>
        /// <param name="yieldCapabilities">The yieldCapabilities that regulates how <paramref name="leftItems"/> and <paramref name="rightItems"/> are synchronized.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TComparablePart>(
            IEnumerable<TLeftItem> leftItems,
            Func<TLeftItem, TComparablePart> getComparablePartOfLeftItem,
            IEnumerable<TRightItem> rightItems,
            Func<TRightItem, TComparablePart> getComparablePartOfRightItem,
            IComparer<TComparablePart> comparer,
            CollectionModificationYieldCapabilities yieldCapabilities)
        {
            var canInsert = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Insert);
            var canRemove = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Remove);
            var canReplace = yieldCapabilities.HasFlag(CollectionModificationYieldCapabilities.Replace);

            var leftIndexDirectory = new IndexDirectory();

            var leftItemsEnumerator = new IndexPreferredEnumerator<TLeftItem>(leftItems, () => leftIndexDirectory.Count - 1);
            bool leftItemsEnumeratorIsFunctional = leftItemsEnumerator.MoveNext();

            var rightItemsEnumerator = rightItems.GetEnumerator();
            bool rightItemsEnumeratorIsFunctional = rightItemsEnumerator.MoveNext();

            int leftIndexOfLatestSyncedRightItem = -1;

            while (leftItemsEnumeratorIsFunctional || rightItemsEnumeratorIsFunctional) {
                if (!rightItemsEnumeratorIsFunctional) {
                    if (canRemove) {
                        var leftItem = leftItemsEnumerator.Current;
                        yield return CollectionModification.ForRemove<TRightItem, TLeftItem>(leftIndexOfLatestSyncedRightItem + 1, leftItem);
                    } else {
                        leftIndexDirectory.Expand(leftIndexDirectory.Count);
                        leftIndexOfLatestSyncedRightItem++;
                    }

                    leftItemsEnumeratorIsFunctional = leftItemsEnumerator.MoveNext();
                } else if (!leftItemsEnumeratorIsFunctional) {
                    if (canInsert) {
                        var rightItem = rightItemsEnumerator.Current;
                        leftIndexOfLatestSyncedRightItem++;
                        yield return CollectionModification.ForAdd<TRightItem, TLeftItem>(leftIndexOfLatestSyncedRightItem, rightItem);
                        leftIndexDirectory.Expand(leftIndexDirectory.Count);
                    }

                    rightItemsEnumeratorIsFunctional = rightItemsEnumerator.MoveNext();
                } else {
                    var leftItem = leftItemsEnumerator.Current;
                    var rightItem = rightItemsEnumerator.Current;

                    var comparablePartOfLeftItem = getComparablePartOfLeftItem(leftItem);
                    var comparablePartOfRightItem = getComparablePartOfRightItem(rightItem);
                    var comparablePartComparison = comparer.Compare(comparablePartOfLeftItem, comparablePartOfRightItem);

                    if (comparablePartComparison < 0) {
                        if (canRemove) {
                            yield return CollectionModification.ForRemove<TRightItem, TLeftItem>(leftIndexOfLatestSyncedRightItem + 1, leftItem);
                        } else {
                            leftIndexDirectory.Expand(leftIndexDirectory.Count);
                            leftIndexOfLatestSyncedRightItem++;
                        }

                        leftItemsEnumeratorIsFunctional = leftItemsEnumerator.MoveNext();
                    } else if (comparablePartComparison > 0) {
                        if (canInsert) {
                            leftIndexOfLatestSyncedRightItem++;
                            yield return CollectionModification.ForAdd<TRightItem, TLeftItem>(leftIndexOfLatestSyncedRightItem, rightItem);
                            leftIndexDirectory.Expand(leftIndexDirectory.Count);
                        }

                        rightItemsEnumeratorIsFunctional = rightItemsEnumerator.MoveNext();
                    } else {
                        leftIndexOfLatestSyncedRightItem++;

                        if (canReplace) {
                            yield return CollectionModification.ForReplace(
                                leftIndexOfLatestSyncedRightItem,
                                leftItem,
                                rightItem);
                        }

                        leftIndexDirectory.Expand(leftIndexDirectory.Count);
                        leftItemsEnumeratorIsFunctional = leftItemsEnumerator.MoveNext();
                        rightItemsEnumeratorIsFunctional = rightItemsEnumerator.MoveNext();
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
        /// <param name="getComparablePartOfLeftItem">The part of left item that is comparable with part of right item.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="getComparablePartOfRightItem">The part of right item that is comparable with part of left item.</param>
        /// <param name="comparer">The comparer to be used to compare comparable parts of left and right item.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TComparablePart>(
            IEnumerable<TLeftItem> leftItems,
            Func<TLeftItem, TComparablePart> getComparablePartOfLeftItem,
            IEnumerable<TRightItem> rightItems,
            Func<TRightItem, TComparablePart> getComparablePartOfRightItem,
            IComparer<TComparablePart> comparer) =>
            YieldCollectionModifications(
                leftItems,
                getComparablePartOfLeftItem,
                rightItems,
                getComparablePartOfRightItem,
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
        /// <param name="getComparablePartOfLeftItem">The part of left item that is comparable with part of right item.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="getComparablePartOfRightItem">The part of right item that is comparable with part of left item.</param>
        /// <param name="yieldCapabilities">The yieldCapabilities that regulates how <paramref name="leftItems"/> and <paramref name="rightItems"/> are synchronized.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when non-nullable arguments are null.</exception>
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem, TComparablePart>(
            IEnumerable<TLeftItem> leftItems,
            Func<TLeftItem, TComparablePart> getComparablePartOfLeftItem,
            IEnumerable<TRightItem> rightItems,
            Func<TRightItem, TComparablePart> getComparablePartOfRightItem,
            CollectionModificationYieldCapabilities yieldCapabilities) =>
            YieldCollectionModifications(
                leftItems,
                getComparablePartOfLeftItem,
                rightItems,
                getComparablePartOfRightItem,
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
            IEnumerable<TItem> leftItems,
            IEnumerable<TItem> rightItems,
            IComparer<TItem> comparer,
            CollectionModificationYieldCapabilities yieldCapabilities) =>
            YieldCollectionModifications(
                leftItems,
                leftItem => leftItem,
                rightItems,
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
            IEnumerable<TItem> leftItems,
            IEnumerable<TItem> rightItems,
            IComparer<TItem> comparer) =>
            YieldCollectionModifications(
                leftItems,
                leftItem => leftItem,
                rightItems,
                rightItem => rightItem,
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
            IEnumerable<TItem> leftItems,
            IEnumerable<TItem> rightItems,
            CollectionModificationYieldCapabilities yieldCapabilities) =>
            YieldCollectionModifications(
                leftItems,
                leftItem => leftItem,
                rightItems,
                rightItem => rightItem,
                Comparer<TItem>.Default,
                yieldCapabilities);
    }
}
