// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Teronis.Collections.Synchronization
{
    // TODO: Consider to remove <TItem>.
    public interface INotifyCollectionSynchronized<TItem>
    {
        event EventHandler CollectionSynchronized;
    }
}
