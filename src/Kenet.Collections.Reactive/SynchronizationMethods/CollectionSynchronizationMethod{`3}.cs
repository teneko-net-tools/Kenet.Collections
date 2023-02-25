// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    public abstract class CollectionSynchronizationMethod<TLeftItem, TRightItem, TComparableItem> : ICollectionSynchronizationMethod<TLeftItem, TRightItem>
        where TComparableItem : notnull
    {
        public CollectionSequenceType SequenceType { get; }

        protected Func<TLeftItem, TComparableItem> LeftComparablePartProvider { get; }
        protected Func<TRightItem, TComparableItem> RightComparablePartProvider { get; }

        protected CollectionSynchronizationMethod(
            CollectionSequenceType sequenceType,
            Func<TLeftItem, TComparableItem> leftItemComparablePartGetter,
            Func<TRightItem, TComparableItem> rightItemComparablePartGetter)
        {
            SequenceType = sequenceType;
            LeftComparablePartProvider = leftItemComparablePartGetter ?? throw new ArgumentNullException(nameof(leftItemComparablePartGetter));
            RightComparablePartProvider = rightItemComparablePartGetter ?? throw new ArgumentNullException(nameof(rightItemComparablePartGetter));
        }

        /// <summary>
        /// Checks arguments when yielding collection modifications.
        /// </summary>
        /// <param name="leftItems">The collection you want to have transformed.</param>
        /// <param name="rightItems">The collection in which <paramref name="leftItems"/> could be transformed.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="leftItems"/> is null.</exception>
        private void FixRightItems(IEnumerator<TLeftItem> leftItems, [NotNull] ref IEnumerator<TRightItem>? rightItems)
        {
            if (leftItems is null) {
                throw new ArgumentNullException(nameof(leftItems));
            }

            rightItems ??= Enumerable.Empty<TRightItem>().GetEnumerator();
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
            IEnumerator<TLeftItem> leftItems,
            IEnumerator<TRightItem>? rightItems,
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
                IEnumerator<TLeftItem> leftItems,
                IEnumerator<TRightItem>? rightItems,
                CollectionModificationYieldCapabilities yieldCapabilities)
            {
                FixRightItems(leftItems, ref rightItems);

                return EqualityTrailingCollectionSynchronizationMethod.YieldCollectionModifications(
                    leftItems,
                    rightItems,
                    new CovertingEnumerator<TLeftItem, TComparableItem>(leftItems, LeftComparablePartProvider),
                    new CovertingEnumerator<TRightItem, TComparableItem>(rightItems, RightComparablePartProvider),
                    EqualityComparer,
                    yieldCapabilities);
            }

            private class CovertingEnumerator<TSource, TComparablePart> : IEnumerator<TComparablePart>
            {
                public TComparablePart Current {
                    get {
                        if (_isCurrentConverted) {
                            return _currentConverted;
                        }

                        _currentConverted = _converter(_source.Current);
                        _isCurrentConverted = true;
                        return _currentConverted;
                    }
                }

                object? IEnumerator.Current => Current;

                [MemberNotNullWhen(true, nameof(_currentConverted))]
                private bool _isCurrentConverted { get; set; }

                private readonly IEnumerator<TSource> _source;
                private readonly Func<TSource, TComparablePart> _converter;
                private TComparablePart? _currentConverted;

                public CovertingEnumerator(IEnumerator<TSource> source, Func<TSource, TComparablePart> converter)
                {
                    _source = source;
                    _converter = converter;
                }

                public bool MoveNext()
                {
                    _isCurrentConverted = false;
                    return true;
                }

                public void Reset() => throw new NotImplementedException();

                public void Dispose()
                {
                }
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
                IEnumerator<TLeftItem> leftItems,
                IEnumerator<TRightItem>? rightItems,
                CollectionModificationYieldCapabilities yieldCapabilities)
            {
                FixRightItems(leftItems, ref rightItems);

                return SortedCollectionSynchronizationMethod.YieldCollectionModifications(
                    leftItems,
                    rightItems,
                    LeftComparablePartProvider,
                    RightComparablePartProvider,
                    Comparer,
                    yieldCapabilities);
            }
        }
    }
}
