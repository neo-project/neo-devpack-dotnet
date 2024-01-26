using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_StaticVarInit : SmartContract.Framework.SmartContract
    {
        //define and staticvar and initit with a runtime code.
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
