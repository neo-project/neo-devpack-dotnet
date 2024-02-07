using System.Numerics;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework
{
    public abstract class Fee
    {
        /// <summary>
        ///  GAS stands for NeoGas, 1 GAS = 100_000_000 Satoshis
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [OpCode(OpCode.PUSHINT32, "00E1F505")]
        [OpCode(OpCode.MUL)]
        public extern static BigInteger GAS(ulong amount);

        /// <summary>
        /// Satoshi stands for smallest unit of Neo GAS, 1 Satoshi = 0.00000001 GAS
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static BigInteger Satoshi(ulong amount) => amount;

        /// <summary>
        /// kSatoshi stands for kilo-Satoshi, 1 kSatoshi = 1000 Satoshi or 0.00001 GAS
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [OpCode(OpCode.PUSHINT16, "E803")]
        [OpCode(OpCode.MUL)]
        public extern static BigInteger kSatoshi(ulong amount);

        /// <summary>
        /// mSatoshi stands for mega-Satoshi, 1 mSatoshi = 1_000_000 Satoshi or 0.01 GAS
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        [OpCode(OpCode.PUSHINT32, "40420F00")]
        [OpCode(OpCode.MUL)]
        public extern static BigInteger mSatoshi(ulong amount);
    }
}
