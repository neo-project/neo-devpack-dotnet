using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_StorageBacked : SmartContract
    {
        [StorageBacked]
        public static BigInteger ValueA { [Safe] get; protected set; }

        public void putWithoutConstructor(BigInteger value)
        {
            ValueA = value;
        }

        [Safe]
        public BigInteger getWithoutConstructor() => ValueA;

        // ---

        [StorageBacked(0x01)]
        public static BigInteger ValueB { [Safe] get; protected set; }

        public static void putWithKey(BigInteger value)
        {
            ValueB = value;
        }

        [Safe]
        public static BigInteger getWithKey() => ValueB;

        // ---

        [StorageBacked("testMe")]
        public static BigInteger ValueC { [Safe] get; protected set; }

        public static void putWithString(BigInteger value)
        {
            ValueC = value;
        }

        [Safe]
        public static BigInteger getWithString() => ValueC;
    }
}
