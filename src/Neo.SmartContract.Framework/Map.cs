namespace Neo.SmartContract.Framework
{
    public class Map<TKey, TValue>
    {
        [OpCode(OpCode.NEWMAP)]
        public extern Map();

        public extern int Count
        {
            [OpCode(OpCode.SIZE)]
            get;
        }

        public extern TValue this[TKey key]
        {
            [OpCode(OpCode.PICKITEM)]
            get;
            [OpCode(OpCode.SETITEM)]
            set;
        }

        [OpCode(OpCode.CLEARITEMS)]
        public extern void Clear();

        public extern TKey[] Keys
        {
            [OpCode(OpCode.KEYS)]
            get;
        }

        public extern TValue[] Values
        {
            [OpCode(OpCode.VALUES)]
            get;
        }

        [OpCode(OpCode.HASKEY)]
        public extern bool HasKey(TKey key);

        [OpCode(OpCode.REMOVE)]
        public extern void Remove(TKey key);
    }
}
