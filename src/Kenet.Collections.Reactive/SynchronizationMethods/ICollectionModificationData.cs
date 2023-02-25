// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Specialized;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    public interface ICollectionModificationData
    {
        NotifyCollectionChangedAction Action { get; }
        int? OldItemsCount { get; }
        int OldIndex { get; }
        int? NewItemsCount { get; }
        int NewIndex { get; }
    }
}

