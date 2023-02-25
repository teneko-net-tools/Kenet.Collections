// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    public static class CollectionModification
    {
        public static CollectionModification<TNewItem, TOldItem> ForAdd<TNewItem, TOldItem>(int newIndex, TNewItem newItem) =>
            new(NotifyCollectionChangedAction.Add, oldItem: default!, oldIndex: -1, newItem, newIndex);

        public static CollectionModification<TItem, TItem> ForAdd<TItem>(int newIndex, TItem newItem) =>
            ForAdd<TItem, TItem>(newIndex, newItem);

        public static CollectionModification<TNewItem, TOldItem> ForAdd<TNewItem, TOldItem>(int newIndex, IReadOnlyList<TNewItem> newItems) =>
            new(NotifyCollectionChangedAction.Add, oldItems: null, oldIndex: -1, newItems, newIndex);

        public static CollectionModification<TItem, TItem> ForAdd<TItem>(int newIndex, IReadOnlyList<TItem> newItems) =>
            ForAdd<TItem, TItem>(newIndex, newItems);

        public static CollectionModification<TNewItem, TOldItem> ForRemove<TNewItem, TOldItem>(int oldIndex, TOldItem oldItem) =>
            new(NotifyCollectionChangedAction.Remove, oldItem, oldIndex, newItems: default, newIndex: -1);

        public static CollectionModification<TItem, TItem> ForRemove<TItem>(int oldIndex, TItem oldItem) =>
            ForRemove<TItem, TItem>(oldIndex, oldItem);

        public static CollectionModification<TNewItem, TOldItem> ForRemove<TNewItem, TOldItem>(int oldIndex, IReadOnlyList<TOldItem> oldItems) =>
            new(NotifyCollectionChangedAction.Remove, oldItems, oldIndex, newItem: default, newIndex: -1);

        public static CollectionModification<TItem, TItem> ForRemove<TItem>(int oldIndex, IReadOnlyList<TItem> oldItems) =>
            ForRemove<TItem, TItem>(oldIndex, oldItems);

        #region Replace (indexed)

        public static CollectionModification<TNewItem, TOldItem> ForReplace<TNewItem, TOldItem>(int oldIndex, TOldItem oldItem, TNewItem newItem) =>
               new(NotifyCollectionChangedAction.Replace, oldItem, oldIndex, newItem, oldIndex);

        public static CollectionModification<TNewItem, TOldItem> ForReplace<TNewItem, TOldItem>(int oldIndex, IReadOnlyList<TOldItem> oldItems, IReadOnlyList<TNewItem> newItems) =>
            new(NotifyCollectionChangedAction.Replace, oldItems, oldIndex, newItems, oldIndex);

        public static CollectionModification<TNewItem, TOldItem> ForReplace<TNewItem, TOldItem>(int oldIndex, TOldItem oldItem, IReadOnlyList<TNewItem> newItems) =>
               new(NotifyCollectionChangedAction.Replace, oldItem, oldIndex, newItems, oldIndex);

        public static CollectionModification<TNewItem, TOldItem> ForReplace<TNewItem, TOldItem>(int oldIndex, IReadOnlyList<TOldItem> oldItems, TNewItem newItem) =>
            new(NotifyCollectionChangedAction.Replace, oldItems, oldIndex, newItem, oldIndex);

        #endregion

        #region Replace (non-indexed)

        /* TODO: Allow replacable non-indexed old items with new items. */

        //public static CollectionModification<TNewItem, TOldItem> ForReplace<TNewItem, TOldItem>(TOldItem oldItem, TNewItem newItem) =>
        //       new CollectionModification<TNewItem, TOldItem>(NotifyCollectionChangedAction.Replace, oldItem, oldIndex: -1, newItem, newIndex: -1);

        //public static CollectionModification<TNewItem, TOldItem> ForReplace<TNewItem, TOldItem>(IReadOnlyList<TOldItem> oldItems, IReadOnlyList<TNewItem> newItems) =>
        //    new CollectionModification<TNewItem, TOldItem>(NotifyCollectionChangedAction.Replace, oldItems, oldIndex: -1, newItems, newIndex: -1);

        //public static CollectionModification<TNewItem, TOldItem> ForReplace<TNewItem, TOldItem>(TOldItem oldItem, IReadOnlyList<TNewItem> newItems) =>
        //       new CollectionModification<TNewItem, TOldItem>(NotifyCollectionChangedAction.Replace, oldItem, oldIndex: -1, newItems, newIndex: -1);

        //public static CollectionModification<TNewItem, TOldItem> ForReplace<TNewItem, TOldItem>(IReadOnlyList<TOldItem> oldItems, TNewItem newItem) =>
        //    new CollectionModification<TNewItem, TOldItem>(NotifyCollectionChangedAction.Replace, oldItems, oldIndex: -1, newItem, newIndex: -1);

        #endregion

        public static CollectionModification<TNewItem, TOldItem> ForMove<TNewItem, TOldItem>(int oldIndex, TOldItem oldItem, int newIndex) =>
               new(NotifyCollectionChangedAction.Move, oldItem, oldIndex, newItem: default, newIndex);

        public static CollectionModification<TItem, TItem> ForMove<TItem>(int oldIndex, TItem oldItem, int newIndex) =>
            ForMove<TItem, TItem>(oldIndex, oldItem, newIndex);

        public static CollectionModification<TNewItem, TOldItem> ForMove<TNewItem, TOldItem>(int oldIndex, IReadOnlyList<TOldItem> oldItems, int newIndex) =>
            new(NotifyCollectionChangedAction.Move, oldItems, oldIndex, newItems: null, newIndex);

        public static CollectionModification<TItem, TItem> ForMove<TItem>(int oldIndex, IReadOnlyList<TItem> oldItems, int newIndex) =>
            ForMove<TItem, TItem>(oldIndex, oldItems, newIndex);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TNewItem"></typeparam>
        /// <typeparam name="TOldItem"></typeparam>
        /// <param name="oldIndex"></param>
        /// <param name="oldItems"></param>
        /// <param name="newIndex"></param>
        /// <param name="oldItemsCutOutSize">
        /// If specified it means, that <paramref name="oldItems"/> is the 
        /// whole and  original item list of which the items are cutted out
        /// by the amount of <paramref name="oldItemsCutOutSize"/>.
        /// </param>
        /// <returns></returns>
        public static CollectionModification<TNewItem, TOldItem> ForMove<TNewItem, TOldItem>(int oldIndex, IEnumerable<TOldItem> oldItems, int newIndex, int oldItemsCutOutSize)
        {
            static IReadOnlyList<TItem> CutOut<TItem>(int index, IEnumerable<TItem> originalItems, int cutOutSize)
            {
                if (originalItems is List<TItem> list) {
                    var items = new TItem[cutOutSize];
                    list.CopyTo(index, items, 0, cutOutSize);
                    return items;
                } else {
                    return originalItems.Skip(index).Take(cutOutSize).ToArray();
                }
            }

            var oldItemList = CutOut(oldIndex, oldItems, oldItemsCutOutSize);
            return new CollectionModification<TNewItem, TOldItem>(NotifyCollectionChangedAction.Move, oldItemList, oldIndex, newItems: null, newIndex);
        }

        public static CollectionModification<TItem, TItem> ForMove<TItem>(int oldIndex, IEnumerable<TItem> oldItems, int newIndex, int oldItemsCutOutSize) =>
            ForMove<TItem, TItem>(oldIndex, oldItems, newIndex, oldItemsCutOutSize);

        public static CollectionModification<TNewItem, TOldItem> ForReset<TNewItem, TOldItem>() =>
            new(NotifyCollectionChangedAction.Reset, oldItems: null, oldIndex: -1, newItems: null, newIndex: -1);

        public static CollectionModification<TItem, TItem> ForReset<TItem>() =>
            ForReset<TItem, TItem>();
    }
}
