// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Kenet.Collections.Reactive
{
    public class SynchronizingCollection<TSuperItem, TSubItem> : SynchronizingCollectionBase<TSuperItem, TSubItem>
        where TSuperItem : notnull
        where TSubItem : notnull
    {
        private Func<TSuperItem, TSubItem> _subItemSelector;

        public SynchronizingCollection(Func<TSuperItem, TSubItem> subItemSelector, SynchronizingCollectionOptions<TSuperItem, TSubItem>? options)
            : base(options) =>
            OnInitialize(subItemSelector);

        public SynchronizingCollection(Func<TSuperItem, TSubItem> subItemSelector) :
            base() =>
            OnInitialize(subItemSelector);

        [MemberNotNull(nameof(_subItemSelector))]
        private void OnInitialize(Func<TSuperItem, TSubItem> subItemSelector) =>
            _subItemSelector = subItemSelector ?? throw new ArgumentNullException(nameof(subItemSelector));

        protected override TSubItem SelectSubItem(TSuperItem superItem) =>
            _subItemSelector(superItem);
    }
}
