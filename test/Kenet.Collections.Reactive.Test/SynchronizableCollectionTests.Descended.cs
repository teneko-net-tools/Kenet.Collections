// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Kenet.Collections.Reactive;
using Kenet.Collections.Reactive.SynchronizationMethods;
using Xunit;

namespace Kenet.Collections.Reactive
{
    public abstract partial class SynchronizableCollectionTests
    {
        public class Descended : TestSuite<Number>
        {
            public override EqualityComparer<Number> EqualityComparer =>
                Number.ReferenceEqualityComparer.Default;

            public Descended() : base(
                new SynchronizableCollection<Number>(
                    SynchronizableCollectionOptions.Create<Number>()
                        .ConfigureItems(options => options
                            .SetItems(ListMutationTarget<Number>.ItemReplacableCollectionChangeBehaviour.Default))
                        .SetSortedSynchronizationMethod(Number.Comparer.Descended)))
            { }

            [Theory]
            [ClassData(typeof(DescendedGenerator))]
            public override void Direct_synchronization_by_modifications(Number[] leftItems, Number[] rightItems, Number[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null) =>
                base.Direct_synchronization_by_modifications(leftItems, rightItems, expected, yieldCapabilities);

            [Theory]
            [ClassData(typeof(DescendedGenerator))]
            public override void Direct_synchronization_by_batched_modifications(Number[] leftItems, Number[] rightItems, Number[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null) =>
                base.Direct_synchronization_by_batched_modifications(leftItems, rightItems, expected, yieldCapabilities);

            [Theory]
            [ClassData(typeof(DescendedGenerator))]
            public override void Relocated_synchronization_by_modifications(Number[] leftItems, Number[] rightItems, Number[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null) =>
                base.Relocated_synchronization_by_modifications(leftItems, rightItems, expected, yieldCapabilities);

            [Theory]
            [ClassData(typeof(DescendedGenerator))]
            public override void Relocated_synchronization_by_batched_modifications(Number[] leftItems, Number[] rightItems, Number[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null) =>
                base.Relocated_synchronization_by_batched_modifications(leftItems, rightItems, expected, yieldCapabilities);

            public class DescendedGenerator : Ascended.Generator
            {
                protected override Number[] Values(params Number[] values) =>
                    values.Reverse().ToArray();
            }
        }
    }
}
