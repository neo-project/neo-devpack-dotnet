namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_shift_bigint : SmartContract.Framework.SmartContract
    {
        public static object Main(string method, object[] args)
        {
            System.Numerics.BigInteger v = 8;
            var v1 = v << 0;
            var v2 = v << 1;
            var v3 = v >> 1;
            var v4 = v >> 2;
            SmartContract.Framework.Services.Neo.Runtime.Notify(v1);
            SmartContract.Framework.Services.Neo.Runtime.Notify(v2);
            SmartContract.Framework.Services.Neo.Runtime.Notify(v3);
            SmartContract.Framework.Services.Neo.Runtime.Notify(v4);
            return false;
        }
    }
}
