using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class OwnerOnly : Modifier
    {
        public override void Validate()
        {
            UInt160 owner = (UInt160)new byte[20];
            if (!Runtime.CheckWitness(owner)) throw new System.Exception();
        }
    }

    public class Contract_Attribute : SmartContract
    {
        [OwnerOnly]
        public static bool test()
        {
            return true;
        }
    }
}
