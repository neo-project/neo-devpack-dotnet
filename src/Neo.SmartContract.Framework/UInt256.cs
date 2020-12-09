using Neo.SmartContract.Framework;

namespace Neo
{
    public class UInt256
    {
        public static extern UInt256 Zero { [OpCode(OpCode.PUSHDATA1, "200000000000000000000000000000000000000000000000000000000000000000")] get; }

        public extern bool IsZero
        {
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.NUMEQUAL)]
            get;
        }

        public extern int Size
        {
            [OpCode(OpCode.SIZE)]
            get;
        }

        public extern bool IsValid
        {
            [OpCode(OpCode.DUP)]
            [OpCode(OpCode.ISNULL)]
            [OpCode(OpCode.JMPIF, "0x08")]
            [OpCode(OpCode.SIZE)]
            [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
            [OpCode(OpCode.NUMEQUAL)]
            [OpCode(OpCode.JMP, "0x04")]
            [OpCode(OpCode.DROP)]
            [OpCode(OpCode.PUSH0)]
            get;
        }

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public static extern explicit operator UInt256(byte[] value);

        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "20")] // 0x20 == 32 bytes expected array size
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public static extern explicit operator UInt256(ByteString value);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](UInt256 value);

        [Script]
        public static extern implicit operator ByteString(UInt256 value);
    }
}
