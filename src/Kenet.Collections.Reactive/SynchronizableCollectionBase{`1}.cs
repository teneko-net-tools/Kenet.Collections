﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Teronis.Collections.Algorithms.Modifications;
using Teronis.ComponentModel;
using System.Reactive.Subjects;

namespace Teronis.Collections.Synchronization
{
    public abstract class SynchronizableCollectionBase<TItem, TNewItem> : Collection<TItem>, ISynchronizedCollection<TItem>
    {
        /* Related to observable collection. */
        internal protected const string CountString = "Count";
        /// <summary>
        /// See https://docs.microsoft.com/en-us/archive/blogs/xtof/binding-to-indexers.
        /// </summary>
        internal protected const string IndexerName = "Item[]";

        public event PropertyChangedEventHandler? PropertyChanged {
            add => ChangeComponent.PropertyChanged += value;
            remove => ChangeComponent.PropertyChanged -= value;
        }

        public event PropertyChangingEventHandler? PropertyChanging {
            add => ChangeComponent.PropertyChanging += value;
            remove => ChangeComponent.PropertyChanging -= value;
        }

        public event EventHandler? CollectionSynchronizing;
        public event NotifyNotifyCollectionModifiedEventHandler<TItem>? CollectionModified;

        private event NotifyCollectionChangedEventHandler? collectionChanged;

        event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged {
            add => collectionChanged += value;
            remove => collectionChanged -= value;
        }

        public event EventHandler? CollectionSynchronized;

        protected PropertyChangeComponent ChangeComponent { get; private set; }

        private OccupationMonitor occupationMonitor;
        // We take the Subject<> implementation because it provides full thread-safety.
        private Subject<ICollectionModification<TItem, TItem>> collectionModificationSubject;
        private readonly ICollectionChangeHandler<TItem> changeHandler;

        public SynchronizableCollectionBase(ICollectionChangeHandler<TItem> changeHandler, IReadOnlyCollectionItemsOptions options)
            : base(changeHandler.Items)
        {
            Initialize();
            this.changeHandler = changeHandler;
            changeHandler.RedirectInsert += ChangeHandler_RedirectInsert;
            changeHandler.RedirectRemove += ChangeHandler_RedirectRemove;
            changeHandler.RedirectReplace += ChangeHandler_RedirectReplace;
            changeHandler.RedirectMove += ChangeHandler_RedirectMove;
            changeHandler.RedirectReset += ChangeHandler_RedirectReset;
        }

        public SynchronizableCollectionBase()
        {
            changeHandler = new CollectionChangeHandler<TItem>(Items);
            Initialize();
        }

        [MemberNotNull(nameof(occupationMonitor), nameof(ChangeComponent), nameof(collectionModificationSubject))]
        private void Initialize()
        {
            occupationMonitor = new OccupationMonitor();
            ChangeComponent = new PropertyChangeComponent(this);
            collectionModificationSubject = new Subject<ICollectionModification<TItem, TItem>>();
        }

        protected virtual void OnCollectionSynchronizing() =>
            CollectionSynchronizing?.Invoke(this, new EventArgs());

        protected virtual void OnCollectionSynchronized() =>
            CollectionSynchronized?.Invoke(this, new EventArgs());

        protected virtual CollectionModifiedEventArgs<TItem> CreateCollectionModifiedEventArgs(ICollectionModification<TItem, TItem> modification) =>
            new CollectionModifiedEventArgs<TItem>(modification);

        protected IDisposable BlockReentrancy()
        {
            occupationMonitor.Occupy();
            return occupationMonitor;
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
            if (occupationMonitor.IsOccupied) {
                int invocationCount = collectionChanged?.GetInvocationList().Length ?? 0
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
            ChangeComponent.OnPropertyChanging(CountString, IndexerName);

        protected virtual void OnAfterAddItem(int itemIndex, TItem item) =>
            ChangeComponent.OnPropertyChanged(CountString, IndexerName);

        protected override void InsertItem(int itemIndex, TItem item)
        {
            CheckReentrancy();
            OnBeforeAddItem(itemIndex, item);
            changeHandler.InsertItem(itemIndex, item, preventInsertRedirect: true);
            var modification = CollectionModification.ForAdd<TItem, TItem>(itemIndex, item);
            OnCollectionModified(modification);
            OnAfterAddItem(itemIndex, item);
        }

        private void ChangeHandler_RedirectInsert(int insertAt, TItem item) =>
            InsertItem(insertAt, item);

        protected virtual void OnBeforeRemoveItem(int itemIndex) =>
            ChangeComponent.OnPropertyChanging(CountString, IndexerName);

        protected virtual void OnAfterRemoveItem(int itemIndex) =>
            ChangeComponent.OnPropertyChanged(CountString, IndexerName);

        protected override void RemoveItem(int itemIndex)
        {
            CheckReentrancy();
            OnBeforeRemoveItem(itemIndex);
            var oldItem = Items[itemIndex];
            changeHandler.RemoveItem(itemIndex, preventRemoveRedirect: true);
            var modification = CollectionModification.ForRemove<TItem, TItem>(itemIndex, oldItem);
            OnCollectionModified(modification);
            OnAfterRemoveItem(itemIndex);
        }

        private void ChangeHandler_RedirectRemove(int removeAt) =>
            RemoveItem(removeAt);

        protected virtual void OnBeforeReplaceItem(int itemIndex) =>
            ChangeComponent.OnPropertyChanging(IndexerName);

        protected virtual void OnAfterReplaceItem(int itemIndex) =>
            ChangeComponent.OnPropertyChanged(IndexerName);

        protected override void SetItem(int itemIndex, TItem item)
        {
            CheckReentrancy();
            OnBeforeReplaceItem(itemIndex);
            var oldItem = Items[itemIndex];
            changeHandler.ReplaceItem(itemIndex, () => item, preventReplaceRedirect: true);
            var modification = CollectionModification.ForReplace(itemIndex, oldItem, item);
            OnCollectionModified(modification);
            OnAfterReplaceItem(itemIndex);
        }

        private void ChangeHandler_RedirectReplace(int replaceAt, Func<TItem> getNewItem) =>
            SetItem(replaceAt, getNewItem());

        protected virtual void OnBeforeMoveItems() =>
            ChangeComponent.OnPropertyChanging(IndexerName);

        protected virtual void OnAfterMoveItems() =>
            ChangeComponent.OnPropertyChanged(IndexerName);

        protected virtual void MoveItems(int fromIndex, int toIndex, int count)
        {
            CheckReentrancy();
            OnBeforeMoveItems();
            changeHandler.MoveItems(fromIndex, toIndex, count, preventMoveRedirect: true);
            var modification = CollectionModification.ForMove<TItem, TItem>(fromIndex, Items, toIndex, count);
            OnCollectionModified(modification);
            OnAfterMoveItems();
        }

        private void ChangeHandler_RedirectMove(int fromIndex, int toIndex, int count) =>
            MoveItems(fromIndex, toIndex, count);

        public void Move(int fromIndex, int toIndex, int count) =>
            MoveItems(fromIndex, toIndex, count);

        public void Move(int fromIndex, int toIndex) =>
            MoveItems(fromIndex, toIndex, 1);

        protected virtual void OnBeforeResetItems() =>
            ChangeComponent.OnPropertyChanging(CountString, IndexerName);

        protected virtual void OnAfterResetItems() =>
            ChangeComponent.OnPropertyChanged(CountString, IndexerName);

        protected override void ClearItems()
        {
            CheckReentrancy();
            OnBeforeResetItems();
            changeHandler.ResetItems(preventResetRedirect: true);
            OnCollectionModified(CollectionModification.ForReset<TItem, TItem>());
            OnAfterResetItems();
        }

        private void ChangeHandler_RedirectReset() =>
            ClearItems();

        public IDisposable Subscribe(IObserver<ICollectionModification<TItem, TItem>> observer) =>
            collectionModificationSubject.Subscribe(observer);

        public SynchronizedDictionary<TKey, TItem> ToSynchronizedDictionary<TKey>(Func<TItem, TKey> getItemKey, IEqualityComparer<TKey> keyEqualityComparer)
            where TKey : notnull =>
            new SynchronizedDictionary<TKey, TItem>(this, getItemKey, keyEqualityComparer);

        public SynchronizedDictionary<KeyType, TItem> ToSynchronizedDictionary<KeyType>(Func<TItem, KeyType> getItemKey)
            where KeyType : notnull =>
            new SynchronizedDictionary<KeyType, TItem>(this, getItemKey);

        /// <summary>
        /// Helps to detect reentrances.
        /// </summary>
        private class OccupationMonitor : IDisposable
        {
            int occupationCount;

            public void Occupy() =>
                ++occupationCount;

            public void Dispose() =>
                --occupationCount;

            public bool IsOccupied =>
                occupationCount > 0;
        }
    }
}
