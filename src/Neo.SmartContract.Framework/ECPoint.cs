using Neo.SmartContract.Framework;

namespace Neo.Cryptography.ECC
{
    public abstract class ECPoint : ByteString
    {
        public extern bool IsValid
        {
            [OpCode(OpCode.DUP)]
            [OpCode(OpCode.ISTYPE, "0x28")] //ByteString
            [OpCode(OpCode.SWAP)]
            [OpCode(OpCode.SIZE)]
            [OpCode(OpCode.PUSHINT8, "21")] // 0x21 == 33 bytes expected array size
            [OpCode(OpCode.NUMEQUAL)]
            [OpCode(OpCode.BOOLAND)]
            get;
        }

        [OpCode(OpCode.CONVERT, StackItemType.ByteString)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.ISNULL)]
        [OpCode(OpCode.JMPIF, "09")]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "21")] // 0x21 == 33 bytes expected array size
        [OpCode(OpCode.JMPEQ, "03")]
        [OpCode(OpCode.THROW)]
        public static extern explicit operator ECPoint(byte[] value);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](ECPoint value);
    }
}
