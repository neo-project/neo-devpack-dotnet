using Neo.VM;

namespace Neo.SmartContract.Framework
{
    public class Map<TKey, TValue>
    {
        [OpCode(OpCode.NEWMAP)]
        public extern Map();
    }

    public static class MapHelper
    {
        [OpCode(OpCode.SETITEM)]
        public extern static void Put<TKey, TValue>(this Map<TKey, TValue> map, TKey key, TValue value);

        [OpCode(OpCode.PICKITEM)]
        public extern static TValue Get<TKey, TValue>(this Map<TKey, TValue> map, TKey key);

        [OpCode(OpCode.REMOVE)]
        public extern static void Remove<TKey, TValue>(this Map<TKey, TValue> map, TKey key);

        [OpCode(OpCode.HASKEY)]
        public extern static bool HasKey<TKey, TValue>(this Map<TKey, TValue> map, TKey key);

        [OpCode(OpCode.KEYS)]
        public extern static TKey[] Keys<TKey, TValue>(this Map<TKey, TValue> map);

        [OpCode(OpCode.VALUES)]
        public extern static TValue[] Values<TKey, TValue>(this Map<TKey, TValue> map);
    }
}
