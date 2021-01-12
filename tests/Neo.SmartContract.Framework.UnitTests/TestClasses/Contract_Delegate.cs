using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_Delegate : SmartContract.Framework.SmartContract
    {
        private static string TestDynamicCall(byte[] token, string method)
        {
            var result = ((Func<string, object[], string>)token.ToDelegate())(method, new object[0]);
            return result;
        }
    }
}
