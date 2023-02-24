﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Teronis.Collections.Algorithms.Modifications;
using Xunit;

namespace Teronis.Collections.Synchronization
{
    public abstract partial class SynchronizableCollectionTests
    {
        public abstract class TestSuite<T>
            where T : notnull
        {
            public SynchronizableCollection<T> Collection { get; }
            public abstract EqualityComparer<T> EqualityComparer { get; }

            protected TestSuite(SynchronizableCollection<T> collection) =>
                Collection = collection;

            public virtual void Direct_synchronization_by_modifications(
                T[] leftItems,
                T[] rightItems,
                T[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null)
            {
                expected = expected ?? rightItems;
                yieldCapabilities = yieldCapabilities ?? CollectionModificationYieldCapabilities.All;

                Collection.SynchronizeCollection(leftItems);
                Assert.Equal(leftItems, Collection, EqualityComparer);

                Collection.SynchronizeCollection(rightItems, yieldCapabilities.Value);
                Assert.Equal(expected, Collection, EqualityComparer);
            }

            public virtual void Direct_synchronization_by_batched_modifications(
                T[] leftItems,
                T[] rightItems,
                T[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null)
            {
                expected = expected ?? rightItems;
                yieldCapabilities = yieldCapabilities ?? CollectionModificationYieldCapabilities.All;

                Collection.SynchronizeCollection(leftItems, batchModifications: true);
                Assert.Equal(leftItems, Collection, EqualityComparer);

                Collection.SynchronizeCollection(rightItems, yieldCapabilities.Value, batchModifications: true);
                Assert.Equal(expected, Collection, EqualityComparer);
            }

            private IEnumerable<T> ToEnumerable(IEnumerable<T> items)
            {
                foreach (var item in items) {
                    yield return item;
                }
            }

            public virtual void Relocated_synchronization_by_modifications(
                T[] leftItems,
                T[] rightItems,
                T[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null)
            {
                expected = expected ?? rightItems;
                yieldCapabilities = yieldCapabilities ?? CollectionModificationYieldCapabilities.All;

                Collection.SynchronizeCollection(leftItems, batchModifications: true);
                Assert.Equal(leftItems, Collection, EqualityComparer);

                var collection = ToEnumerable(Collection);
                Collection.SynchronizeCollection(collection, rightItems, yieldCapabilities.Value, batchModifications: true);
                Assert.Equal(expected, Collection, EqualityComparer);
            }

            public virtual void Relocated_synchronization_by_batched_modifications(
                T[] leftItems,
                T[] rightItems,
                T[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null)
            {
                expected = expected ?? rightItems;
                yieldCapabilities = yieldCapabilities ?? CollectionModificationYieldCapabilities.All;

                Collection.SynchronizeCollection(leftItems, batchModifications: true);
                Assert.Equal(leftItems, Collection, EqualityComparer);

                var collection = ToEnumerable(Collection);
                Collection.SynchronizeCollection(collection, rightItems, yieldCapabilities.Value, batchModifications: true);
                Assert.Equal(expected, Collection, EqualityComparer);
            }
        }
    }
}
