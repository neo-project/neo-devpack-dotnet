using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_InvokeCsNef : SmartContract.Framework.SmartContract
    {
        /// <summary>
        /// One return
        /// </summary>
        public static int returnInteger()
        {
            return 42;
        }

        public static int Main()
        {
            return 22;
        }

        public static string returnString()
        {
            return "hello world";
        }
    }
}
