// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_TypeConvert.cs file belongs to the neo project and is free
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
    public class Contract_TypeConvert : SmartContract.Framework.SmartContract
    {
        public static object testType()
        {
            BigInteger int0 = 0;
            var bts0 = int0.ToByteArray();

            BigInteger int1 = 2;
            var bts1 = int1.ToByteArray();

            var bts2 = new byte[1] { 3 };
            var int2 = new BigInteger(bts2);

            var bts3 = new byte[0];
            var int3 = new BigInteger(bts3);

            var arrobj = new object[8];
            arrobj[0] = int0;
            arrobj[1] = bts0;
            arrobj[2] = int1;
            arrobj[3] = bts1;
            arrobj[4] = bts2;
            arrobj[5] = int2;
            arrobj[6] = bts3;
            arrobj[7] = int3;
            return arrobj;
        }
    }
}
