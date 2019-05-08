using System;
using System.Collections;
using System.Collections.Generic;

namespace MinMaxSearch
{
    class NullDictionary<K, V> : IDictionary<K, V>
    {
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => new List<KeyValuePair<K, V>>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<K, V> item)
        {
            // Do nothing
        }

        public void Clear()
        {
            // Do nothing
        }

        public bool Contains(KeyValuePair<K, V> item) => false;

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            // Do nothing
        }

        public bool Remove(KeyValuePair<K, V> item) => true;

        public int Count => 0;
        public bool IsReadOnly => true;
        public void Add(K key, V value)
        {
            // Do nothing
        }

        public bool ContainsKey(K key) => false;

        public bool Remove(K key) => true;

        public bool TryGetValue(K key, out V value)
        {
            value = default(V);
            return false;
        }

        public V this[K key]
        {
            get => default(V);
            set
            {
                // Do nothing 
            }
        }

        public ICollection<K> Keys => null;
        public ICollection<V> Values => null;
    }
}
