// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Kenet.Collections.Reactive.SynchronizationMethods;

namespace Kenet.Collections.Reactive
{
    public abstract class AbstractCollectionOptions<TDerived, TItem>
        where TDerived : AbstractCollectionOptions<TDerived, TItem>
        where TItem : notnull
    {
        public ICollectionSynchronizationMethod<TItem, TItem>? SynchronizationMethod { get; set; }

        public TDerived SetSynchronizationMethod(ICollectionSynchronizationMethod<TItem, TItem> synchronizationMethod)
        {
            SynchronizationMethod = synchronizationMethod;
            return (TDerived)this;
        }

        public TDerived SetSequentialSynchronizationMethod(IEqualityComparer<TItem> equalityComparer)
        {
            SynchronizationMethod = CollectionSynchronizationMethod.Sequential(equalityComparer);
            return (TDerived)this;
        }

        public TDerived SetSortedSynchronizationMethod(IComparer<TItem> equalityComparer)
        {
            SynchronizationMethod = CollectionSynchronizationMethod.Sorted(equalityComparer);
            return (TDerived)this;
        }
    }
}
