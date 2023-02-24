// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Kenet.Collections.Synchronization.PostConfigurators
{
    internal class SynchronizingCollectionOptionsPostConfigurator
    {
        public readonly static SynchronizingCollectionOptionsPostConfigurator Default = new SynchronizingCollectionOptionsPostConfigurator();

        public void PostConfigure<TItem>(
            ISynchronizingCollectionItemsOptions<TItem> itemsOptions,
            out ICollectionChangeHandler<TItem> collectionChangeHandler,
            Func<ICollectionChangeHandler<TItem>, ISynchronizedCollection<TItem>> synchronizedItemsFactory,
            out ISynchronizedCollection<TItem> synchronizedItems)
            where TItem : notnull
        {
            SynchronizableCollectionItemsOptionsPostConfigurator.Default.PostConfigure(itemsOptions, out collectionChangeHandler);

            var itemOptionsSynchronizedItems = itemsOptions.SynchronizedItems;

            if (!(itemOptionsSynchronizedItems is null)) {
                synchronizedItems = itemOptionsSynchronizedItems;
                return;
            }

            synchronizedItems = synchronizedItemsFactory(collectionChangeHandler);
            itemsOptions.SetItems(synchronizedItems, collectionChangeHandler);
        }
    }
}

