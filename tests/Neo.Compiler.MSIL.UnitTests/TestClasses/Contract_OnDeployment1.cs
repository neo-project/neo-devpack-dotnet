using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Numerics;
using System.Runtime.InteropServices;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_OnDeployment1 : SmartContract.Framework.SmartContract
    {
        [DisplayName("_deploy")]
        public static void MyDeployMethod(bool update)
        {
            
        }
    }
}
