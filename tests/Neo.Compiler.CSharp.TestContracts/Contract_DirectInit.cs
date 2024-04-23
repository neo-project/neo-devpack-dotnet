using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using System.Collections;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{

    public class Contract_DirectInit : SmartContract.Framework.SmartContract
    {

        /// <summary>
        /// A static field of type ECPoint initialized directly from a string. This is used to demonstrate initializing
        /// complex types like ECPoint at compile time to avoid runtime overhead.
        /// </summary>
        // [PublicKey("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9")]
        private static readonly ECPoint eCPoint = "024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9";

        /// <summary>
        /// A static field of type UInt160 initialized directly from a string. This allows for compile-time
        /// initialization of blockchain-specific types like addresses, represented here as Hash160.
        /// </summary>
        // [Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]
        private static readonly UInt160 uInt160 = "NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq";

        /// <summary>
        /// A static field of type UInt160 initialized directly from a hex string. This allows for compile-time
        /// initialization of blockchain-specific types like txid/blockhash, represented here as Hash256.
        /// </summary>
        // [ByteArray("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925")]
        private static readonly UInt256 validUInt256 = "edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925";

        /// <summary>
        /// A static string field initialized directly.
        /// This demonstrates initializing contract fields that cannot be directly assigned with their value at compile time.
        /// </summary>
        // [String("hello world")]
        public static readonly string a4 = "hello world";

        public static UInt160 testGetUInt160()
        {
            return uInt160;
        }

        public static ECPoint testGetECPoint()
        {
            return eCPoint;
        }

        public static UInt256 testGetUInt256()
        {
            return validUInt256;
        }

        public static string testGetString()
        {
            return a4;
        }
    }
}
