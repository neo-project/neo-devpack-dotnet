using System;
using System.Collections.Generic;
using System.Text;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_UInt : SmartContract.Framework.SmartContract
    {
        public static UInt160 TestUInt160(String str)
        {
            UInt160 result = str.HexStringToUInt160();
            return result;
        }

        public static UInt256 TestUInt256(String str)
        {
            UInt256 result = str.HexStringToUInt256();
            return result;
        }
    }
}
