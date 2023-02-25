// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Kenet.Collections.Reactive;

namespace Kenet.Collections.Algorithms
{
    public interface INotifyCollectionModification<TSuperItem, TSubItem>
    {
        event NotifyCollectionModifiedEventHandler<TSuperItem, TSubItem> CollectionModified;
    }
}
