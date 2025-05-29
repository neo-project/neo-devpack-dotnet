// Copyright (C) 2015-2025 The Neo Project.
//
// Contract_ByteArrayAssignment.cs file belongs to the neo project and is free
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
    public class Contract_ByteArrayAssignment : SmartContract.Framework.SmartContract
    {
        public static byte[] TestAssignment()
        {
            var a = new byte[] { 0x00, 0x02, 0x03 };
            a[0] = 0x01;
            a[2] = 0x04;
            return a;
        }

        public static byte[] testAssignmentOutOfBounds()
        {
            var a = new byte[] { 0x00, 0x02, 0x03 };
            a[0] = 0x01;
            a[3] = 0x04;
            return a;
        }

        public static byte[] testAssignmentOverflow()
        {
            int max = int.MaxValue;
            var a = new byte[] { 0x00, 0x02, 0x03 };
            a[0] = (byte)max;
            return a;
        }

        public static byte[] testAssignmentWrongCasting()
        {
            object obj = "test";
            var a = new byte[] { 0x00, 0x02, 0x03 };
            a[0] = (byte)obj;
            return a;
        }

        public static byte[] testAssignmentDynamic(byte x)
        {
            byte[] result = [0x01, x];
            return result;
        }
    }
}


