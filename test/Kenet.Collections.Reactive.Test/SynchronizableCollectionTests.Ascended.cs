﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Kenet.Collections.Algorithms.Modifications;
using Xunit;

namespace Kenet.Collections.Synchronization
{
    public abstract partial class SynchronizableCollectionTests
    {
        public class Ascended : TestSuite<Number>
        {
            public override EqualityComparer<Number> EqualityComparer =>
                Number.ReferenceEqualityComparer.Default;

            private static Number @null = null!;
            private static Number @null2 = null!;
            private static Number zero = 0;
            private static Number one = 1;
            private static Number two = 2;
            private static Number three = new Number(3) { Tag = nameof(three) };
            private static Number three2 = new Number(3) { Tag = nameof(three2) };
            private static Number four = 4;
            private static Number five = 5;
            private static Number nine = 9;

            public Ascended() : base(
                new SynchronizableCollection<Number>(
                    new SynchronizableCollectionOptions<Number>()
                    .ConfigureItems(options => options
                        .SetItems(CollectionChangeHandler<Number>.ItemReplacableCollectionChangeBehaviour.Default))
                    .SetSortedSynchronizationMethod(Number.Comparer.Ascended)))
            { }

            [Theory]
            [ClassData(typeof(Generator))]
            public override void Direct_synchronization_by_modifications(Number[] leftItems, Number[] rightItems, Number[]? expected = null,
                 CollectionModificationYieldCapabilities? yieldCapabilities = null) =>
                 base.Direct_synchronization_by_modifications(leftItems, rightItems, expected, yieldCapabilities);

            [Theory]
            [ClassData(typeof(Generator))]
            public override void Direct_synchronization_by_batched_modifications(Number[] leftItems, Number[] rightItems, Number[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null) =>
                base.Direct_synchronization_by_batched_modifications(leftItems, rightItems, expected, yieldCapabilities);

            [Theory]
            [ClassData(typeof(Generator))]
            public override void Relocated_synchronization_by_modifications(Number[] leftItems, Number[] rightItems, Number[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null) =>
                base.Relocated_synchronization_by_modifications(leftItems, rightItems, expected, yieldCapabilities);

            [Theory]
            [ClassData(typeof(Generator))]
            public override void Relocated_synchronization_by_batched_modifications(Number[] leftItems, Number[] rightItems, Number[]? expected = null,
                CollectionModificationYieldCapabilities? yieldCapabilities = null) =>
                base.Relocated_synchronization_by_batched_modifications(leftItems, rightItems, expected, yieldCapabilities);

            public class Generator : GeneratorBase<Number>
            {
                public override IEnumerator<object[]> GetEnumerator()
                {
                    yield return List(Values(@null, nine), Values(zero, one));
                    yield return List(Values(@null, one, nine), Values(zero, one));
                    yield return List(Values(@null, one, three, nine), Values(one, four));
                    yield return List(Values(@null, one, three, nine), Values(zero, one, four));
                    yield return List(Values(@null, one, three, four, nine), Values(zero, three, three2, four, five));

                    yield return List(Values(@null), Values(@null, null2));
                    yield return List(Values(@null), Values(null2, @null));
                    yield return List(Values(@null), Values(null2, @null), Values(null2, @null), CollectionModificationYieldCapabilities.InsertRemove);
                    yield return List(Values(@null), Values(null2, @null), Values(@null, null2), CollectionModificationYieldCapabilities.InsertRemove);

                    yield return List(Values(@null, three2, nine), Values(three), Values(three));
                    yield return List(Values(@null, three2, nine), Values(three), Values(three2), CollectionModificationYieldCapabilities.InsertRemove);

                    yield return List(Values(one, three), Values(two), Values(one, two, three), CollectionModificationYieldCapabilities.Insert);

                    yield return List(Values(two, three, four), Values(two, four), Values(two, four), CollectionModificationYieldCapabilities.Remove);
                    yield return List(Values(one, two, three, three2, four), Values(two, four, nine), Values(two, four), CollectionModificationYieldCapabilities.Remove);
                }
            }
        }
    }
}
