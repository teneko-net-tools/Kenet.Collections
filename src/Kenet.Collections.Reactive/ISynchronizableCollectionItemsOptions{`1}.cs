// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Kenet.Collections.Reactive
{
    public interface ICollectionItemsOptions<TItem> : IReadOnlyCollectionItemsOptions
    {
        IMutableList<TItem>? Items { get; }

        void SetItems(IMutableList<TItem>? list);
    }
}
