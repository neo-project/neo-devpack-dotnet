using System.Collections.Generic;
using System.Text;
using System.Numerics;

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
		
    }
}