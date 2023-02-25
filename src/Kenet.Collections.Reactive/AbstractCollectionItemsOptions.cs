// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Kenet.Collections.Reactive
{
    public abstract class AbstractCollectionItemsOptions<TDerived, TItem> : ICollectionItemsOptions<TItem>
        where TDerived : AbstractCollectionItemsOptions<TDerived, TItem>
    {
        public IMutableList<TItem>? Items { get; protected set; }

        public TDerived SetItems(IMutableList<TItem>? items)
        {
            Items = items;
            return (TDerived)this;
        }

        public TDerived SetItems(IList<TItem> items)
        {
            Items = new MutableList<TItem>(items);
            return (TDerived)this;
        }

        /// <summary>
        /// Sets <see cref="Items"/> by creating a 
        /// <see cref="MutableList{ItemType}"/>
        /// with <paramref name="items"/> and <paramref name="mutator"/>.
        /// <see cref="SynchronizableCollectionBase{ItemType, NewItemType}"/>.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="mutator"></param>
        public TDerived SetItems(IList<TItem> items, IStatelessCollectionMutator<TItem> mutator)
        {
            if (items is null) {
                throw new ArgumentNullException(nameof(items));
            }

            if (mutator is null) {
                throw new ArgumentNullException(nameof(mutator));
            }

            Items = new MutableList<TItem>(items, mutator);
            return (TDerived)this;
        }

        /// <summary>
        /// Sets <see cref="Items"/> by creating a 
        /// <see cref="MutableList{ItemType}"/>
        /// with new <see cref="List{T}"/> and <paramref name="mutator"/>.
        /// <see cref="SynchronizableCollectionBase{ItemType, NewItemType}"/>.
        /// </summary>
        /// <param name="mutator"></param>
        public TDerived SetItems(IStatelessCollectionMutator<TItem> mutator) =>
            SetItems(new List<TItem>(), mutator);

        #region ISynchronizableCollectionItemsOptions<TItem>

        void ICollectionItemsOptions<TItem>.SetItems(IMutableList<TItem>? items) =>
            SetItems(items);

        #endregion
    }
}
