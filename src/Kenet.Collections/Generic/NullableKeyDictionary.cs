// Copyright (c) Teroneko.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Teronis.Extensions;

namespace Kenet.Collections.Generic
{
    public class NullableKeyDictionary<TKey, TValue> : INullableKeyDictionary<TKey, TValue>, IReadOnlyNullableKeyDictionary<TKey, TValue>
        where TKey : notnull
    {
        public ICollection<YetNullable<TKey>> Keys {
            get {
                var readOnlyDictionary = (IReadOnlyDictionary<YetNullable<TKey>, TValue>)this;
                var keyList = new List<YetNullable<TKey>>(readOnlyDictionary.Keys);
                return keyList.AsReadOnly();
            }
        }

        public ICollection<TValue> Values {
            get {
                var readOnlyDictionary = (IReadOnlyDictionary<YetNullable<TKey>, TValue>)this;
                var valueList = new List<TValue>(readOnlyDictionary.Values);
                return valueList.AsReadOnly();
            }
        }

        public int Count {
            get {
                var count = dictionary.Count;

                if (nullableKeyValuePair.HasValue) {
                    count++;
                }

                return count;
            }
        }

        public bool IsReadOnly => dictionary.AsCollectionWithPairs().IsReadOnly;

        private readonly Dictionary<TKey, TValue> dictionary;
        private KeyValuePair<YetNullable<TKey>, TValue>? nullableKeyValuePair;

        public NullableKeyDictionary() =>
            dictionary = new Dictionary<TKey, TValue>();

        public NullableKeyDictionary(IDictionary<TKey, TValue> dictionary) =>
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);

