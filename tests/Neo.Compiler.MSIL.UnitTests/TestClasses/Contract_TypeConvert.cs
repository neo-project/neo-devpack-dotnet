using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_TypeConvert : SmartContract.Framework.SmartContract
    {
        public static object testType()
        {
            BigInteger int0 = 0;
            var bts0 = int0.ToByteArray();

            BigInteger int1 = 2;
            var bts1 = int1.ToByteArray();

            var bts2 = new byte[1] { 3 };
            var int2 = bts2.ToBigInteger();

            var bts3 = new byte[0];
            var int3 = bts3.ToBigInteger();

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
