// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Kenet.Collections.Reactive.SynchronizationMethods;

namespace Kenet.Collections.Reactive
{
    internal interface ICollectionSynchronizationContext<TItem>
    {
        void BeginCollectionSynchronization();
        void ProcessModification(ICollectionModification<TItem, TItem> superItemModification);
        void EndCollectionSynchronization();
    }
}
