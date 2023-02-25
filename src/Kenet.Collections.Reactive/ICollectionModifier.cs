// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Kenet.Collections.Reactive
{
    public interface IListMutator<TItem>
    {
        /// <summary>
        /// Indicates whether <see cref="ReplaceItem(IList{TItem}, int, Func{TItem})"/> is functional and ready to be called.
        /// If it is <see langword="false"/> then it is intended that <see cref="ReplaceItem(IList{TItem}, int, Func{TItem})"/>
        /// is not called.
        /// </summary>
        bool CanReplaceItem { get; }

        /// <summary>
        /// Inserts the item at the given index.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="insertAt"></param>
        /// <param name="item"></param>
        void InsertItem(IList<TItem> list, int insertAt, TItem item);
        /// <summary>
        /// Removes item at given index.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="removeAt"></param>
        void RemoveItem(IList<TItem> list, int removeAt);
        /// <summary>
        /// Moves the items between <paramref name="fromIndex"/> and <paramref name="fromIndex"/> + <paramref name="count"/> to <paramref name="toIndex"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        /// <param name="count"></param>
        void MoveItems(IList<TItem> list, int fromIndex, int toIndex, int count);
        /// <summary>
        /// Replaces the item at index <paramref name="replaceAt"/> by the item you get from <paramref name="getNewItem"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="replaceAt"></param>
        /// <param name="getNewItem"></param>
        void ReplaceItem(IList<TItem> list, int replaceAt, Func<TItem> getNewItem);
        /// <summary>
        /// Clears the list.
        /// </summary>
        /// <param name="list"></param>
        void Reset(IList<TItem> list);
    }
}
