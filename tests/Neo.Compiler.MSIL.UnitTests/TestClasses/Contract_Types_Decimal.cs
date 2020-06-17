using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Types_Decimal : SmartContract.Framework.SmartContract
    {
        public static Decimal checkDecimal() { return 0.1M; }
    }
}
