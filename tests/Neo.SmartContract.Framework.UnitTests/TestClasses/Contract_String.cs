using System;
using System.Collections.Generic;
using System.Text;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;

namespace Neo.Compiler.MSIL.TestClasses
{
    public class Contract_String : SmartContract.Framework.SmartContract
    {
        public static int TestStringAdd(string s1, string s2)
        {
            int a = 3;
            string c = s1 + s2;
            if (c == "student")
            {
                a = 4;
            }
            else if (c == "test")
            {
                a = 5;
            }
            return a;
        }
    }
}
