// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Kenet.Collections.Reactive.SynchronizationMethods;

namespace Kenet.Collections.Reactive
{
    public abstract partial class SynchronizingCollectionBase<TSuperItem, TSubItem>
    {
        public abstract partial class ItemCollection<TItem, TNewItem> : SynchronizableCollectionBase<TItem, TNewItem>
        {
            public ItemCollection(IListMutationTarget<TItem> itemsMutationTarget, ICollectionItemsOptions<TItem> options, SynchronizingCollectionBase<TSuperItem, TSubItem> items)
                : base(itemsMutationTarget, options)
            {
                items.CollectionSynchronizing += SynchronizingCollection_CollectionSynchronizing;
                items.CollectionModified += CollectionModificationNotifier_CollectionModified;
                items.CollectionSynchronized += SynchronizingCollection_CollectionSynchronized;
            }

            private void SynchronizingCollection_CollectionSynchronizing(object? sender, EventArgs e) =>
                OnCollectionSynchronizing();

            protected abstract ICollectionModification<TItem, TItem> GetCollectionModification(CollectionModifiedEventArgs<TSuperItem, TSubItem> args);

            private void CollectionModificationNotifier_CollectionModified(object? sender, CollectionModifiedEventArgs<TSuperItem, TSubItem> args)
            {
                var collectionModification = GetCollectionModification(args);
                OnCollectionModified(collectionModification);
            }

            private void SynchronizingCollection_CollectionSynchronized(object? sender, EventArgs e) =>
                OnCollectionSynchronized();
        }

        public class SubItemCollection : ItemCollection<TSubItem, TSuperItem>
        {
            public SubItemCollection(IListMutationTarget<TSubItem> itemsMutationTarget, ICollectionItemsOptions<TSubItem> options, SynchronizingCollectionBase<TSuperItem, TSubItem> items)
                : base(itemsMutationTarget, options, items) { }

            protected override ICollectionModification<TSubItem, TSubItem> GetCollectionModification(CollectionModifiedEventArgs<TSuperItem, TSubItem> args) =>
                args.SubItemModification;
        }

        public class SuperItemCollection : ItemCollection<TSuperItem, TSubItem>
        {
            public SuperItemCollection(IListMutationTarget<TSuperItem> itemsMutationTarget, ICollectionItemsOptions<TSuperItem> options, SynchronizingCollectionBase<TSuperItem, TSubItem> items)
                : base(itemsMutationTarget, options, items) { }

            protected override ICollectionModification<TSuperItem, TSuperItem> GetCollectionModification(CollectionModifiedEventArgs<TSuperItem, TSubItem> args) =>
                args.SuperItemModification;
        }
    }
}
