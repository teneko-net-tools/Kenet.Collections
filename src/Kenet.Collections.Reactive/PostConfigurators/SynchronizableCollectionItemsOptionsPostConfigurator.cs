// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Reactive.PostConfigurators
{
    internal class SynchronizableCollectionItemsOptionsPostConfigurator
    {
        public static readonly SynchronizableCollectionItemsOptionsPostConfigurator Default = new();

        public void PostConfigure<TItem>(
            ICollectionItemsOptions<TItem> itemsOptions, out IMutableList<TItem> collectionChangeHandler)
            where TItem : notnull
        {
            var itemOptionsCollectionChangeHandler = itemsOptions.Items;

            if (!(itemOptionsCollectionChangeHandler is null)) {
                collectionChangeHandler = itemOptionsCollectionChangeHandler;
                return;
            }

            var items = new List<TItem>();
            collectionChangeHandler = new MutableList<TItem>(items);
            itemsOptions.SetItems(collectionChangeHandler);
        }
    }
}
