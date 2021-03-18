using System;
using System.Collections.Generic;
using System.Text;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_MapException : SmartContract
    {
        public static string TestByteArrayMap()
        {
            Map<byte[], string> some = new Map<byte[], string>();
            some[new byte[] { 0x01, 0x01 }] = StdLib.JsonSerialize("");
            return StdLib.JsonSerialize(some);
        }
    }
}
