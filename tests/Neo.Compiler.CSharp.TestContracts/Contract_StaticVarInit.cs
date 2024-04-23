using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_StaticVarInit : SmartContract.Framework.SmartContract
    {
        //define and static var and init it with a runtime code.
        static byte[] callscript = (byte[])Runtime.EntryScriptHash;

        public static object StaticInit()
        {
            return TestStaticInit();
        }

        public static object DirectGet()
        {
            return Runtime.EntryScriptHash;
        }

        static byte[] TestStaticInit()
        {
            return callscript;
        }
    }
}
