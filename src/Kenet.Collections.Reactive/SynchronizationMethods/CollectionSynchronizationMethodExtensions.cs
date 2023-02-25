// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    public static class CollectionSynchronizationMethodExtensions
    {
        public static IEnumerable<CollectionModification<TRightItem, TLeftItem>> YieldCollectionModifications<TLeftItem, TRightItem>(
            this ICollectionSynchronizationMethod<TLeftItem, TRightItem> synchronizationMethod,
            IEnumerable<TLeftItem> leftItems,
            IEnumerable<TRightItem>? rightItems) =>
            synchronizationMethod.YieldCollectionModifications(leftItems.GetEnumerator(), rightItems?.GetEnumerator(), CollectionModificationYieldCapabilities.All);
    }
}
