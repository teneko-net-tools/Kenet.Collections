// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    /// <summary>
    /// Represents a collection modification. It is the typed version of <see cref="NotifyCollectionChangedEventArgs"/>.
    /// </summary>
    /// <typeparam name="TNewItem"></typeparam>
    /// <typeparam name="TOldItem"></typeparam>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class CollectionModification<TNewItem, TOldItem> : ICollectionModification<TNewItem, TOldItem>
    {
        /// <inheritdoc/>
        public NotifyCollectionChangedAction Action { get; private set; }
        /// <inheritdoc/>

        public ICollectionModificationPart<TNewItem, TOldItem, TOldItem, TNewItem> OldPart =>
            oldPart;
        /// <inheritdoc/>

        public IReadOnlyList<TOldItem>? OldItems =>
            oldPart.Items;
        /// <inheritdoc/>

        public int OldIndex =>
            oldPart.Index;
        /// <inheritdoc/>

        public ICollectionModificationPart<TNewItem, TOldItem, TNewItem, TOldItem> NewPart =>
            newPart;
        /// <inheritdoc/>

        public IReadOnlyList<TNewItem>? NewItems =>
            newPart.Items;
        /// <inheritdoc/>

        public int NewIndex =>
            newPart.Index;

        private readonly CollectionModificationPart<TOldItem, TNewItem> oldPart;
        private readonly CollectionModificationPart<TNewItem, TOldItem> newPart;

        private string GetDebuggerDisplay() =>
            $"{Action}, {nameof(OldIndex)} = {OldIndex}, {nameof(NewIndex)} = {NewIndex}";
        /// <inheritdoc/>

        protected internal CollectionModification(NotifyCollectionChangedAction changeAction, IReadOnlyList<TOldItem>? oldItems, int oldIndex, IReadOnlyList<TNewItem>? newItems, int newIndex)
        {
            Action = changeAction;
            oldPart = new CollectionModificationPart<TOldItem, TNewItem>(this, PartialCollectionChangeItemState.OldItem, oldItems, oldIndex);
            newPart = new CollectionModificationPart<TNewItem, TOldItem>(this, PartialCollectionChangeItemState.NewItem, newItems, newIndex);
        }
        /// <inheritdoc/>

        protected internal CollectionModification(NotifyCollectionChangedAction changeAction, IReadOnlyList<TOldItem>? oldValues, int oldIndex, [AllowNull] TNewItem newItem, int newIndex)
            : this(changeAction, oldValues, oldIndex, new TNewItem[] { newItem! }, newIndex) { }
        /// <inheritdoc/>

        protected internal CollectionModification(NotifyCollectionChangedAction changeAction, [AllowNull] TOldItem oldItem, int oldIndex, IReadOnlyList<TNewItem>? newItems, int newIndex)
            : this(changeAction, new TOldItem[] { oldItem! }, oldIndex, newItems, newIndex) { }
        /// <inheritdoc/>

        protected internal CollectionModification(NotifyCollectionChangedAction changeAction, [AllowNull] TOldItem oldItem, int oldIndex, [AllowNull] TNewItem newItem, int newIndex)
            : this(changeAction, new TOldItem[] { oldItem! }, oldIndex, new TNewItem[] { newItem! }, newIndex) { }

        #region ICollectionModificationData

        int? ICollectionModificationData.OldItemsCount =>
            oldPart.Items?.Count;

        int? ICollectionModificationData.NewItemsCount =>
            newPart.Items?.Count;

        #endregion

        private enum PartialCollectionChangeItemState
        {
            OldItem,
            NewItem
        }

        internal abstract class CollectionModificationPartBase<ItemType, TOtherItem> : ICollectionModificationPart<TNewItem, TOldItem, ItemType, TOtherItem>
        {
            public ICollectionModification<TNewItem, TOldItem> Owner { get; }

            public ICollectionModificationPart<TNewItem, TOldItem, TOtherItem, ItemType> OtherPart =>
                ReferenceEquals(this, Owner.OldPart) ? (ICollectionModificationPart<TNewItem, TOldItem, TOtherItem, ItemType>)Owner.NewPart :
                (ICollectionModificationPart<TNewItem, TOldItem, TOtherItem, ItemType>)Owner.OldPart;

            public abstract IReadOnlyList<ItemType>? Items { get; }
            public abstract int Index { get; }

            public CollectionModificationPartBase(ICollectionModification<TNewItem, TOldItem> modification) =>
                Owner = modification;
        }

        private class CollectionModificationPart<TItem, TOtherItem> : CollectionModificationPartBase<TItem, TOtherItem>
        {
            public PartialCollectionChangeItemState ItemState { get; private set; }
            public override IReadOnlyList<TItem>? Items { get; }
            public override int Index { get; }

            public CollectionModificationPart(CollectionModification<TNewItem, TOldItem> modification,
                PartialCollectionChangeItemState itemState, IReadOnlyList<TItem>? items, int index)
                : base(modification)
            {
                ItemState = itemState;
                Items = items;
                Index = index;
            }
        }
    }
}
