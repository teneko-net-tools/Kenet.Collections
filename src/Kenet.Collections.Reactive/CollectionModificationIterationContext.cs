// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Kenet.Collections.Reactive
{
    public class CollectionModificationIterationContext
    {
        /// <summary>
        /// The item index in the scope of the modification.
        /// </summary>
        public int ModificationItemIndex {
            get => _modificationItemIndex;

            internal set {
                _modificationItemIndex = value;
                _isCollectionIndexDirty = true;
            }
        }

        /// <summary>
        /// The first index of the collection that is affected by the modification. If you add the modification item index and the collection start
        /// index you get the actual item index of the collection. The collection the talk is about is the collection which modification you apply on.
        /// </summary>
        public int CollectionStartIndex { get; }

        public int CollectionItemIndex {
            get {
                if (_isCollectionIndexDirty) {
                    _collectionItemIndex = CollectionStartIndex + ModificationItemIndex;
                    _isCollectionIndexDirty = false;
                }

                return _collectionItemIndex;
            }
        }

        private bool _isCollectionIndexDirty;
        private int _collectionItemIndex;
        private int _modificationItemIndex;

        public CollectionModificationIterationContext(int collectionStartIndex) =>
            CollectionStartIndex = collectionStartIndex;
    }
}
