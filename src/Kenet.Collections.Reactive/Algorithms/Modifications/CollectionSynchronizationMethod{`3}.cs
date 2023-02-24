﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Teronis.Collections.Algorithms.Modifications
{
    public abstract class CollectionSynchronizationMethod<TLeftItem, TRightItem, TComparableItem> : ICollectionSynchronizationMethod<TLeftItem, TRightItem>
        where TComparableItem : notnull
    {
        public CollectionSequenceType SequenceType { get; }

        protected Func<TLeftItem, TComparableItem> GetComparablePartOfLeftItem { get; }
        protected Func<TRightItem, TComparableItem> GetComparablePartOfRightItem { get; }

        protected CollectionSynchronizationMethod(
            CollectionSequenceType sequenceType,
            Func<TLeftItem, TComparableItem> getComparablePartOfLeftItem,
            Func<TRightItem, TComparableItem> getComparablePartOfRightItem)
        {
            SequenceType = sequenceType;
            GetComparablePartOfLeftItem = getComparablePartOfLeftItem ?? throw new ArgumentNullException(nameof(getComparablePartOfLeftItem));
            GetComparablePartOfRightItem = getComparablePartOfRightItem ?? throw new ArgumentNullException(nameof(getComparablePartOfRightItem));
        }

        /// <summary>
        /// Checks arguments when yielding collection modifications.
        /// </summary>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="leftItems"/> is null.</exception>
        protected void CheckArgumentsWhenYieldingCollectionModifications(IEnumerable<TLeftItem> leftItems, [NotNull] ref IEnumerable<TRightItem>? rightItems)
        {
            if (leftItems is null) {
                throw new ArgumentNullException(nameof(leftItems));
            }

            rightItems ??= Enumerable.Empty<TRightItem>();
        }

        /// <summary>
        /// Yields modifications that can transform one collection into another collection.
        /// </summary>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <param name="yieldCapabilities">The yield capabilities, e.g. only insert or only remove.</param>
        /// <returns>The collection modifications.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="leftItems"/> is null.</exception>
        public abstract IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications(
            IEnumerable<TLeftItem> leftItems,
            IEnumerable<TRightItem>? rightItems,
            CollectionModificationYieldCapabilities yieldCapabilities);

        public class Sequential : CollectionSynchronizationMethod<TLeftItem, TRightItem, TComparableItem>
        {
            public IEqualityComparer<TComparableItem> EqualityComparer { get; }

            public Sequential(
                Func<TLeftItem, TComparableItem> getComparablePartOfLeftItem,
                Func<TRightItem, TComparableItem> getComparablePartOfRightItem,
                IEqualityComparer<TComparableItem> equalityComparer)
                : base(CollectionSequenceType.Sequential, getComparablePartOfLeftItem, getComparablePartOfRightItem) =>
                EqualityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));

            public override IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications(
                IEnumerable<TLeftItem> leftItems,
                IEnumerable<TRightItem>? rightItems,
                CollectionModificationYieldCapabilities yieldCapabilities)
            {
                CheckArgumentsWhenYieldingCollectionModifications(leftItems, ref rightItems);

                return EqualityTrailingCollectionModifications.YieldCollectionModifications(
                    leftItems,
                    GetComparablePartOfLeftItem,
                    rightItems,
                    GetComparablePartOfRightItem,
                    EqualityComparer,
                    yieldCapabilities);
            }
        }

        public class Sorted : CollectionSynchronizationMethod<TLeftItem, TRightItem, TComparableItem>
        {
            public IComparer<TComparableItem> Comparer { get; }

            public Sorted(
                Func<TLeftItem, TComparableItem> getComparablePartOfLeftItem,
                Func<TRightItem, TComparableItem> getComparablePartOfRightItem,
                IComparer<TComparableItem> comparer)
                : base(CollectionSequenceType.Sequential, getComparablePartOfLeftItem, getComparablePartOfRightItem) =>
                Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

            public override IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications(
                IEnumerable<TLeftItem> leftItems,
                IEnumerable<TRightItem>? rightItems,
                CollectionModificationYieldCapabilities yieldCapabilities)
            {
                CheckArgumentsWhenYieldingCollectionModifications(leftItems, ref rightItems);

                return SortedCollectionModifications.YieldCollectionModifications(
                    leftItems,
                    GetComparablePartOfLeftItem,
                    rightItems,
                    GetComparablePartOfRightItem,
                    Comparer,
                    yieldCapabilities);
            }
        }
    }
}
