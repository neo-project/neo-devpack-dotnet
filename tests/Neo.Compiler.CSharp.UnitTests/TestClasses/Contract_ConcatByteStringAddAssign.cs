using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_Returns : SmartContract.Framework.SmartContract
    {
        public static ByteString ByteStringAddAssign(ByteString a, ByteString b, string c)
        {
            ByteString result = ByteString.Empty;
            result += a;
            result += b;
            result += c;
            return result;
        }
    }
}
