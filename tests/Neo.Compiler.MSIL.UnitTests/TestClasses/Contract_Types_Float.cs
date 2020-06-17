using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Types_Float : SmartContract.Framework.SmartContract
    {
        public static float checkFloat() { return 0.1F; }
    }
}
