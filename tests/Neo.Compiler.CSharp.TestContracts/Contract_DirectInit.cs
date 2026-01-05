// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_DirectInit.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{

    public class Contract_DirectInit : SmartContract.Framework.SmartContract
    {

        /// <summary>
        /// A static field of type ECPoint initialized directly via <see cref="ECPoint.Parse(string)"/>.
        /// </summary>
        private static readonly ECPoint eCPoint = ECPoint.Parse("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9");

        /// <summary>
        /// A static field of type UInt160 initialized directly via <see cref="UInt160.Parse(string)"/>.
        /// </summary>
        private static readonly UInt160 uInt160 = UInt160.Parse("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq");

        /// <summary>
        /// A static field of type UInt256 initialized directly via <see cref="UInt256.Parse(string)"/>.
        /// </summary>
        private static readonly UInt256 validUInt256 = UInt256.Parse("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925");

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
