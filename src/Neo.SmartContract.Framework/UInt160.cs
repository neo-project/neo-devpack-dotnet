using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;

namespace Neo
{
    public abstract class UInt160 : ByteString
    {
        public static extern UInt160 Zero { [OpCode(OpCode.PUSHDATA1, "140000000000000000000000000000000000000000")] get; }

        public extern bool IsZero
        {
            [OpCode(OpCode.PUSH0)]
            [OpCode(OpCode.NUMEQUAL)]
            get;
        }

        public extern bool IsValid
        {
            [OpCode(OpCode.DUP)]
            [OpCode(OpCode.ISTYPE, "0x28")] //ByteString
            [OpCode(OpCode.SWAP)]
            [OpCode(OpCode.SIZE)]
            [OpCode(OpCode.PUSHINT8, "14")] // 0x14 == 20 bytes expected array size
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
        [OpCode(OpCode.PUSHINT8, "14")] // 0x14 == 20 bytes expected array size
        [OpCode(OpCode.JMPEQ, "03")]
        [OpCode(OpCode.THROW)]
        public static extern explicit operator UInt160(byte[] value);

        [OpCode(OpCode.CONVERT, StackItemType.Buffer)]
        public static extern explicit operator byte[](UInt160 value);

        /// <summary>
        /// Converts the specified script hash to an address.
        /// </summary>
        /// <param name="scriptHash">The script hash to convert.</param>
        /// <param name="version">The address version.</param>
        /// <returns>The converted address.</returns>
        public string ToAddress(byte version)
        {
            ByteString data = (ByteString)new byte[] { version };
            data = data + (ByteString)this;
            return StdLib.Base58CheckEncode(data);
        }
    }
}
