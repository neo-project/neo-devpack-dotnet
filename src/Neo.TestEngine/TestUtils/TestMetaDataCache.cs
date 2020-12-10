using Neo.IO;
using Neo.IO.Caching;

namespace Neo.TestingEngine
{
    public class TestMetaDataCache<T> : MetaDataCache<T> where T : class, ICloneable<T>, ISerializable, new()
    {
        private T metadata = null;
        public TestMetaDataCache()
            : base(null)
        {
        }

        protected override void AddInternal(T item)
        {
        }

        protected override T TryGetInternal()
        {
            return metadata;
        }

        protected override void UpdateInternal(T item)
        {
            metadata = item;
        }
    }
}
