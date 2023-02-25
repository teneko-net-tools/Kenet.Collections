// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Kenet.Collections.Reactive.SynchronizationMethods;

namespace Kenet.Collections.Reactive
{
    public interface ISynchronizedCollection<TItem> :
        INotifyPropertyChanged, INotifyPropertyChanging,
        IReadOnlyList<TItem>,
        INotifyCollectionSynchronizing<TItem>, INotifyCollectionModified<TItem>, INotifyCollectionChanged, INotifyCollectionSynchronized<TItem>,
        IObservable<ICollectionModification<TItem, TItem>>
    {
        new TItem this[int index] { get; set; }
        new int Count { get; }
        new IEnumerator<TItem> GetEnumerator();
    }
}
