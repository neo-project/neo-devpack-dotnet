using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.UnitTests.TestClasses
{
    class Contract_syscall : SmartContract.Framework.SmartContract
    {
        //这个appcall的地址，在testcase中可以配置
        //[Appcall("0102030405060708090A0102030405060708090A")]
        [Syscall("System.Contract.Call")]
        static extern object unittest001(byte[] scriptHash, string method, object[] arguments);

        public static object testAppCall()
        {
            var scriptHash = new byte[] { 0x0A, 0x09, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01, 0x0A, 0x09, 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };
            var methodName = "unitTest_001";
            object[] arguments = new object[0] { };
            return unittest001(scriptHash, methodName, arguments);
        }
    }
}
