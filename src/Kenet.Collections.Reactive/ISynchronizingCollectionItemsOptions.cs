// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Kenet.Collections.Reactive
{
    internal interface ISynchronizingCollectionItemsOptions<TItem> : ICollectionItemsOptions<TItem>
    {
        ISynchronizedCollection<TItem>? SynchronizedItems { get; }

        void SetItems(ISynchronizedCollection<TItem> synchronizedItems, IMutableList<TItem> items);
    }
}
