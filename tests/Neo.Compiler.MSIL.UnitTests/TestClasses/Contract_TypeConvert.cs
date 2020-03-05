using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{

    class Contract_TypeConvert : SmartContract.Framework.SmartContract
    {
        //Integer = 0x21,
        //ByteArray = 0x28,
        [OpCode(SmartContract.Framework.OpCode.CONVERT, "0x28")]
        public extern static byte[] ConvertToByteArray(object from);


        [OpCode(SmartContract.Framework.OpCode.CONVERT, "0x21")]
        public extern static BigInteger ConvertToInteger(byte[] from);




        public static object testType()
        {
            BigInteger int0 = 0;
            var bts0 = ConvertToByteArray(int0);
            BigInteger int1 = 2;
            var bts1 = ConvertToByteArray(int1);

            var bts2=new byte[1] { 3 };
            var int2 = ConvertToInteger(bts2);

            var bts3 = new byte[0];
            var int3 = ConvertToInteger(bts3);

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
