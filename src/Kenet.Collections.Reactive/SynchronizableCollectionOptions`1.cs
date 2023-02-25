// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Kenet.Collections.Reactive
{
    public sealed class SynchronizableCollectionOptions<TItem> : AbstractCollectionOptions<SynchronizableCollectionOptions<TItem>, TItem>
        where TItem : notnull
    {
        public ItemsOptionsImpl ItemsOptions { get; }

        public SynchronizableCollectionOptions() =>
            ItemsOptions = new ItemsOptionsImpl();

        public SynchronizableCollectionOptions<TItem> ConfigureItems(Action<ItemsOptionsImpl> configureOptions)
        {
            configureOptions?.Invoke(ItemsOptions);
            return this;
        }

        public class ItemsOptionsImpl : AbstractCollectionItemsOptions<ItemsOptionsImpl, TItem>
        {
            /// <summary>
            /// If not null it is called in <see cref="SynchronizingCollectionBase{SuperItemType, SubItemType}.ReplaceItems(SynchronizingCollectionBase{SuperItemType, SubItemType}.ApplyingCollectionModifications)"/>
            /// but after the items could have been replaced.
            /// </summary>
            public CollectionUpdateItemHandler<TItem, TItem>? ItemUpdateHandler { get; set; }
        }
    }
}
