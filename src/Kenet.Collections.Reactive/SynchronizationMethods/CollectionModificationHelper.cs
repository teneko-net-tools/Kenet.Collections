// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    internal static class CollectionModificationHelper
    {
        public static bool MoveRangeContains(ICollectionModificationData modification, int index) =>
            CollectionTools.MoveRangeContains(modification.OldIndex, modification.NewIndex, modification.OldItemsCount!.Value, index);

        public static CollectionModifiedEventArgs<TItem> ToEventArgs<TItem>(this ICollectionModification<TItem, TItem> modification) =>
            new CollectionModifiedEventArgs<TItem>(modification);
    }
}
