// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_Types_ECPoint.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.Compiler.CSharp.TestContracts
{
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("Compiler Test Contract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.Compiler.CSharp.TestContracts")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class Contract_Types_ECPoint : SmartContract.Framework.SmartContract
    {
        [PublicKey("024700db2e90d9f02c4f9fc862abaca92725f95b4fddcc8d7ffa538693ecf463a9")]
        private static readonly ECPoint publicKey2Ecpoint = default!;

        public static bool isValid(ECPoint point) { return point.IsValid; }

        public static string ecpoint2String()
        {
            return publicKey2Ecpoint;
        }

        public static ECPoint ecpointReturn()
        {
            return publicKey2Ecpoint;
        }

        public static object ecpoint2ByteArray()
        {
            return (byte[])publicKey2Ecpoint;
        }
    }
}


