namespace Neo.SmartContract.Framework
{
    public class List<T>
    {
        [OpCode(OpCode.NEWARRAY0)]
        public extern List();

        public extern int Count
        {
            [OpCode(OpCode.SIZE)]
            get;
        }

        public extern T this[int key]
        {
            [OpCode(OpCode.PICKITEM)]
            get;
            [OpCode(OpCode.SETITEM)]
            set;
        }

        [OpCode(OpCode.APPEND)]
        public extern void Add(T item);

        [OpCode(OpCode.REMOVE)]
        public extern void RemoveAt(int index);

        [OpCode(OpCode.CLEARITEMS)]
        public extern void Clear();

        [OpCode(OpCode.VALUES)]
        public extern List<T> Clone();

        [Script]
        public static extern implicit operator List<T>(T[] array);

        [Script]
        public static extern implicit operator T[](List<T> array);
    }
}
