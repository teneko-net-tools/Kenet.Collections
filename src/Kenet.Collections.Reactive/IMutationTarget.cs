// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Kenet.Collections.Reactive
{
    public interface IMutationTarget<TItem> {
        void InsertItem(int insertAt, TItem item);
        void MoveItems(int fromIndex, int toIndex, int count);
        void RemoveItem(int removeAt);
        void ReplaceItem(int replaceAt, Func<TItem> getNewItem);
        void ResetItems();
    }
}
