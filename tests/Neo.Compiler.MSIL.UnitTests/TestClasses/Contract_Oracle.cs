using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    public class Contract_Oracle : SmartContract.Framework.SmartContract
    {
        public static byte[][] testGetOracleNodes()
        {
            return Neo.SmartContract.Framework.Services.Neo.Oracle.GetOracleNodes();
        }
    }
}
