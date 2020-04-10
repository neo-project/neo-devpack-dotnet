using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_ByteArrayAssignment : SmartContract.Framework.SmartContract
    {
        public static byte[] testByteArrayAssignment()
        {
            var a = new byte[] { 0x00, 0x02, 0x03 };
            a[0] = 0x01;
            a[2] = 0x04;
            return a;
        }
    }
}