        public NullableKeyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer) =>
            this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);

        public NullableKeyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) =>
            dictionary = collection.ToDictionary(x => x.Key, x => x.Value);

        public NullableKeyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer) =>
            dictionary = collection.ToDictionary(x => x.Key, x => x.Value, comparer);

        public NullableKeyDictionary(IEqualityComparer<TKey>? comparer) =>
            dictionary = new Dictionary<TKey, TValue>(comparer);

        public NullableKeyDictionary(int capacity) =>
            dictionary = new Dictionary<TKey, TValue>(capacity);

        public NullableKeyDictionary(int capacity, IEqualityComparer<TKey>? comparer) =>
            dictionary = new Dictionary<TKey, TValue>(capacity, comparer);

        public TValue this[YetNullable<TKey> key] {
            get {
                if (key.IsNull) {
                    if (!nullableKeyValuePair.HasValue) {
                        throw new KeyNotFoundException();
                    }

                    return nullableKeyValuePair.Value.Value;
                }

                return dictionary[key.Value];
            }

            set {
                if (IsReadOnly) {
                    throw NullableKeyDictionaryExceptionHelper.CreateNotSupportedException();
                }

                if (key.IsNull) {
                    nullableKeyValuePair = new KeyValuePair<YetNullable<TKey>, TValue>(key, value);
                } else {
                    dictionary[key.Value] = value;
                }
            }
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="INullableKeyDictionary{KeyType, ValueType}"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(YetNullable<TKey> key, TValue value)
        {
            if (IsReadOnly) {
                throw new NotSupportedException("");
            }

            if (key.IsNull) {
                if (nullableKeyValuePair.HasValue) {
                    throw new ArgumentException();
                }

                nullableKeyValuePair = new KeyValuePair<YetNullable<TKey>, TValue>(key, value!);
            } else {
                dictionary.Add(key.Value, value!);
            }
        }

        public void Add(TValue value) =>
            Add(YetNullable<TKey>.Null, value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
            dictionary.AsCollectionWithPairs().Contains(item);

        public bool ContainsKey(YetNullable<TKey> key)
        {
            if (key.IsNull) {
                return nullableKeyValuePair.HasValue;
            }

            return dictionary.ContainsKey(key.Value);
        }

        public bool TryGetValue(YetNullable<TKey> key, [MaybeNullWhen(false)] out TValue value)
        {
            if (key.IsNull) {
                if (nullableKeyValuePair.HasValue) {
                    value = nullableKeyValuePair.Value.Value;
                    return true;
                }
            } else {
                return dictionary.TryGetValue(key.Value, out value);
            }

            value = default;
            return false;
        }

        public CovariantTuple<bool, TValue> FindValue(YetNullable<TKey> key)
        {
            if (TryGetValue(key, out TValue value)) {
                return new CovariantTuple<bool, TValue>(true, value);
            }

            return new CovariantTuple<bool, TValue>(default, default!);
        }

        public bool Remove(YetNullable<TKey> key)
        {
            if (IsReadOnly) {
                throw NullableKeyDictionaryExceptionHelper.CreateNotSupportedException();
            }

            if (key.IsNull) {
                if (nullableKeyValuePair.HasValue) {
                    nullableKeyValuePair = null;
                    return true;
                }

                return false;
            }

            return dictionary.Remove(key.Value);
        }

        public bool Remove() =>
            Remove(YetNullable<TKey>.Null);

        public void Clear()
        {
            if (IsReadOnly) {
                throw NullableKeyDictionaryExceptionHelper.CreateNotSupportedException();
            }

            nullableKeyValuePair = null;
            dictionary.Clear();
        }

        public IEnumerator<KeyValuePair<YetNullable<TKey>, TValue>> GetEnumerator() =>
            new NullableKeyEnumuerator<TKey, TValue>(dictionary.AsCollectionWithPairs().GetEnumerator(), nullableKeyValuePair);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (nullableKeyValuePair.HasValue) {
                array[arrayIndex++] = new KeyValuePair<TKey, TValue>(default!, nullableKeyValuePair.Value.Value);
            }

            dictionary.AsCollectionWithPairs().CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<YetNullable<TKey>, TValue>[] array, int arrayIndex)
        {
            if (nullableKeyValuePair.HasValue) {
                array[arrayIndex++] = nullableKeyValuePair.Value;
            }

            var readOnlyDictionary = (IReadOnlyDictionary<YetNullable<TKey>, TValue>)this;

            foreach (var pair in readOnlyDictionary) {
                array[arrayIndex++] = pair;
            }
        }

        #region IDictionary<KeyType, ValueType>

        TValue IDictionary<TKey, TValue>.this[TKey key] {
            get => this[key];
            set => this[key] = value;
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => dictionary.Keys;
        ICollection<TValue> IDictionary<TKey, TValue>.Values => dictionary.Values;

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) =>
            Add(key, value);

        bool IDictionary<TKey, TValue>.Remove(TKey key) =>
            Remove(key);

        bool IDictionary<TKey, TValue>.ContainsKey(TKey key) =>
            ContainsKey(key);

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) =>
            TryGetValue(key, out value);

        #endregion

        #region IReadOnlyDictionary<KeyType, ValueType>

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => dictionary.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;

        bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (key is null) {
                if (nullableKeyValuePair.HasValue) {
                    value = nullableKeyValuePair.Value.Value;
                    return true;
                }

                value = default;
                return false;
            } else {
                return dictionary.TryGetValue(key, out value);
            }
        }

        #endregion

        #region ICollection<KeyValuePair<KeyType, ValueType>>

        int ICollection<KeyValuePair<TKey, TValue>>.Count => dictionary.Count;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            if (IsReadOnly) {
                throw NullableKeyDictionaryExceptionHelper.CreateNotSupportedException();
            }

            dictionary.AsCollectionWithPairs().Add(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (IsReadOnly) {
                throw NullableKeyDictionaryExceptionHelper.CreateNotSupportedException();
            }

            return dictionary.AsCollectionWithPairs().Remove(item);
        }

        #endregion

        #region IReadOnlyCollection<KeyValuePair<KeyType, ValueType>>

        int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => dictionary.Count;

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() =>
            this.AsReadOnlyDictionaryWithNullableKeys().GetEnumerator();

        #endregion

        #region IEnumerable<KeyValuePair<KeyType, ValueType>>

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            dictionary.AsCollectionWithPairs().GetEnumerator();

        #endregion

        #region IDictionary<YetNullable<KeyType>, ValueType>

        void ICollection<KeyValuePair<YetNullable<TKey>, TValue>>.Add(KeyValuePair<YetNullable<TKey>, TValue> item) =>
            Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<YetNullable<TKey>, TValue>>.Contains(KeyValuePair<YetNullable<TKey>, TValue> item)
        {
            var (nullableKey, value) = item;

            if (nullableKey.IsNull) {
                if (nullableKeyValuePair.HasValue) {
                    return EqualityComparer<TValue>.Default.Equals(item.Value, nullableKeyValuePair.Value.Value);
                }

                return false;
            }

            return dictionary.AsCollectionWithPairs().Contains(new KeyValuePair<TKey, TValue>(nullableKey.Value, value));
        }

        bool ICollection<KeyValuePair<YetNullable<TKey>, TValue>>.Remove(KeyValuePair<YetNullable<TKey>, TValue> item)
        {
            if (IsReadOnly) {
                throw NullableKeyDictionaryExceptionHelper.CreateNotSupportedException();
            }

            var (nullableKey, value) = item;

            if (TryGetValue(nullableKey, out var foundValue)) {
                if (EqualityComparer<TValue>.Default.Equals(value, foundValue)) {
                    Remove(nullableKey);
                    return true;
                }

                return false;
            }

            return false;
        }

        #endregion

        #region IReadOnlyDictionary<YetNullable<KeyType>, ValueType>

        IEnumerable<YetNullable<TKey>> IReadOnlyDictionary<YetNullable<TKey>, TValue>.Keys {
            get {
                var nullableKeyValuePair = this.nullableKeyValuePair;

                if (nullableKeyValuePair != null) {
                    yield return nullableKeyValuePair.Value.Key;
                }

                foreach (var key in dictionary.Keys) {
                    yield return new YetNullable<TKey>(key);
                }
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<YetNullable<TKey>, TValue>.Values {
            get {
                var nullableKeyValuePair = this.nullableKeyValuePair;

                if (nullableKeyValuePair != null) {
                    yield return nullableKeyValuePair.Value.Value;
                }

                foreach (var value in dictionary.Values) {
                    yield return value;
                }
            }
        }

        TValue IReadOnlyDictionary<YetNullable<TKey>, TValue>.this[YetNullable<TKey> key] {
            get {
                if (key.IsNull) {
                    if (!nullableKeyValuePair.HasValue) {
                        throw new KeyNotFoundException("The key does not exist in the collection.");
                    }

                    return nullableKeyValuePair.Value.Value;
                }

                return dictionary[key.Value];
            }
        }

        #endregion

        #region IReadOnlyCollection<KeyValuePair<IYetNullable<KeyType>,ValueType>>

        IEnumerator<KeyValuePair<IYetNullable<TKey>, TValue>> IEnumerable<KeyValuePair<IYetNullable<TKey>, TValue>>.GetEnumerator() =>
            new KeyValuePairEnumeratorWithPairHavingCovariantNullableKey<TKey, TValue>(GetEnumerator());

        #endregion

        #region IReadOnlyDictionary<KeyType, ValueType>

        bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) =>
            ContainsKey(key);

        TValue IReadOnlyDictionary<TKey, TValue>.this[TKey key] =>
            this[key];

        #endregion

        #region

        bool ICovariantReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) =>
            ContainsKey(key);

        TValue ICovariantReadOnlyDictionary<TKey, TValue>.this[TKey key] =>
            this[key];

        #endregion

        #region ICovariantReadOnlyNullableKeyDictionary<KeyType, ValueType>

        IEnumerable<TKey> ICovariantReadOnlyNullableKeyDictionary<TKey, TValue>.Keys => dictionary.Keys;
        IEnumerable<TValue> ICovariantReadOnlyNullableKeyDictionary<TKey, TValue>.Values => dictionary.Values;

        ICovariantTuple<bool, TValue> ICovariantReadOnlyNullableKeyDictionary<TKey, TValue>.TryGetValue(YetNullable<TKey> key) =>
            FindValue(key);

        #region ICovariantReadOnlyDictionary<KeyType, ValueType>

        IEnumerable<TKey> ICovariantReadOnlyDictionary<TKey, TValue>.Keys => dictionary.Keys;
        IEnumerable<TValue> ICovariantReadOnlyDictionary<TKey, TValue>.Values => dictionary.Values;

        ICovariantTuple<bool, TValue> ICovariantReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key) =>
            FindValue(key);

        #endregion

        #region IEnumerable<ICovariantKeyValuePair<KeyType, ValueType>>

        IEnumerator<ICovariantKeyValuePair<TKey, TValue>> IEnumerable<ICovariantKeyValuePair<TKey, TValue>>.GetEnumerator() =>
            this.AsReadOnlyDictionary().ToCovariantKeyValuePairCollection().GetEnumerator();

        #endregion

        #region ICovariantReadOnlyDictionary<YetNullable<KeyType>, ValueType>

        IEnumerable<YetNullable<TKey>> ICovariantReadOnlyDictionary<YetNullable<TKey>, TValue>.Keys => Keys;
        IEnumerable<TValue> ICovariantReadOnlyDictionary<YetNullable<TKey>, TValue>.Values => Values;

        ICovariantTuple<bool, TValue> ICovariantReadOnlyDictionary<YetNullable<TKey>, TValue>.TryGetValue(YetNullable<TKey> key) =>
            FindValue(key);

        #endregion

        #region IEnumerable<ICovariantKeyValuePair<YetNullable<KeyType>, ValueType>>

        IEnumerator<ICovariantKeyValuePair<YetNullable<TKey>, TValue>> IEnumerable<ICovariantKeyValuePair<YetNullable<TKey>, TValue>>.GetEnumerator() =>
            this.AsReadOnlyDictionaryWithNullableKeys().ToCovariantKeyValuePairCollection().GetEnumerator();

        #endregion

        #endregion

        internal static class NullableKeyDictionaryExceptionHelper
        {
            public static NotSupportedException CreateNotSupportedException() =>
                new NotSupportedException($"The {nameof(IReadOnlyNullableKeyDictionary<TKey, TValue>)} is read-only.");
        }
    }
}
