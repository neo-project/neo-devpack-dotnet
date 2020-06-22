namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_shift : SmartContract.Framework.SmartContract
    {
        public static object Main()
        {
            int v = 8;
            var v1 = v << 1;
            var v2 = v << -1;
            var v3 = v >> 1;
            var v4 = v >> -1;
            SmartContract.Framework.Services.Neo.Runtime.Notify("1", v1);
            SmartContract.Framework.Services.Neo.Runtime.Notify("2", v2);
            SmartContract.Framework.Services.Neo.Runtime.Notify("3", v3);
            SmartContract.Framework.Services.Neo.Runtime.Notify("4", v4);
            return false;
        }
    }
}
