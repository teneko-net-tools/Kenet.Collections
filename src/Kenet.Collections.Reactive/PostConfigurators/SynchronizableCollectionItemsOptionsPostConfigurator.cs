// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Reactive.PostConfigurators
{
    internal class SynchronizableCollectionItemsOptionsPostConfigurator
    {
        public static readonly SynchronizableCollectionItemsOptionsPostConfigurator Default = new();

        public void PostConfigure<TItem>(
            ICollectionItemsOptions<TItem> itemsOptions, out IListMutationTarget<TItem> itemsMutationTarget)
            where TItem : notnull
        {
            var itemOptionsCollectionChangeHandler = itemsOptions.ItemsMutationTarget;

            if (!(itemOptionsCollectionChangeHandler is null)) {
                itemsMutationTarget = itemOptionsCollectionChangeHandler;
                return;
            }

            var items = new List<TItem>();
            itemsMutationTarget = new ListMutationTarget<TItem>(items);
            itemsOptions.SetItems(itemsMutationTarget);
        }
    }
}
