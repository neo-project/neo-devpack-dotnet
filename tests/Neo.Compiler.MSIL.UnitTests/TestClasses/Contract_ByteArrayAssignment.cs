using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_ByteArrayAssignment : SmartContract.Framework.SmartContract
    {
        public static byte[] testAssignment()
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
            object obj = int.MaxValue;
            var a = new byte[] { 0x00, 0x02, 0x03 };
            a[0] = (byte)obj;
            return a;
        }

        public static byte[] testAssignmentOverflowUncheked()
        {
            var a = new byte[] { 0x00, 0x02, 0x03 };
            a[0] = unchecked((byte)int.MaxValue);
            return a;
        }

        public static byte[] testAssignmentWrongCasting()
        {
            object obj = "test";
            var a = new byte[] { 0x00, 0x02, 0x03 };
            a[0] = (byte)obj;
            return a;
        }
    }
}
