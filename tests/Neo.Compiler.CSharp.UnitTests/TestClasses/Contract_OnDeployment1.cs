using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_OnDeployment1 : SmartContract.Framework.SmartContract
    {
        [DisplayName("_deploy")]
        public static void MyDeployMethod(object data, bool update)
        {

        }
    }
}
