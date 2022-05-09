using System;
using System.Numerics;
using System.ComponentModel;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_DuplicateNames : SmartContract.Framework.SmartContract
    {
        [DisplayName("Notify")]
        public static event Action<BigInteger> Notice;

        [DisplayName("Notify")]
        public static event Action<BigInteger> AA;
    }
}
