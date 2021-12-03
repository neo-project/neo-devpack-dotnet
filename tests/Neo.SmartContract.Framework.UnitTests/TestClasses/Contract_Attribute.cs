using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class OwnerOnly : Modifier
    {
        UInt160 owner;

        public OwnerOnly(string hex)
        {
            UInt160 owner = (UInt160)(byte[])StdLib.Base64Decode(hex);
        }

        public override void Validate()
        {
            if (!Runtime.CheckWitness(owner)) throw new System.Exception();
        }
    }

    public class Contract_Attribute : SmartContract
    {
        [OwnerOnly("AAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
        public static bool test()
        {
            return true;
        }
    }
}
