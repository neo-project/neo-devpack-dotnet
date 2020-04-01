using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_BufferReverse : SmartContract.Framework.SmartContract
    {
        public static byte[] testType()
        {
            var arr = (new byte[]{ 0x01, 0x02, 0x03 }).Reverse(); 
            return arr;
        }
    }
}
