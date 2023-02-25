// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    internal static class IndexBasedEnumeratorExtensions
    {
        public static IList<TItem> AsList<TItem>(this IList<TItem> list) =>
            list;

        /// <summary>
        /// Wraps <paramref name="list"/> to mark it. Indicates that this list is getting mutated while collection modifications are produced.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IndexBasedEnumerator<TItem> ToIndexBasedEnumerator<TItem>(this IList<TItem> list) =>
            new(new ReadOnlyCollection<TItem>(list));

        public static IReadOnlyList<ItemType> AsReadOnlyList<ItemType>(this IReadOnlyList<ItemType> list) =>
            list;

        /// <inheritdoc cref="ToIndexBasedEnumerator{TItem}(IList{TItem})"/>
        public static IndexBasedEnumerator<TItem> InfluencedByStateMachine<TItem>(this IReadOnlyList<TItem> list) =>
            new(list);
    }
}
