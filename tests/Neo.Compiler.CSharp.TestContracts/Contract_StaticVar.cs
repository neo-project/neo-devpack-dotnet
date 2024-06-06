using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    /// <summary>
    /// A smart contract designed for testing the definition and initialization of static fields within a contract.
    /// Since contract interfaces exposed to the blockchain can be static, any field to be used within these static interfaces
    /// must also be defined as static.
    /// This contract demonstrates both traditional direct assignment of static fields and the use of the InitialValue
    /// attribute for types that cannot be directly assigned.
    /// <remarks>public interfaces of <see cref="SmartContract"/> can be static and non-static.</remarks>
    /// </summary>
    public class Contract_StaticVar : SmartContract.Framework.SmartContract
    {
        // Direct initialization of a static integer.
        static int a1 = 1;
        // Static fields of type BigInteger initialized using BigInteger.Parse method.
        static readonly BigInteger a2 = BigInteger.Parse("120");
        static readonly BigInteger a3 = BigInteger.Parse("3");

        /// <summary>
        /// A static field of type ECPoint initialized with the InitialValue attribute. This is used to demonstrate initializing
        /// complex types like ECPoint at compile time to avoid runtime overhead.
        /// </summary>
        // [PublicKey("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9")]
        private static readonly ECPoint eCPoint = "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9";

        /// <summary>
        /// A static field of type UInt160 initialized with the InitialValue attribute. This allows for compile-time
        /// initialization of blockchain-specific types like addresses, represented here as Hash160.
        /// </summary>
        // [Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]
        private static readonly UInt160 uInt160 = "NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq";

        /// <summary>
        /// A static string field initialized with the InitialValue attribute. This demonstrates initializing contract fields that cannot be directly assigned with their value at compile time.
        /// </summary>
        // [String("hello world")]
        public static readonly string a4 = "hello world";

        /// <summary>
        /// Tests retrieval of the static field initialized with an initial value.
        /// </summary>
        /// <returns>The value of the static field <c>a4</c>.</returns>
        public static string testinitalvalue() => a4;

        public static int TestMain()
        {
            testadd();
            testmulti();
            return a1;
        }

        static void testadd()
        {
            a1 += 5;
        }

        static void testmulti()
        {
            a1 *= 7;
        }

        public static BigInteger testBigIntegerParse()
        {
            return a2 + a3;
        }

        public static BigInteger testBigIntegerParse2(string text)
        {
            return BigInteger.Parse(text);
        }

        public static UInt160 testGetUInt160()
        {
            return uInt160;
        }

        public static ECPoint testGetECPoint()
        {
            return eCPoint;
        }

        public static string testGetString()
        {
            return a4;
        }
    }
}
