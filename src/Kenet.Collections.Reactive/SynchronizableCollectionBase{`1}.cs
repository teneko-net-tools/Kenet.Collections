// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;
using Kenet.Collections.Reactive.SynchronizationMethods;
using Teronis.ComponentModel;

namespace Kenet.Collections.Reactive
{
    public abstract class SynchronizableCollectionBase<TItem, TNewItem> : Collection<TItem>, ISynchronizedCollection<TItem>
    {
        /* Related to observable collection. */
        protected internal const string CountPropertyName = "Count";

        /// <summary>
        /// See https://docs.microsoft.com/en-us/archive/blogs/xtof/binding-to-indexers.
        /// </summary>
        protected internal const string IndexerPropertyName = "Item[]";

        public event PropertyChangedEventHandler? PropertyChanged {
            add => ChangeComponent.PropertyChanged += value;
            remove => ChangeComponent.PropertyChanged -= value;
        }

        public event PropertyChangingEventHandler? PropertyChanging {
            add => ChangeComponent.PropertyChanging += value;
            remove => ChangeComponent.PropertyChanging -= value;
        }

        public event EventHandler? CollectionSynchronizing;
        public event NotifyCollectionModifiedEventHandler<TItem>? CollectionModified;

        private event NotifyCollectionChangedEventHandler? collectionChanged;

        event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged {
            add => collectionChanged += value;
            remove => collectionChanged -= value;
        }

        public event EventHandler? CollectionSynchronized;

        protected PropertyChangeComponent ChangeComponent { get; private set; }

        private OccupationMonitor _occupationMonitor;
        // We take the Subject<> implementation because it provides full thread-safety.
        private Subject<ICollectionModification<TItem, TItem>> collectionModificationSubject;
        private readonly IListMutationTarget<TItem> _itemsMutationTarget;

        public SynchronizableCollectionBase(IListMutationTarget<TItem> itemsMutationTarget, IReadOnlyCollectionItemsOptions options)
            : base(itemsMutationTarget.Items)
        {
            Initialize();
            _itemsMutationTarget = itemsMutationTarget;
            var collectionMutationTargetAdapter = new CollectionMutationTargetAdapter(this);

            void Mutate(object? _, Action<IMutationTarget<TItem>> collect) =>
                collect(collectionMutationTargetAdapter);

            itemsMutationTarget.CollectRedirectionTargets += Mutate;
        }

        public SynchronizableCollectionBase()
        {
            _itemsMutationTarget = new ListMutationTarget<TItem>(Items);
            Initialize();
        }

        [MemberNotNull(nameof(_occupationMonitor), nameof(ChangeComponent), nameof(collectionModificationSubject))]
        private void Initialize()
        {
            _occupationMonitor = new OccupationMonitor();
            ChangeComponent = new PropertyChangeComponent(this);
            collectionModificationSubject = new Subject<ICollectionModification<TItem, TItem>>();
        }

        protected virtual void OnCollectionSynchronizing() =>
            CollectionSynchronizing?.Invoke(this, new EventArgs());

        protected virtual void OnCollectionSynchronized() =>
            CollectionSynchronized?.Invoke(this, new EventArgs());

        protected virtual CollectionModifiedEventArgs<TItem> CreateCollectionModifiedEventArgs(ICollectionModification<TItem, TItem> modification) =>
            new(modification);

        protected IDisposable BlockReentrancy()
        {
            _occupationMonitor.Occupy();
            return _occupationMonitor;
        }

        /// <summary>
        /// Notifies all handlers of CollectionChanged and CollectionModified and all subscribed observers.
        /// Early returns if no handlers for CollectionChanged, CollectionModified attached and no observers have been subscribed.
        /// </summary>
        /// <param name="collectionModification"></param>
        protected virtual void OnCollectionModified(ICollectionModification<TItem, TItem> collectionModification)
        {
            if (collectionChanged is null && CollectionModified is null && !collectionModificationSubject.HasObservers) {
                return;
            }

            var collectionChangedEventArgs = CreateCollectionModifiedEventArgs(collectionModification);
            using var _ = BlockReentrancy();
            collectionChanged?.Invoke(this, collectionChangedEventArgs);
            CollectionModified?.Invoke(this, collectionChangedEventArgs);

            if (collectionModificationSubject.HasObservers) {
                collectionModificationSubject.OnNext(collectionModification);
            }
        }

        /// <summary>
        /// Check and assert for reentrant attempts to change this collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Raised when changing the collection while another collection change is still being 
        /// notified to other listeners.
        /// </exception>
        protected void CheckReentrancy()
        {
            if (_occupationMonitor.IsOccupied) {
                var invocationCount = collectionChanged?.GetInvocationList().Length ?? 0
                    + CollectionModified?.GetInvocationList().Length ?? 0;

                // Excerpt from https://github.com/microsoft/referencesource/blob/5697c29004a34d80acdaf5742d7e699022c64ecd/System/compmod/system/collections/objectmodel/observablecollection.cs:
                // We can allow changes if there's only one listener - the problem
                // only arises if reentrant changes make the original event args
                // invalid for later listeners.  This keeps existing code working
                // (e.g. Selector.SelectedItems).
                if (1 < invocationCount) {
                    throw new InvalidOperationException("Observable collection reentrancy is not allowed.");
                }
            }
        }

        protected virtual void OnBeforeAddItem(int itemIndex, TItem item) =>
            ChangeComponent.OnPropertyChanging(CountPropertyName, IndexerPropertyName);

        protected virtual void OnAfterAddItem(int itemIndex, TItem item) =>
            ChangeComponent.OnPropertyChanged(CountPropertyName, IndexerPropertyName);

        protected override void InsertItem(int itemIndex, TItem item)
        {
            CheckReentrancy();
            OnBeforeAddItem(itemIndex, item);
            _itemsMutationTarget.Mutate(collection => collection.InsertItem(itemIndex, item), disableRedirection: true);
            var modification = CollectionModification.ForAdd<TItem, TItem>(itemIndex, item);
            OnCollectionModified(modification);
            OnAfterAddItem(itemIndex, item);
        }

        protected virtual void OnBeforeRemoveItem(int itemIndex) =>
            ChangeComponent.OnPropertyChanging(CountPropertyName, IndexerPropertyName);

        protected virtual void OnAfterRemoveItem(int itemIndex) =>
            ChangeComponent.OnPropertyChanged(CountPropertyName, IndexerPropertyName);

        protected override void RemoveItem(int itemIndex)
        {
            CheckReentrancy();
            OnBeforeRemoveItem(itemIndex);
            var oldItem = Items[itemIndex];
            _itemsMutationTarget.Mutate(collection => collection.RemoveItem(itemIndex), disableRedirection: true);
            var modification = CollectionModification.ForRemove<TItem, TItem>(itemIndex, oldItem);
            OnCollectionModified(modification);
            OnAfterRemoveItem(itemIndex);
        }

        protected virtual void OnBeforeReplaceItem(int itemIndex) =>
            ChangeComponent.OnPropertyChanging(IndexerPropertyName);

        protected virtual void OnAfterReplaceItem(int itemIndex) =>
            ChangeComponent.OnPropertyChanged(IndexerPropertyName);

        protected override void SetItem(int itemIndex, TItem item)
        {
            CheckReentrancy();
            OnBeforeReplaceItem(itemIndex);
            var oldItem = Items[itemIndex];
            _itemsMutationTarget.Mutate(collection => collection.ReplaceItem(itemIndex, () => item), disableRedirection: true);
            var modification = CollectionModification.ForReplace(itemIndex, oldItem, item);
            OnCollectionModified(modification);
            OnAfterReplaceItem(itemIndex);
        }

        protected virtual void OnBeforeMoveItems() =>
            ChangeComponent.OnPropertyChanging(IndexerPropertyName);

        protected virtual void OnAfterMoveItems() =>
            ChangeComponent.OnPropertyChanged(IndexerPropertyName);

        protected virtual void MoveItems(int fromIndex, int toIndex, int count)
        {
            CheckReentrancy();
            OnBeforeMoveItems();
            _itemsMutationTarget.Mutate(collection => collection.MoveItems(fromIndex, toIndex, count), disableRedirection: true);
            var modification = CollectionModification.ForMove<TItem, TItem>(fromIndex, Items, toIndex, count);
            OnCollectionModified(modification);
            OnAfterMoveItems();
        }

        public void Move(int fromIndex, int toIndex, int count) =>
            MoveItems(fromIndex, toIndex, count);

        public void Move(int fromIndex, int toIndex) =>
            MoveItems(fromIndex, toIndex, 1);

        protected virtual void OnBeforeResetItems() =>
            ChangeComponent.OnPropertyChanging(CountPropertyName, IndexerPropertyName);

        protected virtual void OnAfterResetItems() =>
            ChangeComponent.OnPropertyChanged(CountPropertyName, IndexerPropertyName);

        protected override void ClearItems()
        {
            CheckReentrancy();
            OnBeforeResetItems();
            _itemsMutationTarget.Mutate(collection => collection.ResetItems(), disableRedirection: true);
            OnCollectionModified(CollectionModification.ForReset<TItem, TItem>());
            OnAfterResetItems();
        }

        public IDisposable Subscribe(IObserver<ICollectionModification<TItem, TItem>> observer) =>
            collectionModificationSubject.Subscribe(observer);

        public SynchronizedDictionary<TKey, TItem> ToSynchronizedDictionary<TKey>(Func<TItem, TKey> getItemKey, IEqualityComparer<TKey> keyEqualityComparer)
            where TKey : notnull =>
            new(this, getItemKey, keyEqualityComparer);

        public SynchronizedDictionary<KeyType, TItem> ToSynchronizedDictionary<KeyType>(Func<TItem, KeyType> getItemKey)
            where KeyType : notnull =>
            new(this, getItemKey);

        /// <summary>
        /// Helps to detect reentrances.
        /// </summary>
        private class OccupationMonitor : IDisposable
        {
            private int _occupationCount;

            public void Occupy() =>
                ++_occupationCount;

            public void Dispose() =>
                --_occupationCount;

            public bool IsOccupied =>
                _occupationCount > 0;
        }

        private class CollectionMutationTargetAdapter : IMutationTarget<TItem>
        {
            private readonly SynchronizableCollectionBase<TItem, TNewItem> _collection;

            public CollectionMutationTargetAdapter(SynchronizableCollectionBase<TItem, TNewItem> collection) =>
                _collection = collection;

            public void InsertItem(int insertAt, TItem item) =>
                _collection.InsertItem(insertAt, item);

            public void MoveItems(int fromIndex, int toIndex, int count) =>
                _collection.MoveItems(fromIndex, toIndex, count);

            public void RemoveItem(int removeAt) =>
                _collection.RemoveItem(removeAt);

            public void ReplaceItem(int replaceAt, Func<TItem> getNewItem) =>
                _collection.SetItem(replaceAt, getNewItem());

            public void ResetItems() =>
                _collection.ClearItems();
        }
    }
}
