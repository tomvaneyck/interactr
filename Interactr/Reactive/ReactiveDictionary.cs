using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Interactr.Reactive
{
    /// <summary>
    /// An implementation of IDictionary with observables.
    /// </summary>
    /// <typeparam name="TKey">The type of keys used in the dictionary</typeparam>
    /// <typeparam name="TValue">The type of values stored in the dictionary</typeparam>
    public class ReactiveDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _values = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Observable that emits a key-value pair when a value is set using the matching key.
        /// This observable always emits whether or not the key was previously present in the dictionary.
        /// The pair is emitted after it is added.
        /// </summary>
        public IObservable<KeyValuePair<TKey, TValue>> OnValueChanged => _onValueChanged;
        private readonly Subject<KeyValuePair<TKey, TValue>> _onValueChanged = new Subject<KeyValuePair<TKey, TValue>>();

        /// <summary>
        /// Observable that emits a key-value pair when the entry is removed from the dictionary.
        /// This observable only emits when the entry under this key was removed, not when the value changes.
        /// The pair is emitted after it is removed.
        /// </summary>
        public IObservable<KeyValuePair<TKey, TValue>> OnValueRemoved => _onValueRemoved;
        private readonly Subject<KeyValuePair<TKey, TValue>> _onValueRemoved = new Subject<KeyValuePair<TKey, TValue>>();

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

        public void Clear()
        {
            Dictionary<TKey, TValue> oldDict = _values;
            _values = new Dictionary<TKey, TValue>();

            foreach (KeyValuePair<TKey, TValue> valuePair in oldDict)
            {
                _onValueRemoved.OnNext(valuePair);
            }
            oldDict.Clear();
        }
        
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (((ICollection<KeyValuePair<TKey, TValue>>) _values).Remove(item))
            {
                _onValueRemoved.OnNext(item);
                return true;
            }

            return false;
        }
        
        public void Add(TKey key, TValue value)
        {
            _values.Add(key, value);
            _onValueChanged.OnNext(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool Remove(TKey key)
        {
            if(_values.TryGetValue(key, out TValue val))
            {
                _values.Remove(key);
                _onValueRemoved.OnNext(new KeyValuePair<TKey, TValue>(key, val));
                return true;
            }

            return false;
        }

        public TValue this[TKey key]
        {
            get => _values[key];
            set
            {
                _values[key] = value;
                _onValueChanged.OnNext(new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        #region Default implementation
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)_values).Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)_values).CopyTo(array, arrayIndex);

        public int Count => _values.Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_values).IsReadOnly;

        public bool ContainsKey(TKey key) => _values.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => _values.TryGetValue(key, out value);

        public ICollection<TKey> Keys => _values.Keys;

        public ICollection<TValue> Values => _values.Values; 
        #endregion
    }
}
