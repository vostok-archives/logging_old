using System.Collections.Generic;

namespace Vostok.Logging.Abstractions.Flow
{
    internal class ContextProperty<TKey, TValue> //TODO(mylov): Rename this class.
    {
        public ContextProperty(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; }
        public TValue Value { get; }

        public KeyValuePair<TKey, TValue> ToKeyValuePair()
        {
            return new KeyValuePair<TKey, TValue>(Key, Value);
        }
    }
}
