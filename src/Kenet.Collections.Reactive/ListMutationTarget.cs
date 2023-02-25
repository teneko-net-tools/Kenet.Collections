// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Teronis.Extensions;

namespace Kenet.Collections.Reactive
{
    internal class ListMutationTarget<TItem> : IListMutationTarget<TItem>
    {
        public event EventHandler<Action<IMutationTarget<TItem>>>? CollectRedirectionTargets;

        public IList<TItem> Items { get; }
        public IListMutator<TItem> ItemsMutator { get; }

        private readonly ItemsMutationTargetAdapter _itemsMutationTarget;

        public ListMutationTarget(IList<TItem> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            ItemsMutator = DefaultCollectionChangeBehaviour.Default;
            _itemsMutationTarget = new ItemsMutationTargetAdapter(Items, ItemsMutator);
        }

        public ListMutationTarget(IList<TItem> items, IListMutator<TItem>? itemsMutator)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            ItemsMutator = itemsMutator ?? DefaultCollectionChangeBehaviour.Default;
            _itemsMutationTarget = new ItemsMutationTargetAdapter(Items, ItemsMutator);
        }

        /// <inheritdoc/>
        public virtual bool CanReplaceItem =>
            ItemsMutator.CanReplaceItem;

        public void MutateItems(Action<IMutationTarget<TItem>> mutateItems) =>
            mutateItems(_itemsMutationTarget);

        public void MutateItemsThroughRedirection(Action<IMutationTarget<TItem>> mutateItems)
        {
            var targets = new List<IMutationTarget<TItem>>();
            CollectRedirectionTargets?.Invoke(this, targets.Add);

            foreach (var target in targets) {
                mutateItems(target);
            }
        }

        public void Mutate(Action<IMutationTarget<TItem>> mutateItems, bool disableRedirection)
        {
            if (disableRedirection) {
                MutateItems(mutateItems);
                return;
            }

            MutateItemsThroughRedirection(mutateItems);
        }

        private class ItemsMutationTargetAdapter : IMutationTarget<TItem>
        {
            private readonly IList<TItem> _items;
            private readonly IListMutator<TItem> _itemsMutator;

            public ItemsMutationTargetAdapter(IList<TItem> items, IListMutator<TItem> itemsMutator)
            {
                _items = items;
                _itemsMutator = itemsMutator;
            }

            public virtual void InsertItem(int insertAt, TItem item) =>
                _itemsMutator.InsertItem(_items, insertAt, item);

            public virtual void RemoveItem(int removeAt) =>
                _itemsMutator.RemoveItem(_items, removeAt);

            public virtual void MoveItems(int fromIndex, int toIndex, int count) =>
                _itemsMutator.MoveItems(_items, fromIndex, toIndex, count);

            public virtual void ReplaceItem(int replaceAt, Func<TItem> getItem) =>
                _itemsMutator.ReplaceItem(_items, replaceAt, getItem);

            public virtual void ResetItems() =>
                _itemsMutator.Reset(_items);
        }

        /// <summary>
        /// The default behaviour for changing the collection on collection-change-notifications.
        /// Insert, remove, move and reset do as they are named. Only the replace-functionality is
        /// disabled.
        /// </summary>
        public class DefaultCollectionChangeBehaviour : IListMutator<TItem>
        {
            public static DefaultCollectionChangeBehaviour Default = new();

            public virtual bool CanReplaceItem { get; } = false;

            public virtual void InsertItem(IList<TItem> list, int insertAt, TItem item) =>
                list.Insert(insertAt, item);

            public virtual void RemoveItem(IList<TItem> list, int removeAt) =>
                list.RemoveAt(removeAt);

            public virtual void MoveItems(IList<TItem> list, int fromIndex, int toIndex, int count) =>
                list.Move(fromIndex, toIndex, count);

            /// <summary>
            /// No behaviour is defined. Thus the body is empty.
            /// </summary>
            /// <param name="list"></param>
            /// <param name="replaceAt"></param>
            /// <param name="getNewItem">A function that gives you the new item.</param>
            public virtual void ReplaceItem(IList<TItem> list, int replaceAt, Func<TItem> getNewItem)
            { }

            public virtual void Reset(IList<TItem> list) =>
                list.Clear();
        }

        /// <summary>
        /// Implements the behaviour to replace an item at a given index on request. It is the contrary
        /// implementation of <see cref="DefaultCollectionChangeBehaviour"/> as it would not replace
        /// existing items.
        /// </summary>
        public class ItemReplacableCollectionChangeBehaviour : DefaultCollectionChangeBehaviour
        {
            public static new ItemReplacableCollectionChangeBehaviour Default = new();

            public override bool CanReplaceItem =>
                true;

            /// <summary>
            /// Replaces the item at index <paramref name="replaceAt"/> by the item you get from <paramref name="getNewItem"/>.
            /// </summary>
            /// <param name="list"></param>
            /// <param name="replaceAt"></param>
            /// <param name="getNewItem"></param>
            public override void ReplaceItem(IList<TItem> list, int replaceAt, Func<TItem> getNewItem) =>
                list[replaceAt] = getNewItem();
        }
    }
}
