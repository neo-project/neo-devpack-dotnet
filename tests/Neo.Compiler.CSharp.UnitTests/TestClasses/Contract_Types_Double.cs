using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    class Contract_Types_Float : SmartContract.Framework.SmartContract
    {
        public static double checkFloat() { return 0.1D; }
    }
}
