// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Kenet.Collections.Reactive
{
    public sealed class SynchronizingCollectionOptions<TSuperItem, TSubItem> : AbstractCollectionOptions<SynchronizingCollectionOptions<TSuperItem, TSubItem>, TSuperItem>
        where TSuperItem : notnull
        where TSubItem : notnull
    {
        public SuperItemsOptionsImpl SuperItemsOptions { get; }
        public SubItemsOptionsImpl SubItemsOptions { get; }

        public SynchronizingCollectionOptions()
        {
            SuperItemsOptions = new SuperItemsOptionsImpl();
            SubItemsOptions = new SubItemsOptionsImpl();
        }

        public SynchronizingCollectionOptions<TSuperItem, TSubItem> ConfigureSubItems(Action<SuperItemsOptionsImpl> configureOptions)
        {
            configureOptions?.Invoke(SuperItemsOptions);
            return this;
        }

        public SynchronizingCollectionOptions<TSuperItem, TSubItem> ConfigureSubItems(Action<SubItemsOptionsImpl> configureOptions)
        {
            configureOptions?.Invoke(SubItemsOptions);
            return this;
        }

        public abstract class AbstractItemsOptions<TDerived, TItem> : AbstractCollectionItemsOptions<TDerived, TItem>, ISynchronizingCollectionItemsOptions<TItem>
            where TDerived : AbstractItemsOptions<TDerived, TItem>
        {
            public ISynchronizedCollection<TItem>? Items { get; protected set; }

            /// <summary>
            /// Sets <see cref="Items"/> and <see cref="AbstractCollectionItemsOptions{TDerived, TItem}.ItemsMutationTarget"/>.
            /// </summary>
            /// <param name="items"></param>
            /// <param name="itemsMutationTarget"></param>
            public TDerived SetItems(ISynchronizedCollection<TItem> items, IListMutationTarget<TItem> itemsMutationTarget)
            {
                Items = items ?? throw new ArgumentNullException(nameof(items));
                ItemsMutationTarget = itemsMutationTarget ?? throw new ArgumentNullException(nameof(itemsMutationTarget));
                return (TDerived)this;
            }

            void ISynchronizingCollectionItemsOptions<TItem>.SetItems(ISynchronizedCollection<TItem> items, IListMutationTarget<TItem> itemsMutationTarget) =>
                SetItems(items, itemsMutationTarget);
        }

        public sealed class SuperItemsOptionsImpl : AbstractItemsOptions<SuperItemsOptionsImpl, TSuperItem>
        {
            /// <summary>
            /// If not null it is called in <see cref="SynchronizingCollectionBase{SuperItemType, SubItemType}.ReplaceItems(SynchronizingCollectionBase{SuperItemType, SubItemType}.ApplyingCollectionModifications)"/>
            /// but after the items could have been replaced and before <see cref="SynchronizingCollectionBase{SuperItemType, SubItemType}.OnAfterReplaceItem(int)"/>.
            /// </summary>
            public CollectionUpdateItemHandler<TSuperItem, TSubItem>? ItemUpdateHandler { get; set; }
        }

        public sealed class SubItemsOptionsImpl : AbstractItemsOptions<SubItemsOptionsImpl, TSubItem>
        {
            /// <summary>
            /// If not null it is called by <see cref="SynchronizingCollectionBase{SuperItemType, SubItemType}.ReplaceItems(SynchronizingCollectionBase{SuperItemType, SubItemType}.ApplyingCollectionModifications)"/>
            /// but after the items could have been replaced and before <see cref="SynchronizingCollectionBase{SuperItemType, SubItemType}.OnAfterReplaceItem(int)"/>.
            /// <br/>
            /// <br/>(!) Take into regard, that <see cref="SuperItemsOptionsImpl.ItemUpdateHandler"/> is called at first if not null.
            /// </summary>
            public CollectionUpdateItemHandler<TSubItem, TSuperItem>? ItemUpdateHandler { get; set; }
        }
    }
}
