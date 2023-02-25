// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Teronis.Extensions;

namespace Kenet.Collections.Reactive
{
    internal class MutableList<TItem> : IMutableList<TItem>
    {
        public event EventHandler<Action<IListMutationTarget<TItem>>>? CollectRedirectionTargets;

        public IList<TItem> Items { get; }
        public IStatelessCollectionMutator<TItem> Handler { get; }

        private readonly ListMutationTarget _itemsMutationTarget;

        public MutableList(IList<TItem> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Handler = DefaultCollectionChangeBehaviour.Default;
            _itemsMutationTarget = new ListMutationTarget(Items, Handler);
        }

        public MutableList(IList<TItem> items, IStatelessCollectionMutator<TItem>? modifier)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Handler = modifier ?? DefaultCollectionChangeBehaviour.Default;
            _itemsMutationTarget = new ListMutationTarget(Items, Handler);
        }

        /// <inheritdoc/>
        public virtual bool CanReplaceItem =>
            Handler.CanReplaceItem;

        public void MutateItems(Action<IListMutationTarget<TItem>> mutateCollection) =>
            mutateCollection(_itemsMutationTarget);

        public void MutateItemsThroughRedirection(Action<IListMutationTarget<TItem>> mutateCollection)
        {
            var targets = new List<IListMutationTarget<TItem>>();
            CollectRedirectionTargets?.Invoke(this, targets.Add);

            foreach (var target in targets) {
                mutateCollection(target);
            }
        }

        public void Mutate(Action<IListMutationTarget<TItem>> mutateCollection, bool disableRedirection)
        {
            if (disableRedirection) {
                MutateItems(mutateCollection);
                return;
            }

            MutateItemsThroughRedirection(mutateCollection);
        }

        private class ListMutationTarget : IListMutationTarget<TItem>
        {
            private readonly IList<TItem> _items;
            private readonly IStatelessCollectionMutator<TItem> _collectionModifier;

            public ListMutationTarget(IList<TItem> items, IStatelessCollectionMutator<TItem> collectionModifier)
            {
                _items = items;
                _collectionModifier = collectionModifier;
            }

            public virtual void InsertItem(int insertAt, TItem item) =>
                _collectionModifier.InsertItem(_items, insertAt, item);

            public virtual void RemoveItem(int removeAt) =>
                _collectionModifier.RemoveItem(_items, removeAt);

            public virtual void MoveItems(int fromIndex, int toIndex, int count) =>
                _collectionModifier.MoveItems(_items, fromIndex, toIndex, count);

            public virtual void ReplaceItem(int replaceAt, Func<TItem> getItem) =>
                _collectionModifier.ReplaceItem(_items, replaceAt, getItem);

            public virtual void ResetItems() =>
                _collectionModifier.Reset(_items);
        }

        /// <summary>
        /// The default behaviour for changing the collection on collection-change-notifications.
        /// Insert, remove, move and reset do as they are named. Only the replace-functionality is
        /// disabled.
        /// </summary>
        public class DefaultCollectionChangeBehaviour : IStatelessCollectionMutator<TItem>
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
