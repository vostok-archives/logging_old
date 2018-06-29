using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Vostok.Logging.Abstractions.Flow
{
    internal class ContextPropertiesSnapshot<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TValue : class
    //TODO(mylov): Rename this class. 
    //TODO(mylov): Write tests for it.
    {
        private const int defaultCapacity = 4;

        public static readonly ContextPropertiesSnapshot<TKey, TValue> Empty = new ContextPropertiesSnapshot<TKey, TValue>(0);

        private readonly ContextProperty<TKey, TValue>[] properties;

        private readonly IEqualityComparer<TKey> keyComparer;

        public static ContextPropertiesSnapshot<TKey, TValue> FromCollection(IReadOnlyDictionary<TKey, TValue> collection, IEqualityComparer<TKey> keyComparer = null)
        {
            if (collection == null)
                return null;

            if (collection is ContextPropertiesSnapshot<TKey, TValue> snapshot)
                return snapshot;

            snapshot = new ContextPropertiesSnapshot<TKey, TValue>(keyComparer);

            return collection.Aggregate(snapshot, (current, property) => current.Set(property.Key, property.Value));
        }

        public ContextPropertiesSnapshot(IEqualityComparer<TKey> keyComparer = null) : this(defaultCapacity, keyComparer) { }

        public ContextPropertiesSnapshot(int capacity, IEqualityComparer<TKey> keyComparer = null) : this(new ContextProperty<TKey, TValue>[capacity], 0, keyComparer) { }

        private ContextPropertiesSnapshot(ContextProperty<TKey, TValue>[] properties, int count, IEqualityComparer<TKey> keyComparer)
        {
            this.properties = properties;
            this.keyComparer = keyComparer;
            Count = count;
        }

        public int Count { get; }

        public IEnumerable<TKey> Keys => this.Select(pair => pair.Key);

        public IEnumerable<TValue> Values => this.Select(pair => pair.Value);

        public TValue this[TKey key] => Find(key, out var value, out var _) ? value : throw new KeyNotFoundException();

        public bool ContainsKey(TKey key)
        {
            return Find(key, out var _, out var _);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Find(key, out value, out var _);
        }

        public ContextPropertiesSnapshot<TKey, TValue> Set(TKey key, TValue value)
        {
            ContextProperty<TKey, TValue>[] newProperties;

            var newProperty = new ContextProperty<TKey, TValue>(key, value);

            if (Find(key, out var oldValue, out var oldIndex))
            {
                if (Equals(value, oldValue))
                    return this;

                newProperties = ReallocateArray(properties.Length);
                newProperties[oldIndex] = newProperty;
                return new ContextPropertiesSnapshot<TKey, TValue>(newProperties, Count, keyComparer);
            }

            if (properties.Length == Count)
            {
                newProperties = ReallocateArray(Math.Max(defaultCapacity, properties.Length * 2));
                newProperties[Count] = newProperty;
                return new ContextPropertiesSnapshot<TKey, TValue>(newProperties, Count + 1, keyComparer);
            }

            if (Interlocked.CompareExchange(ref properties[Count], newProperty, null) != null)
            {
                newProperties = ReallocateArray(properties.Length);
                newProperties[Count] = newProperty;
                return new ContextPropertiesSnapshot<TKey, TValue>(newProperties, Count + 1, keyComparer);
            }

            return new ContextPropertiesSnapshot<TKey, TValue>(properties, Count + 1, keyComparer);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return properties[i].ToKeyValuePair();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool Find(TKey key, out TValue value, out int index)
        {
            for (var i = 0; i < Count; i++)
            {
                var property = properties[i];
                if (keyComparer?.Equals(property.Key, key) ?? property.Key.Equals(key))
                {
                    index = i;
                    value = property.Value;
                    return true;
                }
            }

            index = -1;
            value = null;
            return false;
        }

        private ContextProperty<TKey, TValue>[] ReallocateArray(int capacity)
        {
            var reallocated = new ContextProperty<TKey, TValue>[capacity];

            Array.Copy(properties, 0, reallocated, 0, Count);

            return reallocated;
        }
    }
}