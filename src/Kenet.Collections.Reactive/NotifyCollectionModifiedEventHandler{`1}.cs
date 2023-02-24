﻿// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Teronis.Collections.Synchronization
{
    public delegate void NotifyNotifyCollectionModifiedEventHandler<TItem>(object sender, CollectionModifiedEventArgs<TItem> args);
}