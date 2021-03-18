using Neo.SmartContract.Framework.Native;
using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Framework
{
    public abstract class SmartContract
    {
        /// <summary>
        /// Faults if `condition` is false
        /// </summary>
        /// <param name="condition">Condition that MUST meet</param>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [OpCode(OpCode.ASSERT)]
        public static extern void Assert(bool condition);

        /// <summary>
        /// Abort the execution
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall)]
        [OpCode(OpCode.ABORT)]
        public static extern void Abort();

        public static ByteString Hash160(ByteString value)
        {
            return CryptoLib.ripemd160(CryptoLib.Sha256(value));
        }

        public static ByteString Hash256(ByteString value)
        {
            return CryptoLib.Sha256(CryptoLib.Sha256(value));
        }
    }
}
