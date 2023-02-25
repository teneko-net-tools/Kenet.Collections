// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Kenet.Collections.Reactive.PostConfigurators
{
    internal class SynchronizingCollectionOptionsPostConfigurator
    {
        public static readonly SynchronizingCollectionOptionsPostConfigurator Default = new();

        public void PostConfigure<TItem>(
            ISynchronizingCollectionItemsOptions<TItem> itemsOptions,
            out IListMutationTarget<TItem> itemsMutationTarget,
            Func<IListMutationTarget<TItem>, ISynchronizedCollection<TItem>> itemsFactory,
            out ISynchronizedCollection<TItem> items)
            where TItem : notnull
        {
            SynchronizableCollectionItemsOptionsPostConfigurator.Default.PostConfigure(itemsOptions, out itemsMutationTarget);

            var itemOptionsSynchronizedItems = itemsOptions.Items;

            if (!(itemOptionsSynchronizedItems is null)) {
                items = itemOptionsSynchronizedItems;
                return;
            }

            items = itemsFactory(itemsMutationTarget);
            itemsOptions.SetItems(items, itemsMutationTarget);
        }
    }
}

