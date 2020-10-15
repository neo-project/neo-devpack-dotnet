using Neo.IO;
using Neo.IO.Caching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    public class TestDataCache<TKey, TValue> : DataCache<TKey, TValue>
        where TKey : IEquatable<TKey>, ISerializable
        where TValue : class, ICloneable<TValue>, ISerializable, new()
    {
        private readonly Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();

        public TestDataCache() { }

        public TestDataCache(TKey key, TValue value)
        {
            dic.Add(key, value);
        }

        protected override bool ContainsInternal(TKey key)
        {
            return dic.ContainsKey(key);
        }

        protected override void DeleteInternal(TKey key)
        {
            dic.Remove(key);
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            dic.Add(key, value);
        }

        protected override IEnumerable<(TKey Key, TValue Value)> SeekInternal(byte[] keyOrPrefix, SeekDirection direction)
        {
            return dic.Select(u => (u.Key, u.Value));
        }

        protected override TValue GetInternal(TKey key)
        {
            if (dic[key] == null) throw new NotImplementedException();
            return dic[key];
        }

        protected override TValue TryGetInternal(TKey key)
        {
            return dic.TryGetValue(key, out TValue value) ? value : null;
        }

        protected override void UpdateInternal(TKey key, TValue value)
        {
            dic[key] = value;
        }
    }
}
