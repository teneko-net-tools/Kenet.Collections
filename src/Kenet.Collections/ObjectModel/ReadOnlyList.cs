// Copyright (c) 2022 Teneko .NET Tools authors and contributors
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kenet.Collections.ObjectModel
{
    public class ReadOnlyList<TItem> : IReadOnlyList<TItem>, IList<TItem>, IList
    {
        public virtual bool IsSynchronized => false;

        public int Count =>
            readOnlyList.Count;

        private readonly IReadOnlyList<TItem> readOnlyList;

        private object? syncRoot;

        public ReadOnlyList(IReadOnlyList<TItem> readOnlyList) =>
            this.readOnlyList = readOnlyList;

        public TItem this[int index] => 
            readOnlyList[index];

        public IEnumerator<TItem> GetEnumerator() => 
            readOnlyList.GetEnumerator();

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)readOnlyList).GetEnumerator();

        #endregion

        #region IList<ItemType>

        TItem IList<TItem>.this[int index] {
            get => readOnlyList[index];
            set => throw new NotSupportedException();
        }

        int IList<TItem>.IndexOf(TItem item)
        {
            var index = 0;

            foreach (var listItem in readOnlyList) {
                if (Equals(listItem, item)) {
                    return index;
                }

                index++;
            }

            return -1;
        }

        void IList<TItem>.Insert(int index, TItem item) => throw new NotSupportedException();
        void IList<TItem>.RemoveAt(int index) => throw new NotSupportedException();

        bool ICollection<TItem>.IsReadOnly => true;

        void ICollection<TItem>.Add(TItem item) => throw new NotSupportedException();
        void ICollection<TItem>.Clear() => throw new NotSupportedException();
        bool ICollection<TItem>.Contains(TItem item) => readOnlyList.Contains(item);

        void ICollection<TItem>.CopyTo(TItem[] array, int arrayIndex) =>
            ((ICollection)this).CopyTo(array, arrayIndex);

        bool ICollection<TItem>.Remove(TItem item) => throw new NotSupportedException();

        #endregion

        #region IList

        bool IList.IsFixedSize => true;
        bool IList.IsReadOnly => true;

        object? IList.this[int index] {
            get => readOnlyList[index];
            set => throw new NotSupportedException();
        }

        int IList.Add(object? value) => throw new NotSupportedException();
        void IList.Clear() => throw new NotSupportedException();
        bool IList.Contains(object? value) => readOnlyList.Contains((TItem?)value);
        int IList.IndexOf(object? value) => throw new NotSupportedException();
        void IList.Insert(int index, object? value) => throw new NotSupportedException();
        void IList.Remove(object? value) => throw new NotSupportedException();
        void IList.RemoveAt(int index) => throw new NotSupportedException();

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            foreach (var item in readOnlyList) {
                array.SetValue(item, arrayIndex++);
            }
        }

        object ICollection.SyncRoot {
            get {
                if (syncRoot == null) {
                    System.Threading.Interlocked.CompareExchange<object?>(ref syncRoot, new object(), null);
                }

                return syncRoot;
            }
        }

        #endregion
    }
}
