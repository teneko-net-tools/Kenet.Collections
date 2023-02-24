// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Kenet.Collections.Synchronization
{
    public delegate void NotifyCollectionModifiedEventHandler<TSuperItem, TSubItem>(object sender, CollectionModifiedEventArgs<TSuperItem, TSubItem> args);
}
