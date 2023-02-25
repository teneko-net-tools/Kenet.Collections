// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Kenet.Collections.Reactive
{
    /// <summary>
    /// <para>
    /// A mutable list. It handles every action of <see cref="NotifyCollectionChangedAction"/>
    /// for one item except for <see cref="NotifyCollectionChangedAction.Reset"/> and
    /// <see cref="NotifyCollectionChangedAction.Move"/>.
    /// </para>
    /// <para>The naming in this class originates from <see cref="NotifyCollectionChangedAction"/>.</para>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IMutableList<TItem>
    {
        event EventHandler<Action<IListMutationTarget<TItem>>> CollectRedirectionTargets;

        IList<TItem> Items { get; }

        /// <summary>
        /// Indicates whether <see cref="IListMutationTarget{TItem}.ReplaceItem(int, Func{TItem}))"/> is functional and ready to be called.
        /// If it is <see langword="false"/> then it is intended that <see cref="IListMutationTarget{TItem}.ReplaceItem(int, Func{TItem})"/>
        /// is not called.
        /// </summary>
        bool CanReplaceItem { get; }

        ///// <summary>
        ///// Registers a collection mutator. The mutators are called whenever <see cref="Mutate(Action{IMutableCollection{TItem}}, bool)"/>
        ///// is called. The collected instances of type <see cref="IMutableCollection{TItem}"/> will be used one after other used.
        ///// </summary>
        ///// <param name="collectionMutator"></param>
        //void RegisterCollectionMutatorForRedirection(IMutableCollection<TItem> collectionMutator);

        void Mutate(Action<IListMutationTarget<TItem>> mutate, bool disableRedirection);
    }
}
