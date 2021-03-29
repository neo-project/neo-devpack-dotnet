using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Types_Float : SmartContract.Framework.SmartContract
    {
        public static float checkFloat() { return 0.1F; }
    }
}
