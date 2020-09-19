using Neo.SmartContract.Framework;

namespace Neo
{
    public class UInt160
    {
        public static extern UInt160 Zero { [OpCode(OpCode.PUSHDATA1, "140000000000000000000000000000000000000000")] get; }

        public UInt160()
        {
        }

        public UInt160(byte[] _)
        {
        }

        [SmartContract.Framework.Script]
        public static extern implicit operator byte[](UInt160 value);
    }
}