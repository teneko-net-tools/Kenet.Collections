﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Kenet.Collections.Algorithms.Modifications;
using Kenet.Collections.Synchronization.PostConfigurators;

namespace Kenet.Collections.Synchronization
{
    public partial class SynchronizableCollection<TItem> : SynchronizableCollectionBase<TItem, TItem>, ICollectionSynchronizationContext<TItem>
        where TItem : notnull
    {
        private static SynchronizableCollectionOptions<TItem> ConfigureOptions(
            [NotNull] ref SynchronizableCollectionOptions<TItem>? options,
            out ICollectionChangeHandler<TItem> collectionChangeHandler)
        {
            options ??= new SynchronizableCollectionOptions<TItem>();
            SynchronizableCollectionItemsOptionsPostConfigurator.Default.PostConfigure(options.ItemsOptions, out collectionChangeHandler);
            return options;
        }

        public ICollectionSynchronizationMethod<TItem, TItem> SynchronizationMethod { get; private set; } = null!;

        private ICollectionChangeHandler<TItem> collectionChangeHandler;
        private CollectionUpdateItemDelegate<TItem, TItem>? itemUpdateHandler;

        private SynchronizableCollection(SynchronizableCollectionOptions<TItem> options, ICollectionChangeHandler<TItem> collectionChangeHandler)
            : base(collectionChangeHandler, options.ItemsOptions)
        {
            options.SynchronizationMethod ??= CollectionSynchronizationMethod.Sequential<TItem>();
            SynchronizationMethod = options.SynchronizationMethod;
            this.collectionChangeHandler = collectionChangeHandler;
            itemUpdateHandler = options.ItemsOptions.ItemUpdateHandler;
        }

        public SynchronizableCollection(SynchronizableCollectionOptions<TItem>? options)
            : this(ConfigureOptions(ref options, out var collectionChangeHandler), collectionChangeHandler) { }

        public SynchronizableCollection(
            IList<TItem> items,
            ICollectionSynchronizationMethod<TItem, TItem>? synchronizationMethod) : this(
                new SynchronizableCollectionOptions<TItem>() { SynchronizationMethod = synchronizationMethod }
                    .ConfigureItems(options => options
                        .SetItems(items)))
        { }

        public SynchronizableCollection(IList<TItem> items) : this(
            new SynchronizableCollectionOptions<TItem>()
                .ConfigureItems(options => options
                    .SetItems(items)))
        { }

        public SynchronizableCollection(ICollectionSynchronizationMethod<TItem, TItem> synchronizationMethod)
            : this(new SynchronizableCollectionOptions<TItem>() { SynchronizationMethod = synchronizationMethod }) { }

        public SynchronizableCollection()
            : this(options: null) { }

        public SynchronizableCollection(IList<TItem> items, IEqualityComparer<TItem> equalityComparer) : this(
            new SynchronizableCollectionOptions<TItem>()
                .ConfigureItems(options => options
                    .SetItems(items))
                .SetSequentialSynchronizationMethod(equalityComparer))
        { }

        public SynchronizableCollection(IEqualityComparer<TItem> equalityComparer)
            : this(new SynchronizableCollectionOptions<TItem>()
                  .SetSequentialSynchronizationMethod(equalityComparer))
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="comparer"></param>
        public SynchronizableCollection(IList<TItem> items, IComparer<TItem> comparer) : this(
            new SynchronizableCollectionOptions<TItem>()
                .ConfigureItems(options => options
                    .SetItems(items))
                .SetSortedSynchronizationMethod(comparer))
        { }

        public SynchronizableCollection(IComparer<TItem> comparer)
            : this(new SynchronizableCollectionOptions<TItem>()
                  .SetSortedSynchronizationMethod(comparer))
        { }

        protected virtual void AddItems(ICollectionModification<TItem, TItem> modification)
        {
            CollectionModificationIterationTools.BeginInsert(modification)
                // The modification is now null checked.
                .OnIteration(iterationContext => {
                    CheckReentrancy();
                    var newItem = modification.NewItems![iterationContext.ModificationItemIndex];
                    OnBeforeAddItem(iterationContext.CollectionItemIndex, newItem);
                    collectionChangeHandler.InsertItem(iterationContext.CollectionItemIndex, newItem);
                    OnCollectionModified(modification);
                    OnAfterAddItem(iterationContext.CollectionItemIndex, newItem);
                })
                .Iterate();
        }

        protected virtual void RemoveItems(ICollectionModification<TItem, TItem> modification)
        {
            CollectionModificationIterationTools.BeginRemove(modification)
                .OnIteration(iterationContext => {
                    CheckReentrancy();
                    OnBeforeRemoveItem(iterationContext.CollectionItemIndex);
                    collectionChangeHandler.RemoveItem(iterationContext.CollectionItemIndex);
                    OnCollectionModified(modification);
                    OnAfterRemoveItem(iterationContext.CollectionItemIndex);
                })
                .Iterate();
        }

        protected virtual void ReplaceItems(ICollectionModification<TItem, TItem> modification)
        {
            CollectionModificationIterationTools.BeginReplace(modification)
                .OnIteration(iterationContext => {
                    var lazyItem = new SlimLazy<TItem>(() => modification.NewItems![iterationContext.ModificationItemIndex]);

                    if (collectionChangeHandler.CanReplaceItem) {
                        CheckReentrancy();
                        OnBeforeReplaceItem(iterationContext.CollectionItemIndex);
                        collectionChangeHandler.ReplaceItem(iterationContext.CollectionItemIndex, lazyItem.GetValue);
                        OnCollectionModified(modification);
                        OnBeforeReplaceItem(iterationContext.CollectionItemIndex);
                    }

                    itemUpdateHandler?.Invoke(collectionChangeHandler.Items[iterationContext.CollectionItemIndex], lazyItem.GetValue);
                })
                .Iterate();
        }

        protected virtual void MoveItems(ICollectionModification<TItem, TItem> modification)
        {
            CollectionModificationIterationTools.CheckMove(modification);
            CheckReentrancy();
            OnBeforeMoveItems();
            collectionChangeHandler.MoveItems(modification.OldIndex, modification.NewIndex, modification.OldItems!.Count);
            OnCollectionModified(modification);
            OnAfterMoveItems();
        }

        protected virtual void ResetItems(ICollectionModification<TItem, TItem> modification)
        {
            CheckReentrancy();
            OnBeforeResetItems();
            collectionChangeHandler.ResetItems();
            OnCollectionModified(modification);
            OnAfterResetItems();
        }

        protected virtual void ProcessModification(ICollectionModification<TItem, TItem> modification)
        {
            switch (modification.Action) {
                case NotifyCollectionChangedAction.Add:
                    AddItems(modification);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItems(modification);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ReplaceItems(modification);
                    break;
                case NotifyCollectionChangedAction.Move:
                    MoveItems(modification);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetItems(modification);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftItems"></param>
        /// <param name="rightItems"></param>
        /// <param name="yieldCapabilities"></param>
        /// <param name="batchModifications">Indicates that first all modifications are calculated before they get processed.</param>
        internal void SynchronizeCollection(IEnumerable<TItem> leftItems, IEnumerable<TItem>? rightItems, CollectionModificationYieldCapabilities yieldCapabilities, bool batchModifications)
        {
            leftItems = leftItems ?? throw new ArgumentNullException(nameof(leftItems));
            var modifications = SynchronizationMethod.YieldCollectionModifications(leftItems, rightItems, yieldCapabilities);

            if (batchModifications) {
                modifications = modifications.ToList();
            }

            var modificationEnumerator = modifications.GetEnumerator();

            if (!modificationEnumerator.MoveNext()) {
                return;
            }

            OnCollectionSynchronizing();

            do {
                var modification = modificationEnumerator.Current;
                ProcessModification(modification);
            } while (modificationEnumerator.MoveNext());

            OnCollectionSynchronized();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="yieldCapabilities"></param>
        /// <param name="batchModifications">Indicates that first all modifications are calculated before they get processed.</param>
        internal void SynchronizeCollection(IEnumerable<TItem>? enumerable, CollectionModificationYieldCapabilities yieldCapabilities, bool batchModifications) =>
            SynchronizeCollection(
                batchModifications
                    ? Items // When we batch modifications, then we do not need to mark it.
                    : Items.AsIList().ToProducedListModificationsNotBatchedMarker(),
                enumerable,
                yieldCapabilities,
                batchModifications);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="batchModifications">Indicates that first all modifications are calculated before they get processed.</param>
        internal void SynchronizeCollection(IEnumerable<TItem>? enumerable, bool batchModifications) =>
            SynchronizeCollection(enumerable, CollectionModificationYieldCapabilities.All, batchModifications);

        public void SynchronizeCollection(IEnumerable<TItem>? enumerable, CollectionModificationYieldCapabilities yieldCapabilities) =>
            SynchronizeCollection(enumerable, yieldCapabilities, batchModifications: false);

        public void SynchronizeCollection(IEnumerable<TItem>? enumerable) =>
            SynchronizeCollection(enumerable, yieldCapabilities: CollectionModificationYieldCapabilities.All);

        /// <summary>
        /// Creates for this instance a collection synchronisation mirror. The collection modifications from <paramref name="toBeMirroredCollection"/> are 
        /// forwarded to <see cref="ProcessModification(ICollectionModification{TItem, TItem})"/>
        /// of this instance.
        /// </summary>
        /// <param name="toBeMirroredCollection">The foreign collection that is about to be mirrored related to its modifications.</param>
        /// <returns>A collection synchronization mirror.</returns>
        public SynchronizedCollectionMirror<TItem> MirrorSynchronizedCollection(ISynchronizedCollection<TItem> toBeMirroredCollection) =>
            new SynchronizedCollectionMirror<TItem>(this, toBeMirroredCollection);

        #region ICollectionSynchronizationContext<SuperItemType>

        void ICollectionSynchronizationContext<TItem>.BeginCollectionSynchronization() =>
            OnCollectionSynchronizing();

        void ICollectionSynchronizationContext<TItem>.ProcessModification(ICollectionModification<TItem, TItem> superItemModification) =>
            ProcessModification(superItemModification);

        void ICollectionSynchronizationContext<TItem>.EndCollectionSynchronization() =>
            OnCollectionSynchronized();

        #endregion
    }
}
