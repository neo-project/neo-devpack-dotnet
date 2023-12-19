using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_StorageBacked : SmartContract
    {
        // Test non-static

        [Storage]
        public BigInteger WithoutConstructor { [Safe] get; protected set; }

        public void putWithoutConstructor(BigInteger value)
        {
            WithoutConstructor = value;
        }

        [Safe]
        public BigInteger getWithoutConstructor() => WithoutConstructor;

        // ---

        [Storage(0x01)]
        public static BigInteger WithKey { [Safe] get; protected set; }

        public static void putWithKey(BigInteger value)
        {
            WithKey = value;
        }

        [Safe]
        public static BigInteger getWithKey() => WithKey;

        // ---

        [Storage("testMe")]
        public static BigInteger WithString { [Safe] get; protected set; }

        public static void putWithString(BigInteger value)
        {
            WithString = value;
        }

        [Safe]
        public static BigInteger getWithString() => WithString;


        [Storage]
        public static BigInteger PrivateGetterPublicSetter { [Safe] private get; set; }

        [Safe]
        public static BigInteger getPrivateGetterPublicSetter() => PrivateGetterPublicSetter;


        [Storage]
        public BigInteger NonStaticPrivateGetterPublicSetter { [Safe] private get; set; }

        [Safe]
        public BigInteger getNonStaticPrivateGetterPublicSetter() => NonStaticPrivateGetterPublicSetter;
    }
}
