// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Kenet.Collections.Reactive
{
    public class SynchronizedCollectionMirror<TSuperItem>
        where TSuperItem : notnull
    {
        private readonly ICollectionSynchronizationContext<TSuperItem> _source;

        internal SynchronizedCollectionMirror(ICollectionSynchronizationContext<TSuperItem> target, ISynchronizedCollection<TSuperItem> source)
        {
            source.CollectionSynchronizing += ToBeMirroredCollection_CollectionSynchronizing;
            source.CollectionModified += ToBeMirroredCollection_CollectionModified;
            source.CollectionSynchronized += ToBeMirroredCollection_CollectionSynchronized;
            _source = target;
        }

        private void ToBeMirroredCollection_CollectionSynchronizing(object? sender, EventArgs e) =>
            _source.BeginCollectionSynchronization();

        private void ToBeMirroredCollection_CollectionModified(object? sender, CollectionModifiedEventArgs<TSuperItem> e) =>
            _source.ProcessModification(e);

        private void ToBeMirroredCollection_CollectionSynchronized(object? sender, EventArgs e) =>
            _source.EndCollectionSynchronization();
    }
}
