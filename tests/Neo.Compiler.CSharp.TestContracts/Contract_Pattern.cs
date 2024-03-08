using System;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Pattern : SmartContract.Framework.SmartContract
    {
        public bool between(int value)
        {
            return value is > 1 and < 100;
        }

        public bool between2(int value)
        {
            return value is (> 1 and < 100);
        }

        public bool between3(int value)
        {
            return value switch
            {
                (> 1 and < 50) => true,
                (>= 50 and < 100) => true,
                _ => false,
            };
        }
    }
}
