// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Kenet.Collections.Reactive
{
    public abstract class AbstractCollectionItemsOptions<TDerived, TItem> : ICollectionItemsOptions<TItem>
        where TDerived : AbstractCollectionItemsOptions<TDerived, TItem>
    {
        public IListMutationTarget<TItem>? ItemsMutationTarget { get; protected set; }

        public TDerived SetItems(IListMutationTarget<TItem>? itemsMutationTarget)
        {
            ItemsMutationTarget = itemsMutationTarget;
            return (TDerived)this;
        }

        public TDerived SetItems(IList<TItem> items)
        {
            ItemsMutationTarget = new ListMutationTarget<TItem>(items);
            return (TDerived)this;
        }

        /// <summary>
        /// Sets <see cref="ItemsMutationTarget"/> by creating a 
        /// <see cref="ListMutationTarget{ItemType}"/>
        /// with <paramref name="items"/> and <paramref name="itemsMutator"/>.
        /// <see cref="SynchronizableCollectionBase{ItemType, NewItemType}"/>.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="itemsMutator"></param>
        public TDerived SetItems(IList<TItem> items, IListMutator<TItem> itemsMutator)
        {
            if (items is null) {
                throw new ArgumentNullException(nameof(items));
            }

            if (itemsMutator is null) {
                throw new ArgumentNullException(nameof(itemsMutator));
            }

            ItemsMutationTarget = new ListMutationTarget<TItem>(items, itemsMutator);
            return (TDerived)this;
        }

        /// <summary>
        /// Sets <see cref="ItemsMutationTarget"/> by creating a 
        /// <see cref="ListMutationTarget{ItemType}"/>
        /// with new <see cref="List{T}"/> and <paramref name="itemsMutator"/>.
        /// <see cref="SynchronizableCollectionBase{ItemType, NewItemType}"/>.
        /// </summary>
        /// <param name="itemsMutator"></param>
        public TDerived SetItems(IListMutator<TItem> itemsMutator) =>
            SetItems(new List<TItem>(), itemsMutator);

        #region ISynchronizableCollectionItemsOptions<TItem>

        void ICollectionItemsOptions<TItem>.SetItems(IListMutationTarget<TItem>? itemsMutationTarget) =>
            SetItems(itemsMutationTarget);

        #endregion
    }
}
