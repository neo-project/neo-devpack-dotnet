using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_Types_BigInteger : SmartContract.Framework.SmartContract
    {
        public static BigInteger Zero() { return BigInteger.Zero; }
        public static BigInteger One() { return BigInteger.One; }
        public static BigInteger MinusOne() { return BigInteger.MinusOne; }
    }
}
