using Neo.SmartContract.Framework;

namespace Neo.Cryptography.ECC
{
    public class ECPoint
    {
        [OpCode(OpCode.CONVERT, Helper.StackItemType_ByteString)]
        [OpCode(OpCode.DUP)]
        [OpCode(OpCode.SIZE)]
        [OpCode(OpCode.PUSHINT8, "21")] // 0x21 == 33 bytes expected array size
        [OpCode(OpCode.NUMEQUAL)]
        [OpCode(OpCode.ASSERT)]
        public static extern explicit operator ECPoint(byte[] value);


        [Script]
        public static extern implicit operator byte[](ECPoint value);
    }
}
