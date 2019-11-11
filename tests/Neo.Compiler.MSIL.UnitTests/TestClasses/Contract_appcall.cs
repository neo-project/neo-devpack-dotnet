using Neo.SmartContract.Framework;

namespace Neo.Compiler.MSIL.TestClasses
{
    class Contract_syscall : SmartContract.Framework.SmartContract
    {
        //这个appcall的地址，在testcase中可以配置
        [Appcall("0102030405060708090A0102030405060708090A")]
        static extern object unittest001(string method, object[] arr);

        public static object Main(string method, object[] args)
        {
            return unittest001(method, args);
        }
    }
}
