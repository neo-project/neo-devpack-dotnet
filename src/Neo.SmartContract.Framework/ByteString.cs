namespace Neo.SmartContract.Framework
{
    public abstract class ByteString
    {
        public extern byte this[int index]
        {
            [OpCode(OpCode.PICKITEM)]
            get;
        }

        public extern int Length
        {
            [OpCode(OpCode.SIZE)]
            get;
        }

        [Script]
        public static extern implicit operator string(ByteString str);

        [Script]
        public static extern implicit operator ByteString(string str);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](ByteString str);

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        public static extern explicit operator ByteString(byte[] buffer);
    }
}
