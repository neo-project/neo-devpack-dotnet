using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.InteropServices;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_OnDeployment : SmartContract.Framework.SmartContract
    {
        [OnDeployment]
        public static void MyDeployMethod(bool update)
        {
            
        }
    }
}
