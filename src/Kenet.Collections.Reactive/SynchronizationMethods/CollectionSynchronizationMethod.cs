﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Kenet.Collections.Reactive.SynchronizationMethods
{
    public static class CollectionSynchronizationMethod
    {
        /// <summary>
        /// Creates a method for creating modifications that can transform one
        /// collection into another collection that is in sequential order
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="equalityComparer"></param>
        /// <returns>A collection synchronization method.</returns>
        public static ICollectionSynchronizationMethod<TItem, TItem> Sequential<TItem>(IEqualityComparer<TItem> equalityComparer)
            where TItem : notnull =>
            new CollectionSynchronizationMethod<TItem, TItem, TItem>.Sequential(
                leftItem => leftItem,
                rightItem => rightItem,
                equalityComparer);

        /// <summary>
        /// Creates a method for creating modifications that can transform one
        /// collection into another collection that is in sequential order
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <returns>A collection synchronization method.</returns>
        public static ICollectionSynchronizationMethod<TItem, TItem> Sequential<TItem>()
            where TItem : notnull =>
            Sequential(EqualityComparer<TItem>.Default);

        /// <summary>
        /// Creates a method for creating modifications that can transform one
        /// collection into another collection that is in ascended order
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <param name="comparer"></param>
        /// <returns>A collection synchronization method.</returns>
        public static ICollectionSynchronizationMethod<TItem, TItem> Sorted<TItem>(IComparer<TItem> comparer)
            where TItem : notnull =>
            new CollectionSynchronizationMethod<TItem, TItem, TItem>.Sorted(
                leftItem => leftItem,
                rightItem => rightItem,
                comparer);

        /// <summary>
        /// Creates a method for creating modifications that can transform one
        /// collection into another collection that is in ascended order
        /// </summary>
        /// <typeparam name="TItem">The item type.</typeparam>
        /// <returns>A collection synchronization method.</returns>
        public static ICollectionSynchronizationMethod<TItem, TItem> Sorted<TItem>()
            where TItem : notnull =>
            Sorted(Comparer<TItem>.Default);
    }
}
